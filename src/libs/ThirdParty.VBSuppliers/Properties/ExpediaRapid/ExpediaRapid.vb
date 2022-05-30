Imports System.Net
Imports Intuitive
Imports Intuitive.Helpers.Extensions
Imports Intuitive.Net
Imports MoreLinq
Imports Newtonsoft.Json
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.VBSuppliers.ExpediaRapid.RequestConstants
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses.Book
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses.BookingItinerary
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses.Prebook
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses.Search

Namespace ExpediaRapid

    Public Class ExpediaRapid
        Implements IThirdParty

        Private ReadOnly _api As IExpediaRapidAPI

        Private ReadOnly _settings As IExpediaRapidSettings

        Private ReadOnly _source As String = ThirdParties.EXPEDIARAPID

        Private ReadOnly _support As ITPSupport

        Private Const ErrataTitle As String = "Pay at Hotel"

        Public Sub New(api As IExpediaRapidAPI, settings As IExpediaRapidSettings, support As ITPSupport)
            _api = Ensure.IsNotNull(api, NameOf(api))
            _settings = Ensure.IsNotNull(settings, NameOf(settings))
            _support = Ensure.IsNotNull(support, NameOf(support))
        End Sub

        Public ReadOnly Property SupportsRemarks As Boolean = True Implements IThirdParty.SupportsRemarks
        Public ReadOnly Property SupportsBookingSearch As Boolean = False Implements IThirdParty.SupportsBookingSearch

        Public Function RequiresVCard(info As VirtualCardInfo) As Boolean Implements IThirdParty.RequiresVCard
            Throw New NotImplementedException()
        End Function

        Public Sub EndSession(propertyDetails As PropertyDetails) Implements IThirdParty.EndSession
            Return
        End Sub

        Public Function PreBook(propertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook

            Try
                ' retry search to get cancellation and errata
                Dim searchResponse As SearchResponse = GetPrebookSearchRedoResponse(propertyDetails)

                If searchResponse Is Nothing Then
                    Throw New Exception("Errors in second search response.")
                End If

                Dim responseRooms As List(Of SearchResponseRoom) =
                    GetPrebookSpecificRoomRates(propertyDetails, searchResponse)

                If Not responseRooms.Any() Then
                    Throw New Exception("Room(s) no longer available. Retry search.")
                End If

                propertyDetails.Errata = GetErrataFromAllRooms(propertyDetails, responseRooms)

                propertyDetails.Cancellations = GetCancellationsFromAllRooms(propertyDetails, responseRooms)
                propertyDetails.Cancellations.Solidify(SolidifyType.Sum)

            Catch ex As Exception
                propertyDetails.Warnings.AddNew("Prebook RedoSearch Error", ex.ToString())
                Return False
            End Try

            Dim roomPrebookResponses As New List(Of PrebookResponse)
            ' prebook to get the costs and book link
            Try
                Dim firstRoom As RoomDetails = propertyDetails.Rooms.First()

                Dim prebookResponse As PrebookResponse =
                        GetResponse(Of PrebookResponse)(propertyDetails,
                                                        firstRoom.ThirdPartyReference.Split("|"c)(1),
                                                        WebRequests.eRequestMethod.GET,
                                                        "Prebook PriceCheck ",
                                                        False)

                If prebookResponse Is Nothing Then
                    Throw New Exception("Room price check failed.")
                End If

                UpdateCostsAtRoomLevel(propertyDetails, prebookResponse)
                GetBookingLink(propertyDetails, firstRoom, prebookResponse)

            Catch ex As Exception
                propertyDetails.Warnings.AddNew("Prebook Price Check Error",
                                                ex.ToString())
                Return False
            End Try

            propertyDetails.LocalCost = propertyDetails.Rooms.Sum(Function(r) r.LocalCost)
            propertyDetails.GrossCost = propertyDetails.Rooms.Sum(Function(r) r.GrossCost)

            Return True
        End Function

        Public Function Book(propertyDetails As PropertyDetails) As String Implements IThirdParty.Book

            Dim roomBookResults As New Dictionary(Of String, Result)

            Try
                Dim firstRoom As RoomDetails = propertyDetails.Rooms.First()
                Dim bookRequestBody As String = BuildBookRequestBody(propertyDetails)

                Dim bookResponse As BookResponse =
                        GetResponse(Of BookResponse)(propertyDetails,
                                                     propertyDetails.TPRef2,
                                                     WebRequests.eRequestMethod.POST,
                                                     "Book ",
                                                     True,
                                                     bookRequestBody)


                If bookResponse Is Nothing Then
                    Throw New ArgumentNullException("bookResponse", "Invalid book response recieved for room.")
                End If

                propertyDetails.SourceSecondaryReference = bookResponse.Links("retrieve").HRef
                Return bookResponse.ItineraryID

            Catch ex As Exception
                propertyDetails.Warnings.AddNew("Book Error", ex.ToString())
                Return "failed"
            End Try
        End Function

        Public Function CancelBooking(propertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking

            Dim amount As Decimal = 0
            Dim cancelReferences As New List(Of String)
            Dim success As Boolean = True

            Try
                Dim bookingItineraryResponse1 As BookingItineraryResponse =
                        GetResponse(Of BookingItineraryResponse)(propertyDetails,
                                                                 propertyDetails.SourceSecondaryReference,
                                                                 WebRequests.eRequestMethod.GET,
                                                                 "Booking Itinerary - Before Cancel ",
                                                                 True)

                If bookingItineraryResponse1 Is Nothing Then
                    Throw New ArgumentNullException("bookingItineraryResponse1",
                                                    "Unable to find booking itinerary for room.")
                End If

                cancelReferences = CancelRooms(propertyDetails, bookingItineraryResponse1)

                If propertyDetails.Warnings.Any() Then
                    Throw New Exception("Unable to cancel a room.")
                End If

                Dim bookingItineraryResponse2 As BookingItineraryResponse =
                        GetResponse(Of BookingItineraryResponse)(propertyDetails,
                                                                 propertyDetails.SourceSecondaryReference,
                                                                 WebRequests.eRequestMethod.GET,
                                                                 "Booking Itinerary - After Cancel ",
                                                                 True)

                If bookingItineraryResponse2 Is Nothing Then
                    Throw New ArgumentNullException("bookingItineraryResponse2",
                                                    "Unable to find booking itinerary for room.")
                End If

                If bookingItineraryResponse2.Rooms.Any(Function(r) r.Status <> "canceled") Then
                    Throw New Exception("Unable to cancel room.")
                End If

                Dim cancelPenalties As IEnumerable(Of CancelPenalty) =
                    bookingItineraryResponse2.Rooms.SelectMany(Function(r) r.Rate.CancelPenalities)

                If cancelPenalties.Any() Then
                    amount += cancelPenalties.Where(Function(cp) cp.CancelStartDate < Date.Now _
                                                            AndAlso cp.CancelEndDate > Date.Now) _
                                                 .Sum(Function(cp) cp.Amount)
                End If

            Catch ex As Exception
                propertyDetails.Warnings.AddNew("Cancel Error",
                                                ex.ToString())
                success = False
            End Try

            Return New ThirdPartyCancellationResponse() With {
                .Amount = amount,
                .CurrencyCode = _support.CurrencyLookup(propertyDetails.CurrencyID),
                .Success = success,
                .CostRecievedFromThirdParty = amount > 0,
                .TPCancellationReference = String.Join("|", cancelReferences)
            }
        End Function

        Public Function GetCancellationCost(propertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost

            Dim amount As Decimal = 0
            Dim success As Boolean = True

            Try
                Dim precancelResponse As BookingItineraryResponse =
                        GetResponse(Of BookingItineraryResponse)(propertyDetails,
                                                                 propertyDetails.SourceSecondaryReference,
                                                                 WebRequests.eRequestMethod.GET,
                                                                 "Booking Itinerary ",
                                                                 True)

                If precancelResponse Is Nothing Then
                    Throw New ArgumentNullException("precancelResponse",
                                                    "There was an error in the precancel response.")
                End If

                If (precancelResponse.AffiliateReferenceID <> propertyDetails.BookingReference.Trim()) AndAlso _settings.ValidateAffiliateID(propertyDetails) Then
                    Throw New Exception("Unrecognised precancel response booking reference.")
                End If

                Dim cancelPenalties As IEnumerable(Of CancelPenalty) =
                    precancelResponse.Rooms.SelectMany(Function(r) r.Rate.CancelPenalities)

                If cancelPenalties.Any() Then
                    amount += cancelPenalties.Where(Function(cp) cp.CancelStartDate < Date.Now _
                                                                AndAlso cp.CancelEndDate > Date.Now) _
                                                .Sum(Function(cp) cp.Amount)
                End If

            Catch ex As Exception
                propertyDetails.Warnings.AddNew("Precancel Error", ex.ToString())
                success = False
            End Try

            Return New ThirdPartyCancellationFeeResult() With {
                .Amount = amount,
                .CurrencyCode = _support.CurrencyLookup(propertyDetails.CurrencyID),
                .Success = success
            }
        End Function

        Public Function BookingSearch(bookingSearchDetails As BookingSearchDetails) As ThirdPartyBookingSearchResults Implements IThirdParty.BookingSearch
            Throw New NotImplementedException()
        End Function

        Public Function BookingStatusUpdate(propertyDetails As PropertyDetails) As ThirdPartyBookingStatusUpdateResult Implements IThirdParty.BookingStatusUpdate
            Return New ThirdPartyBookingStatusUpdateResult()
        End Function

        Public Function CreateReconciliationReference(inputReference As String) As String Implements IThirdParty.CreateReconciliationReference
            Return String.Empty
        End Function

        Private Function CancelRooms(
                propertyDetails As PropertyDetails, bookingItineraryResponse1 As BookingItineraryResponse) As List(Of String)

            Dim cancelReferences As New List(Of String)

            For Each room As BookingItineraryResponseRoom In bookingItineraryResponse1.Rooms
                Try
                    Dim cancelLink As String = room.Links("cancel").HRef

                    Dim url As String = BuildDefaultURL(propertyDetails, cancelLink)
                    Dim request As WebRequests.Request = BuildRequest(propertyDetails,
                                                                      url,
                                                                      "Cancel",
                                                                      WebRequests.eRequestMethod.DELETE,
                                                                      True)

                    Dim cancelResponseString As String = _api.GetResponse(propertyDetails, request)
                    Dim statusCode As Integer = request.HTTPWebResponse.StatusCode

                    If Not String.IsNullOrWhiteSpace(cancelResponseString) _
                        AndAlso (statusCode <> 202 OrElse statusCode <> 204) Then

                        Throw New ArgumentNullException("cancelResponse",
                                                "There was an error in the cancel response.")
                    End If

                    cancelReferences.Add(propertyDetails.SourceReference)

                Catch ex As Exception
                    propertyDetails.Warnings.AddNew("Cancel Error",
                                                ex.ToString())
                    cancelReferences.Add("[Failed]")
                End Try
            Next

            Return cancelReferences
        End Function

        Private Sub GetBookingLink(
                propertyDetails As PropertyDetails,
                firstRoom As RoomDetails,
                prebookResponse As PrebookResponse)

            Dim bookLink As Link = Nothing
            If Not prebookResponse.Links.TryGetValue("book", bookLink) Then
                Throw New Exception("Couldn't find booklink for room in prebook response")
            End If

            Dim tpSessionID As String = firstRoom.ThirdPartyReference.Split("|"c)(0)
            propertyDetails.TPRef1 = tpSessionID
            propertyDetails.TPRef2 = bookLink.HRef
        End Sub

        Private Sub UpdateCostsAtRoomLevel(propertyDetails As PropertyDetails, prebookResponse As PrebookResponse)
            For Each room As RoomDetails In propertyDetails.Rooms
                Dim occupancy As New ExpediaRapidOccupancy(room.Adults, room.ChildAges, room.Infants)

                Dim occupancyRoomRate As OccupancyRoomRate = Nothing

                If Not prebookResponse.OccupancyRoomRates.TryGetValue(occupancy.GetExpediaRapidOccupancy(), occupancyRoomRate) Then
                    Throw New Exception("Couldn't find room in prebook response")
                End If

                Dim inclusiveTotal As Decimal =
                        occupancyRoomRate.OccupancyRateTotals("inclusive").TotalInRequestCurrency.Amount
                room.LocalCost = inclusiveTotal
                room.GrossCost = inclusiveTotal
            Next
        End Sub

        Private Function GetResponse(Of TResponse As {IExpediaRapidResponse, New})(
                propertyDetails As PropertyDetails,
                link As String,
                method As WebRequests.eRequestMethod,
                logName As String,
                addCustomerIPHeader As Boolean,
                Optional requestBody As String = Nothing) As TResponse

            Dim url As String = BuildDefaultURL(propertyDetails, link)
            Dim request As WebRequests.Request = BuildRequest(propertyDetails,
                                                                    url,
                                                                    logName,
                                                                    method,
                                                                    addCustomerIPHeader,
                                                                    requestBody)
            Dim response As TResponse = _api.GetDeserializedResponse(Of TResponse)(propertyDetails, request)
            Return response
        End Function

        Private Function GetPrebookSearchRedoResponse(propertyDetails As PropertyDetails) As SearchResponse

            Dim searchURL As String = BuildPrebookSearchURL(propertyDetails)
            Dim searchRequest As WebRequests.Request = BuildRequest(propertyDetails,
                                                                    searchURL,
                                                                    "Prebook - Redo Search",
                                                                    WebRequests.eRequestMethod.GET,
                                                                    False)
            Dim searchResponse As SearchResponse = _api.GetDeserializedResponse(Of SearchResponse)(propertyDetails, searchRequest)
            Return searchResponse
        End Function

        Private Function GetCancellationsFromAllRooms(
                propertyDetails As PropertyDetails,
                responseRooms As List(Of SearchResponseRoom)) As Cancellations

            Dim cancellations As New Cancellations

            For Each room As RoomDetails In propertyDetails.Rooms
                Dim roomID As String = room.RoomTypeCode.Split("|"c)(0)
                Dim rateID As String = room.RoomTypeCode.Split("|"c)(1)
                Dim roomrate As RoomRate = GetExactRoomRate(responseRooms, roomID, rateID)

                Dim occuapncy As New ExpediaRapidOccupancy(room.Adults, room.ChildAges, room.Infants)
                Dim occupancyRoomRate As OccupancyRoomRate = GetExactOccupancyRoomRate(responseRooms, occuapncy, roomID, rateID)

                If Not roomrate.IsRefundable AndAlso Not roomrate.CancelPenalities.Any Then
                    Dim roomAmount As Decimal = occupancyRoomRate.OccupancyRateTotals("inclusive").TotalInRequestCurrency.Amount
                    cancellations.Add(New Cancellation(DateTime.Now.Date, propertyDetails.ArrivalDate, roomAmount))
                End If

                cancellations.AddRange(roomrate.CancelPenalities.Select(Function(cp) BuildCancellation(occupancyRoomRate, cp)))
            Next

            Return cancellations
        End Function

        Private Function BuildCancellation(occupancyRoomRate As OccupancyRoomRate, cancelPenalty As CancelPenalty) As Cancellation

            Dim amount As Decimal = 0

            If cancelPenalty.Amount <> 0 Then
                amount += cancelPenalty.Amount
            End If

            If cancelPenalty.Percent <> "" Then
                Dim percent As Decimal = cancelPenalty.Percent.Replace("%"c, "").ToSafeDecimal()
                Dim roomAmount As Decimal = occupancyRoomRate.OccupancyRateTotals("inclusive").TotalInRequestCurrency.Amount
                amount += Math.Round(roomAmount / 100 * percent, 2, MidpointRounding.AwayFromZero)
            End If

            If cancelPenalty.Nights > 0 Then
                Dim nightCancellations As List(Of List(Of Rate)) = occupancyRoomRate.NightlyRates.Take(cancelPenalty.Nights).ToList()
                For Each night As List(Of Rate) In nightCancellations
                    For Each rate As Rate In night
                        amount += rate.Amount
                    Next
                Next
            End If

            Dim cancellation As New Cancellation(cancelPenalty.CancelStartDate, cancelPenalty.CancelEndDate, amount)

            Return cancellation
        End Function

        Private Function GetErrataFromAllRooms(
                propertyDetails As PropertyDetails,
                responseRooms As List(Of SearchResponseRoom)) As Errata

            Dim errata As New Errata
            Dim currencyCode As String = _support.TPCurrencyLookup(_source, propertyDetails.CurrencyCode)

            Dim mandatoryFees As New List(Of OccupancyRateFee)
            Dim resortFees As New List(Of OccupancyRateFee)
            Dim mandatoryTaxes As New List(Of OccupancyRateFee)
            Dim taxAndServiceFee As Decimal = 0
            Dim salesTax As Decimal = 0
            Dim extraPersonFee As Decimal = 0

            For Each room As RoomDetails In propertyDetails.Rooms
                Dim occuapncy As New ExpediaRapidOccupancy(room.Adults, room.ChildAges, room.Infants)
                Dim roomID As String = room.RoomTypeCode.Split("|"c)(0)
                Dim rateID As String = room.RoomTypeCode.Split("|"c)(1)

                Dim occupancyRoomRate As OccupancyRoomRate =
                    GetExactOccupancyRoomRate(responseRooms, occuapncy, roomID, rateID)

                If occupancyRoomRate.OccupancyRateFees.Any() Then
                    Dim mandatoryFee As OccupancyRateFee = Nothing
                    If occupancyRoomRate.OccupancyRateFees.TryGetValue("mandatory_fee", mandatoryFee) Then
                        mandatoryFees.Add(mandatoryFee)
                    End If

                    Dim resortFee As OccupancyRateFee = Nothing
                    If occupancyRoomRate.OccupancyRateFees.TryGetValue("resort_fee", resortFee) Then
                        resortFees.Add(resortFee)
                    End If

                    Dim mandatoryTax As OccupancyRateFee = Nothing
                    If occupancyRoomRate.OccupancyRateFees.TryGetValue("mandatory_tax", mandatoryTax) Then
                        mandatoryTaxes.Add(mandatoryTax)
                    End If
                End If

                taxAndServiceFee += ExpediaRapidSearch.GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.TaxAndServiceFee)
                salesTax += ExpediaRapidSearch.GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.SalesTax)
                extraPersonFee += ExpediaRapidSearch.GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.ExtraPersonFee)

                If occupancyRoomRate.StayRates.Any() Then
                    taxAndServiceFee += ExpediaRapidSearch.GetStayRateFromType(occupancyRoomRate, RateTypes.TaxAndServiceFee)
                    salesTax += ExpediaRapidSearch.GetStayRateFromType(occupancyRoomRate, RateTypes.SalesTax)
                    extraPersonFee += ExpediaRapidSearch.GetStayRateFromType(occupancyRoomRate, RateTypes.ExtraPersonFee)
                End If

            Next

            If mandatoryFees.Any() Then errata.Add(BuildErrata("Mandatory Fee",
                                                               mandatoryFees.Sum(Function(f) f.TotalInRequestCurrency.Amount),
                                                               currencyCode))
            If resortFees.Any() Then errata.Add(BuildErrata("Resort Fee",
                                                            resortFees.Sum(Function(f) f.TotalInRequestCurrency.Amount),
                                                            currencyCode))
            If mandatoryTaxes.Any() Then errata.Add(BuildErrata("Mandatory Tax",
                                                                mandatoryTaxes.Sum(Function(f) f.TotalInRequestCurrency.Amount),
                                                                currencyCode))
            If taxAndServiceFee > 0 Then errata.Add(BuildErrata("Tax and Service Fee", taxAndServiceFee, currencyCode))
            If salesTax > 0 Then errata.Add(BuildErrata("Sales Tax", salesTax, currencyCode))
            If extraPersonFee > 0 Then errata.Add(BuildErrata("Extra Person", extraPersonFee, currencyCode))

            Return errata
        End Function

        Private Function GetExactOccupancyRoomRate(
                responseRooms As List(Of SearchResponseRoom),
                occuapncy As ExpediaRapidOccupancy,
                roomID As String,
                rateID As String) As OccupancyRoomRate

            Dim roomRate As RoomRate = GetExactRoomRate(responseRooms, roomID, rateID)
            Dim occupancyRoomRate As OccupancyRoomRate = roomRate.OccupancyRoomRates(occuapncy.GetExpediaRapidOccupancy())

            Return occupancyRoomRate
        End Function

        Private Function GetExactRoomRate(
                responseRooms As List(Of SearchResponseRoom),
                roomID As String,
                rateID As String) As RoomRate

            Dim responseRoom As SearchResponseRoom = responseRooms.First(Function(r) r.RoomID = roomID)
            Dim roomRate As RoomRate = responseRoom.Rates.First(Function(r) r.RateID = rateID)
            Return roomRate
        End Function

        Private Function BuildErrata(
                feeName As String,
                amount As Decimal,
                currencyCode As String) As Erratum

            Return New Erratum(ErrataTitle, $"{feeName}: {currencyCode}/{amount}")
        End Function

        Private Function GetPrebookSpecificRoomRates(
                propertyDetails As PropertyDetails,
                searchResponse As SearchResponse) As List(Of SearchResponseRoom)

            Dim propertyAvail As PropertyAvailablility =
                searchResponse.First(Function(sr) sr.PropertyID = propertyDetails.TPKey)

            If propertyAvail Is Nothing OrElse Not propertyAvail.Rooms.Any() Then Return Nothing

            Dim sameRoomRates As New List(Of SearchResponseRoom)

            For Each room As RoomDetails In propertyDetails.Rooms
                Dim roomID As String = room.RoomTypeCode.Split("|"c)(0)
                Dim rateID As String = room.RoomTypeCode.Split("|"c)(1)
                Dim bedGroupID As String = room.RoomTypeCode.Split("|"c)(2)

                Dim matchedRoom As SearchResponseRoom = propertyAvail.Rooms.First(Function(r) r.RoomID = roomID)

                If matchedRoom Is Nothing OrElse Not matchedRoom.Rates.Any() Then Return Nothing

                Dim matchedRate As RoomRate = matchedRoom.Rates.First(Function(r) r.RateID = rateID)

                If matchedRate Is Nothing _
                    OrElse matchedRate.BedGroupAvailabilities Is Nothing _
                    OrElse Not matchedRate.BedGroupAvailabilities.ContainsKey(bedGroupID) Then Return Nothing

                Dim occupancy As New ExpediaRapidOccupancy(room.Adults, room.ChildAges, room.Infants)

                If matchedRate.OccupancyRoomRates(occupancy.GetExpediaRapidOccupancy()) Is Nothing Then Return Nothing

                sameRoomRates.Add(matchedRoom)
            Next

            Return sameRoomRates
        End Function

        Private Function BuildPrebookSearchURL(propertyDetails As PropertyDetails) As String

            Dim tpKeys As New List(Of String) From {propertyDetails.TPKey}
            Dim currencyCode As String = _support.TPCurrencyLookup(_source, propertyDetails.CurrencyCode)
            Dim occupancies As IEnumerable(Of ExpediaRapidOccupancy) =
                propertyDetails.Rooms.Select(Function(r) New ExpediaRapidOccupancy(r.Adults, r.ChildAges, r.Infants))

            Return ExpediaRapidSearch.BuildSearchURL(tpKeys,
                                                     _settings,
                                                     propertyDetails,
                                                     propertyDetails.ArrivalDate,
                                                     propertyDetails.DepartureDate,
                                                     currencyCode,
                                                     occupancies)
        End Function

        Private Function BuildRequest(
                propertyDetails As PropertyDetails,
                url As String,
                logName As String,
                method As WebRequests.eRequestMethod,
                addCustomerIPHeader As Boolean,
                Optional requestBody As String = Nothing) As WebRequests.Request

            Dim useGzip As Boolean = _settings.UseGZIP(propertyDetails)
            Dim apiKey As String = _settings.ApiKey(propertyDetails)
            Dim secret As String = _settings.Secret(propertyDetails)
            Dim userAgent As String = _settings.UserAgent(propertyDetails)
            Dim customerIp As String = Dns.GetHostEntry(Dns.GetHostName()).AddressList(0).MapToIPv4().ToString()
            Dim tpRef As String = propertyDetails.TPRef1
            Dim tpSessionID As String = If(Not String.IsNullOrWhiteSpace(tpRef), tpRef.Split("|"c)(0), Guid.NewGuid().ToString())
            Dim headers As New WebRequests.RequestHeaders

            With headers
                .AddNew(SearchHeaderKeys.CustomerSessionID, tpSessionID)
                .Add(ExpediaRapidSearch.CreateAuthorizationHeader(apiKey, secret))
                If addCustomerIPHeader Then .AddNew(SearchHeaderKeys.CustomerIP, customerIp)
            End With

            Dim request As WebRequests.Request =
                            ExpediaRapidSearch.BuildDefaultRequest(url,
                                                                   method,
                                                                   headers,
                                                                   useGzip,
                                                                   propertyDetails.CreateLogs,
                                                                   userAgent,
                                                                   logName,
                                                                   requestBody)

            Return request
        End Function

        Private Function BuildBookRequestBody(
                propertyDetails As PropertyDetails) As String

            Dim bookRequest As New BookRequest With {
                .AffiliateReferenceId = propertyDetails.BookingReference.Trim(),
                .Hold = False,
                .Rooms = propertyDetails.Rooms.Select(Function(r, i) CreateBookRequestRoom(propertyDetails, r.Passengers.First(), i)).ToList(),
                .Payments = New List(Of Payment) From {
                    New Payment() With {
                        .Type = "affiliate_collect",
                        .BillingContact = New BillingContact() With {
                            .GivenName = propertyDetails.LeadGuestFirstName,
                            .FamilyName = propertyDetails.LeadGuestLastName,
                            .Email = propertyDetails.LeadGuestEmail,
                            .Phone = New Phone(propertyDetails.LeadGuestPhone),
                            .Address = New Book.Address() With {
                                .Line1 = propertyDetails.LeadGuestAddress1,
                                .Line2 = propertyDetails.LeadGuestAddress2,
                                .City = propertyDetails.LeadGuestTownCity,
                                .StateProvinceCode = propertyDetails.LeadGuestCounty,
                                .PostalCode = propertyDetails.LeadGuestPostcode,
                                .CountryCode = _support.TPBookingCountryLookup(Me._source, propertyDetails.LeadGuestBookingCountryID)
                            }
                        }
                    }
                }
            }

            Return JsonConvert.SerializeObject(bookRequest)
        End Function

        Private Function CreateBookRequestRoom(
                propertyDetails As PropertyDetails,
                firstPasseneger As Passenger,
                index As Integer) As BookRequestRoom

            If index = 0 Then
                Return New BookRequestRoom() With {
                    .Title = propertyDetails.LeadGuestTitle,
                    .GivenName = propertyDetails.LeadGuestFirstName,
                    .FamilyName = propertyDetails.LeadGuestLastName,
                    .Email = propertyDetails.LeadGuestEmail,
                    .Phone = New Phone(propertyDetails.LeadGuestPhone),
                    .SpecialRequest = String.Join(Environment.NewLine, propertyDetails.BookingComments.Select(Function(bc) bc.Text))
                }
            Else
                Return New BookRequestRoom() With {
                    .Title = firstPasseneger.Title,
                    .GivenName = firstPasseneger.FirstName,
                    .FamilyName = firstPasseneger.LastName,
                    .Email = propertyDetails.LeadGuestEmail,
                    .Phone = New Phone(propertyDetails.LeadGuestPhone),
                    .SpecialRequest = String.Join(Environment.NewLine, propertyDetails.BookingComments.Select(Function(bc) bc.Text))
                }
            End If
        End Function

        Private Function BuildDefaultURL(
                propertyDetails As PropertyDetails,
                path As String) As String

            Dim uriBuilder As New UriBuilder(_settings.Scheme(propertyDetails),
                                             _settings.Host(propertyDetails),
                                             -1)

            If String.IsNullOrWhiteSpace(path) Then Throw New ArgumentNullException("path", "No link supplied to append to URL.")

            Return uriBuilder.Uri.AbsoluteUri.TrimEnd("/"c) + path
        End Function

        Private Function IThirdParty_SupportsLiveCancellation(searchDetails As IThirdPartyAttributeSearch, source As String) As Boolean Implements IThirdParty.SupportsLiveCancellation
            Return _settings.AllowCancellations(searchDetails)
        End Function

        Private Function IThirdParty_TakeSavingFromCommissionMargin(searchDetails As IThirdPartyAttributeSearch) As Boolean Implements IThirdParty.TakeSavingFromCommissionMargin
            Throw New NotImplementedException()
        End Function

        Private Function IThirdParty_OffsetCancellationDays(searchDetails As IThirdPartyAttributeSearch) As Integer Implements IThirdParty.OffsetCancellationDays
            Throw New NotImplementedException()
        End Function

    End Class

End Namespace