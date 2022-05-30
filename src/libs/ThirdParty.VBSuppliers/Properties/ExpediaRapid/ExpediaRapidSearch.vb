Imports System.Collections.Specialized
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports Intuitive
Imports Intuitive.Net.WebRequests
Imports ThirdParty.VBSuppliers.ExpediaRapid.RequestConstants
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses
Imports ThirdParty.VBSuppliers.ExpediaRapid.SerializableClasses.Search
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Constants
Imports Newtonsoft.Json
Imports ThirdParty.Abstractions.Results
Imports MoreLinq.Extensions
Imports ThirdParty.Abstractions.Search.Models
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Search.Support
Imports Intuitive.Helpers.Extensions

Namespace ExpediaRapid

    Public Class ExpediaRapidSearch
        Inherits ThirdPartyPropertySearchBase

        Private ReadOnly _settings As IExpediaRapidSettings

        Private ReadOnly _support As ITPSupport

        Public Sub New(settings As IExpediaRapidSettings, support As ITPSupport)
            _settings = Ensure.IsNotNull(settings, NameOf(settings))
            _support = Ensure.IsNotNull(support, NameOf(support))
        End Sub

        Public Overrides Property Source As String = ThirdParties.EXPEDIARAPID

        Public Overrides ReadOnly Property SqlRequest As Boolean = False

        Public Shared Function BuildDefaultRequest(
                url As String,
                requestMethod As eRequestMethod,
                headers As RequestHeaders,
                useGzip As Boolean,
                saveLogs As Boolean,
                userAgent As String,
                logFileName As String,
                Optional requestBody As String = Nothing) As Request

            Dim request As New Request() With {
                    .Accept = "application/json",
                    .ContentType = "application/json",
                    .UseGZip = useGzip,
                    .AuthenticationMode = eAuthenticationMode.None,
                    .Method = requestMethod,
                    .Headers = headers,
                    .UserAgent = userAgent,
                    .EndPoint = url,
                    .CreateLog = saveLogs,
                    .Source = ThirdParties.EXPEDIARAPID,
                    .LogFileName = logFileName,
                    .SuppressExpectHeaders = True
                }

            If Not String.IsNullOrWhiteSpace(requestBody) Then request.SetRequest(requestBody)

            Return request
        End Function

        Public Function BuildSearchRequest(tpKeys As IEnumerable(Of String),
                                           searchDetails As SearchDetails,
                                           savelogs As Boolean) As Request

            Dim searchURL As String = BuildSearchURL(tpKeys, searchDetails)
            Dim useGzip As Boolean = _settings.UseGZIP(searchDetails)
            Dim apiKey As String = _settings.ApiKey(searchDetails)
            Dim secret As String = _settings.Secret(searchDetails)
            Dim userAgent As String = _settings.UserAgent(searchDetails)

            Dim tpSessionID As String = Guid.NewGuid().ToString()
            Dim headers As New RequestHeaders From
                    {
                        New RequestHeader(SearchHeaderKeys.CustomerSessionID, tpSessionID),
                        ExpediaRapidSearch.CreateAuthorizationHeader(apiKey, secret)
                    }

            Dim request As Request =
                BuildDefaultRequest(searchURL,
                                    eRequestMethod.GET,
                                    headers,
                                    useGzip,
                                    savelogs,
                                    userAgent,
                                    "Search")

            With request
                .ExtraInfo = New SearchExtraHelper() With {.SearchDetails = searchDetails}
            End With

            Return request
        End Function

        Public Overrides Function BuildSearchRequests(
                searchDetails As SearchDetails,
                resortSplits As List(Of ResortSplit),
                saveLogs As Boolean) As List(Of Request)

            Dim batchSize As Integer = _settings.SearchRequestBatchSize(searchDetails)
            Dim tpPropertyIDs As List(Of String) = resortSplits.SelectMany(Function(rs) rs.Hotels).Select(Function(h) h.TPKey).ToList()

            Return BatchExtension.Batch(tpPropertyIDs, batchSize).Select(Function(tpKeys) BuildSearchRequest(tpKeys, searchDetails, saveLogs)).ToList()
        End Function

        Public Shared Function BuildSearchURL(
                tpKeys As IEnumerable(Of String),
                settings As IExpediaRapidSettings,
                tpAttributeSearch As IThirdPartyAttributeSearch,
                arrivalDate As Date,
                departureDate As Date,
                currencyCode As String,
                occupancies As IEnumerable(Of ExpediaRapidOccupancy)) As String

            Dim nvc As New NameValueCollection From {
                {SearchQueryKeys.CheckIn, arrivalDate.ToString("yyyy-MM-dd")},
                {SearchQueryKeys.CheckOut, departureDate.ToString("yyyy-MM-dd")},
                {SearchQueryKeys.Currency, currencyCode},
                {SearchQueryKeys.Language, settings.LanguageCode(tpAttributeSearch)},
                {SearchQueryKeys.CountryCode, settings.CountryCode(tpAttributeSearch)},
                {SearchQueryKeys.SalesChannel, settings.SalesChannel(tpAttributeSearch)},
                {SearchQueryKeys.SalesEnvironment, settings.SalesEnvironment(tpAttributeSearch)},
                {SearchQueryKeys.SortType, settings.SortType(tpAttributeSearch)},
                {SearchQueryKeys.RatePlanCount, settings.RatePlanCount(tpAttributeSearch).ToString()},
                {SearchQueryKeys.PaymentTerms, settings.PaymentTerms(tpAttributeSearch)},
                {SearchQueryKeys.PartnerPointOfSale, settings.PartnerPointOfSale(tpAttributeSearch)},
                {SearchQueryKeys.BillingTerms, settings.BillingTerms(tpAttributeSearch)},
                {SearchQueryKeys.RateOption, settings.RateOption(tpAttributeSearch)}
            }

            If Not String.IsNullOrWhiteSpace(settings.PlatformName(tpAttributeSearch)) Then
                nvc.Add(SearchQueryKeys.PlatformName, settings.PlatformName(tpAttributeSearch))
            End If

            For Each occupancy As ExpediaRapidOccupancy In occupancies
                nvc.Add(SearchQueryKeys.Occupancy, occupancy.GetExpediaRapidOccupancy())
            Next

            For Each tpKey As String In tpKeys
                nvc.Add(SearchQueryKeys.PropertyID, tpKey)
            Next

            Dim searchUrl As New UriBuilder(settings.Scheme(tpAttributeSearch),
                                            settings.Host(tpAttributeSearch),
                                            -1, ' to not put the port number in the URL
                                            settings.SearchPath(tpAttributeSearch))

            Return searchUrl.Uri.AbsoluteUri + AddQueryParams(nvc)
        End Function

        Public Overrides Function TransformResponse(
                requests As List(Of Request),
                searchDetails As SearchDetails,
                resortSplits As List(Of ResortSplit)) As TransformedResultCollection

            Dim transformedResults As New TransformedResultCollection
            Dim allAvailabilities As New List(Of PropertyAvailablility)

            Dim tpSessionID As String = requests.First.HTTPWebResponse.Headers("Transaction-Id")

            For Each request As Request In requests
                Dim response As New SearchResponse
                Dim success As Boolean = response.IsValid(request.ResponseString,
                                                          CInt(request.HTTPWebResponse.StatusCode))


                If success Then
                    response = JsonConvert.DeserializeObject(Of SearchResponse)(request.ResponseString)
                    allAvailabilities.AddRange(response)
                End If
            Next

            transformedResults.TransformedResults.AddRange(allAvailabilities.SelectMany(Function(pa) GetResultFromPropertyAvailability(searchDetails, pa, tpSessionID)))

            Return transformedResults

        End Function

        Public Overrides Function SearchRestrictions(searchDetails As SearchDetails) As Boolean

            Return searchDetails.Rooms > 8
        End Function

        Public Overrides Function ResponseHasExceptions(request As Request) As Boolean

            Dim searchResponse As New SearchResponse

            Return Not request.Success OrElse
                   Not searchResponse.IsValid(request.ResponseString, CInt(request.HTTPWebResponse.StatusCode))
        End Function

        Friend Shared Function AddQueryParams(params As NameValueCollection) As String

            Dim queryString As IEnumerable(Of String) =
                From key In params.AllKeys
                From value In params.GetValues(key)
                Select String.Format("{0}={1}",
                    HttpUtility.UrlEncode(key),
                    HttpUtility.UrlEncode(value))

            Return "?" + String.Join("&", queryString)
        End Function

        Private Function BuildSearchURL(
                tpKeys As IEnumerable(Of String),
                searchDetails As SearchDetails) As String

            Dim arrivalDate As Date = searchDetails.ArrivalDate
            Dim departureDate As Date = searchDetails.DepartureDate

            Dim currencyCode As String = _support.TPCurrencyLookup(Me.Source, searchDetails.CurrencyCode)

            Dim occupancies As IEnumerable(Of ExpediaRapidOccupancy) =
                searchDetails.RoomDetails.Select(Function(r) New ExpediaRapidOccupancy(r.Adults, r.ChildAges, r.Infants))

            Return BuildSearchURL(tpKeys, _settings, searchDetails, arrivalDate, departureDate, currencyCode, occupancies)
        End Function

        Private Function GetResultFromPropertyAvailability(
                searchDetails As SearchDetails,
                propertyAvailability As PropertyAvailablility,
                tpSessionID As String) As IEnumerable(Of TransformedResult)

            If searchDetails.Rooms > 1 Then
                Dim cheapestRoom As SearchResponseRoom =
                        propertyAvailability.Rooms _
                            .OrderBy(Function(room) room.Rates.Min(Function(rate) GetTotalInclusiveRate(rate.OccupancyRoomRates))) _
                            .First()

                propertyAvailability.Rooms.RemoveAll(Function(r) r.RoomID <> cheapestRoom.RoomID)
            End If

            Return propertyAvailability.Rooms.SelectMany(Function(room)
                                                             Return BuildResultFromRoom(searchDetails, propertyAvailability.PropertyID, room, tpSessionID)
                                                         End Function)
        End Function

        Private Function GetTotalInclusiveRate(
                occupancyRoomRates As Dictionary(Of String, OccupancyRoomRate)) As Decimal

            Return occupancyRoomRates.Sum(Function(orr) orr.Value.OccupancyRateTotals("inclusive").TotalInRequestCurrency.Amount)
        End Function

        Private Function BuildResultFromBedGroups(
                searchDetails As SearchDetails,
                tpKey As String,
                room As SearchResponseRoom,
                rate As RoomRate,
                bedGroup As BedGroupAvailability,
                tpSessionID As String) As IEnumerable(Of TransformedResult)

            Dim occupancies As IEnumerable(Of ExpediaRapidOccupancy) = searchDetails.RoomDetails.Select(Function(r) New ExpediaRapidOccupancy(r.Adults, r.ChildAges, r.Infants))

            Return occupancies.Select(Function(occupancy, i) BuildResultFromOccupancy(tpKey, room, rate, bedGroup, occupancy, i + 1, tpSessionID))
        End Function

        Private Function BuildResultFromOccupancy(
                tpKey As String,
                room As SearchResponseRoom,
                rate As RoomRate,
                bedGroup As BedGroupAvailability,
                occupancy As ExpediaRapidOccupancy,
                propertyRoomBookingID As Integer,
                tpSessionID As String) As TransformedResult

            Dim occupancyRoomRate As OccupancyRoomRate = rate.OccupancyRoomRates(occupancy.GetExpediaRapidOccupancy())

            Dim inclusiveRate As OccupancyRateAmount =
                occupancyRoomRate.OccupancyRateTotals("inclusive").TotalInRequestCurrency

            Dim baseRate As Decimal = GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.BaseRate)

            Dim taxes As List(Of Tax) = GetTaxes(occupancyRoomRate)

            Dim adjustments As List(Of TransformedResultAdjustment) = GetAdjustments(taxes)

            Dim totalTax As Decimal = taxes.Sum(Function(t) t.TaxAmount)

            Dim lookupMealBasisCodes As HashSet(Of String) =
                _support.TPMealBases(Me.Source).Keys.ToHashSet()

            Dim mealBasisCode As String = GetMealBasisCode(rate.Amenities.Keys,
                                                           lookupMealBasisCodes)

            Dim specialOffers As List(Of String) = GetSpecialOffers(rate,
                                                                    lookupMealBasisCodes)

            Dim result As New TransformedResult



            With result
                If occupancyRoomRate Is Nothing Then
                    .Warnings.Add("No room rate found for this property.")
                Else
                    .MasterID = tpKey.ToSafeInt()
                    .TPKey = tpKey
                    .CurrencyCode = inclusiveRate.CurrencyCode
                    .PropertyRoomBookingID = propertyRoomBookingID
                    .RoomType = $"{room.RoomName}, {bedGroup.Description}"
                    .RoomTypeCode = $"{room.RoomID}|{rate.RateID}|{bedGroup.BedGroupID}"
                    .MealBasisCode = mealBasisCode
                    .Adults = occupancy.Adults
                    .ChildAges = occupancy.ChildAges
                    .Children = occupancy.ChildAges.Where(Function(i) i > 0).Count
                    .Infants = occupancy.ChildAges.Where(Function(i) i = 0).Count
                    .Amount = inclusiveRate.Amount
                    .RegionalTax = totalTax.ToString()
                    .SpecialOffer = String.Join(", ", specialOffers)
                    .Discount = baseRate + totalTax - inclusiveRate.Amount
                    .NonRefundableRates = Not rate.IsRefundable
                    .AvailableRooms = rate.AvailableRooms
                    .Adjustments = adjustments
                    .PayLocalRequired = rate.MerchantOfRecord = "property"
                    .TPReference = $"{tpSessionID}|{bedGroup.Links.PriceCheckLink.HRef}"
                End If
            End With

            result.Validate()

            Return result
        End Function

        Private Function BuildResultFromRoom(
                searchDetails As SearchDetails,
                tpKey As String,
                room As SearchResponseRoom,
                tpSessionID As String) As IEnumerable(Of TransformedResult)

            If searchDetails.Rooms > 1 Then
                Dim cheapestRate As RoomRate =
                        room.Rates _
                            .MinBy(Function(rate) GetTotalInclusiveRate(rate.OccupancyRoomRates)) _
                            .First()

                room.Rates.RemoveAll(Function(r) r.RateID <> cheapestRate.RateID)
            End If

            Return room.Rates _
                        .SelectMany(Function(rate) BuildResultFromRoomRates(searchDetails, tpKey, room, rate, tpSessionID))
        End Function

        Private Function BuildResultFromRoomRates(
                searchDetails As SearchDetails,
                tpKey As String,
                room As SearchResponseRoom,
                rate As RoomRate,
                tpSessionID As String) As IEnumerable(Of TransformedResult)

            Return rate.BedGroupAvailabilities.Values _
                .SelectMany(Function(bedGroup) BuildResultFromBedGroups(searchDetails, tpKey, room, rate, bedGroup, tpSessionID))
        End Function

        Private Function GetAdjustments(
                taxes As List(Of Tax)) As List(Of TransformedResultAdjustment)

            Return taxes.Select(Function(t) New TransformedResultAdjustment With
                {
                    .AdjustmentType = "T",
                    .PayLocal = True,
                    .AdjustmentName = t.TaxName,
                    .AdjustmentAmount = t.TaxAmount
                }).ToList()
        End Function

        Private Function GetTaxes(
                occupancyRoomRate As OccupancyRoomRate) As List(Of Tax)

            Dim salesTax As Decimal = GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.SalesTax)
            Dim taxAndServiceFee As Decimal = GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.TaxAndServiceFee)
            Dim recoverChargesAndFees As Decimal = GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.RecoveryChargesAndFees)

            If occupancyRoomRate.StayRates.Any() Then
                salesTax += GetStayRateFromType(occupancyRoomRate, RateTypes.SalesTax)
                taxAndServiceFee += GetStayRateFromType(occupancyRoomRate, RateTypes.TaxAndServiceFee)
                recoverChargesAndFees += GetStayRateFromType(occupancyRoomRate, RateTypes.RecoveryChargesAndFees)
            End If

            Dim taxes As New List(Of Tax)
            With taxes
                If salesTax > 0 Then .Add(New Tax("Sales Tax", salesTax))
                If taxAndServiceFee > 0 Then .Add(New Tax("Tax And Service Fee", taxAndServiceFee))
                If recoverChargesAndFees > 0 Then .Add(New Tax("Tax Recovery Charges", recoverChargesAndFees))
            End With

            Return taxes
        End Function

        Public Shared Function GetTotalNightlyRateFromType(
                occupancyRoomRate As OccupancyRoomRate,
                rateType As String) As Decimal
            Return occupancyRoomRate.NightlyRates _
                    .SelectMany(Function(nr) nr _
                        .Where(Function(amt) amt.RateType = rateType)) _
                    .Sum(Function(amt) amt.Amount)
        End Function

        Public Shared Function GetStayRateFromType(
                occupancyRoomRate As OccupancyRoomRate,
                rateType As String) As Decimal

            Dim stayRates As List(Of Rate) = occupancyRoomRate.StayRates

            Dim matchedRate As IEnumerable(Of Rate) = stayRates.Where(Function(r) r.RateType = rateType)

            Return If(matchedRate.Any(), matchedRate.First().Amount, 0)
        End Function

        Private Function GetSpecialOffers(
                rate As RoomRate,
                lookupMealBasisCodes As HashSet(Of String)) As List(Of String)

            Dim specialOffers As New List(Of String)

            Dim nonMealBasisAmenities As IEnumerable(Of String) =
                rate.Amenities.Where(Function(a) Not lookupMealBasisCodes.Contains(a.Key)) _
                                .Select(Function(a) a.Value.Name).ToList()
            specialOffers.AddRange(nonMealBasisAmenities)

            If rate.Promotions IsNot Nothing Then

                If rate.Promotions.ValueAdds IsNot Nothing Then
                    Dim valueAdds As IEnumerable(Of String) =
                    rate.Promotions.ValueAdds.Select(Function(vaKvp) vaKvp.Value.Description).ToList()

                    specialOffers.AddRange(valueAdds)
                End If

                If rate.Promotions.Deal IsNot Nothing Then
                    specialOffers.Add(rate.Promotions.Deal.Description)
                End If

            End If

            Return specialOffers
        End Function

        Friend Function GetMealBasisCode(
                amenities As IEnumerable(Of String),
                tpValidMealbasisCodes As HashSet(Of String)) As String

            Dim matchedMealBasisCodes As IEnumerable(Of String) =
                amenities.Where(Function(a) tpValidMealbasisCodes.Contains(a))

            Return If(matchedMealBasisCodes.Any(), matchedMealBasisCodes.First(), "RO")
        End Function

        Public Shared Function CreateAuthorizationHeader(
                apiKey As String,
                secret As String) As RequestHeader

            Dim timeStamp As Double = DateTimeOffset.UtcNow.ToUnixTimeSeconds()

            Dim data As Byte() =
                Encoding.UTF8.GetBytes($"{apiKey}{secret}{timeStamp}")

            Dim hashString As String

            Using sha As SHA512 = New SHA512Managed()
                hashString = BitConverter.ToString(sha.ComputeHash(data)).Replace("-", "").ToLower()
            End Using

            Return New RequestHeader(SearchHeaderKeys.Authorization,
                                                 $"EAN apikey={apiKey},signature={hashString},timestamp={timeStamp}")
        End Function

        Private Class Tax
            Public Sub New(name As String, amount As Decimal)
                Me.TaxName = name
                Me.TaxAmount = amount
            End Sub
            Public Property TaxName As String
            Public Property TaxAmount As Decimal
        End Class

        Private Class QueryParamter
            Public Property Name As String
            Public Property Value As String
        End Class

    End Class

End Namespace