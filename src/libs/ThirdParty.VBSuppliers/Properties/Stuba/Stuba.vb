Imports System.Xml
Imports Intuitive.Net.WebRequests
Imports Intuitive.Functions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.Abstractions.Models
Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports Intuitive
Imports System.Net.Http

Public Class Stuba
    Implements IThirdParty

    Private ReadOnly _settings As IStubaSettings

    Private ReadOnly _support As ITPSupport

    Private ReadOnly _httpClient As HttpClient

    Public Sub New(settings As IStubaSettings, support As ITPSupport, httpClient As HttpClient)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
        _httpClient = Ensure.IsNotNull(httpClient, NameOf(httpClient))
    End Sub

    Public Function PreBook(oPropertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook
        Dim bReturn As Boolean

        Try
            Dim oXML As XmlDocument = SendRequest("Pre-Book", BookReservationRequest(oPropertyDetails, False), oPropertyDetails)
            ExtractErrata(oXML, oPropertyDetails)
            bReturn = CostsAndCancellation(oPropertyDetails, oXML)
        Catch ex As Exception
            oPropertyDetails.Warnings.AddNew("PreBookException", ex.ToString)
            bReturn = False
        End Try
        Return bReturn
    End Function

    Public Function Book(oPropertyDetails As PropertyDetails) As String Implements IThirdParty.Book
        Dim sReference As String = ""

        Try
            Dim oXML As XmlDocument = SendRequest("Book", BookReservationRequest(oPropertyDetails, True), oPropertyDetails)

            If oXML.SelectSingleNode("BookingCreateResult/Booking/HotelBooking/Status").InnerText.ToLower = "confirmed" Then
                sReference = oXML.SelectSingleNode("BookingCreateResult/Booking/Id").InnerText
            End If
        Catch ex As Exception
            oPropertyDetails.Warnings.AddNew("BookException", ex.ToString)
            sReference = "Failed"
        End Try
        Return sReference
    End Function

    Public Function CancelBooking(oPropertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking
        Dim oThirdPartyCancellationResponse As New ThirdPartyCancellationResponse

        Try
            Dim oXML As XmlDocument = SendRequest("Cancel", CancelRequest(oPropertyDetails, True), oPropertyDetails)

            If oXML.SelectSingleNode("BookingCancelResult/Booking/HotelBooking/Status").InnerText.ToLower = "cancelled" Then
                oThirdPartyCancellationResponse.TPCancellationReference = oXML.SelectSingleNode("BookingCancelResult/Booking/HotelBooking/Id").InnerText
                oThirdPartyCancellationResponse.Success = True
            End If

        Catch ex As Exception
            oPropertyDetails.Warnings.AddNew("CancelException", ex.ToString)
            oThirdPartyCancellationResponse.TPCancellationReference = "Failed"
        End Try
        Return oThirdPartyCancellationResponse
    End Function

    Public Function GetCancellationCost(oPropertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost
        Dim oThirdPartyCancellationFeeResult As New ThirdPartyCancellationFeeResult

        Try
            Dim oXML As XmlDocument = SendRequest("Cancellation Charge", CancelRequest(oPropertyDetails, False), oPropertyDetails)
            Dim nTotalCancellationFee As Decimal = 0

            For Each oRoom As RoomDetails In oPropertyDetails.Rooms
                Dim dLastDate As New Date(1900, 1, 1)
                Dim nRoomAmount As Decimal = 0
                Dim oRoomNode As XmlNode = oXML.SelectSingleNode(String.Format("BookingCancelResult/Booking/HotelBooking/Room[RoomType/@code = '{0}']", oRoom.RoomTypeCode))

                For Each oCXL As XmlNode In oRoomNode.SelectNodes("CanxFees/Fee")
                    Dim dStartDate As Date = oCXL.SelectSingleNode("@from").InnerText.ToSafeDate
                    If dStartDate < Date.Now AndAlso dStartDate > dLastDate Then
                        nRoomAmount = oCXL.SelectSingleNode("Amount/@amt").InnerText.ToSafeDecimal()
                    End If
                Next
                nTotalCancellationFee += nRoomAmount
            Next

            oThirdPartyCancellationFeeResult.Amount = nTotalCancellationFee
            oThirdPartyCancellationFeeResult.CurrencyCode = oPropertyDetails.CurrencyCode

        Catch ex As Exception
            oPropertyDetails.Warnings.AddNew("CancellationChargeException", ex.ToString)
        End Try

        Return oThirdPartyCancellationFeeResult
    End Function

    Private Function SendRequest(ByVal RequestType As String, XML As String, ByVal PropertyDetails As PropertyDetails) As XmlDocument

        Dim oRequest As New Request
        With oRequest
            .EndPoint = _settings.URL(PropertyDetails)
            .SetRequest(XML)
            .Method = eRequestMethod.POST
            .Source = ThirdParties.STUBA
            .LogFileName = RequestType
            .CreateLog = True
            .UseGZip = True
            .Send(_httpClient)
        End With

        If oRequest.RequestLog <> "" Then
            PropertyDetails.Logs.AddNew(ThirdParties.STUBA, "Stuba " & RequestType & " Request", oRequest.RequestLog)
        End If

        If oRequest.ResponseLog <> "" Then
            PropertyDetails.Logs.AddNew(ThirdParties.STUBA, "Stuba " & RequestType & " Response", oRequest.ResponseLog)
        End If

        Return oRequest.ResponseXML
    End Function

    Private Function BookReservationRequest(ByVal oPropertyDetails As PropertyDetails, ByVal Confirm As Boolean) As String

        Dim sOrg As String = _settings.Organisation(oPropertyDetails)
        Dim sUser As String = _settings.Username(oPropertyDetails)
        Dim sPassword As String = _settings.Password(oPropertyDetails)
        Dim sVersion As String = _settings.Version(oPropertyDetails)
        Dim sCurrencyCode As String = _settings.Currency(oPropertyDetails)

        EnsurePassengerNamesNotNull(oPropertyDetails)
        Dim oRequest As XElement =
            <BookingCreate>
                <Authority>
                    <Org><%= sOrg %></Org>
                    <User><%= sUser %></User>
                    <Password><%= sPassword %></Password>
                    <Currency><%= sCurrencyCode %></Currency>
                    <Version><%= sVersion %></Version>
                </Authority>
                <HotelBooking>
                    <QuoteId><%= oPropertyDetails.Rooms.First().ThirdPartyReference.Split("|"c)(1) %></QuoteId>
                    <HotelStayDetails>
                        <Nationality><%= GetNationality(oPropertyDetails) %></Nationality>
                        <%=
                            From oRoom As RoomDetails In oPropertyDetails.Rooms
                            Select <Room>
                                       <Guests>
                                           <%=
                                               From oGuest As Passenger In oRoom.Passengers.Where(Function(p) p.PassengerType = PassengerType.Adult)
                                               Select <Adult title=<%= oGuest.Title %> first=<%= oGuest.FirstName %> last=<%= oGuest.LastName %>/>
                                           %>
                                           <%=
                                               From oGuest As Passenger In oRoom.Passengers.Where(Function(p) p.PassengerType <> PassengerType.Adult)
                                               Select <Child title=<%= oGuest.Title %> first=<%= oGuest.FirstName %> last=<%= oGuest.LastName %> age=<%= oGuest.Age %>/>
                                           %>
                                       </Guests>
                                   </Room>
                        %>
                    </HotelStayDetails>
                </HotelBooking>
                <CommitLevel><%= If(Confirm, "confirm", "prepare") %></CommitLevel>
            </BookingCreate>

        Return oRequest.ToString
    End Function

    Private Function CancelRequest(ByVal oPropertyDetails As PropertyDetails, ByVal bConfirm As Boolean) As String
        Dim sOrg As String = _settings.Organisation(oPropertyDetails)
        Dim sUser As String = _settings.Username(oPropertyDetails)
        Dim sPassword As String = _settings.Password(oPropertyDetails)
        Dim sVersion As String = _settings.Version(oPropertyDetails)
        Dim sCurrencyCode As String = _settings.Currency(oPropertyDetails)

        Dim oRequest As XElement =
            <BookingCancel>
                <Authority>
                    <Org><%= sOrg %></Org>
                    <User><%= sUser %></User>
                    <Password><%= sPassword %></Password>
                    <Currency><%= sCurrencyCode %></Currency>
                    <Version><%= sVersion %></Version>
                </Authority>
                <BookingId><%= oPropertyDetails.SourceReference %></BookingId>
                <CommitLevel><%= If(bConfirm, "confirm", "prepare") %></CommitLevel>
            </BookingCancel>

        Return oRequest.ToString()
    End Function

    Private Sub EnsurePassengerNamesNotNull(propertyDetails As PropertyDetails)
        Dim rand As New Random
        Dim charAsInt As Integer = Char.ConvertToUtf32("A", 0)
        For Each guest As Passenger In propertyDetails.Rooms.SelectMany(Function(room) room.Passengers)
            If String.IsNullOrEmpty(guest.Title) Then
#Disable Warning SCS0005 ' Weak random generator
                guest.Title = If(rand.Next(2) = 0, "Ms", "Mr")
#Enable Warning SCS0005 ' Weak random generator
            End If
            If String.IsNullOrEmpty(guest.FirstName) Then
                guest.FirstName = Char.ConvertFromUtf32(charAsInt)
                charAsInt += 1 'names need to be unique
            End If
            If String.IsNullOrEmpty(guest.LastName) Then
                guest.LastName = "Test"
            End If
        Next
    End Sub

    Private Function GetNationality(propertyDetails As PropertyDetails) As String
        Dim sNationality As String = ""
        If propertyDetails.LeadGuestNationalityID <> 0 Then
            sNationality = _support.TPNationalityLookup(ThirdParties.STUBA, propertyDetails.LeadGuestNationalityID)
        End If
        If sNationality = "" Then
            sNationality = _settings.Nationality(propertyDetails)
        End If
        Return sNationality
    End Function

    Private Function CostsAndCancellation(ByVal oPropertyDetails As PropertyDetails, ByVal oXML As XmlDocument) As Boolean
        Dim bAvailable As Boolean = False

        For Each oRoom As RoomDetails In oPropertyDetails.Rooms

            'Check costs
            Dim oRoomNode As XmlNode = oXML.SelectSingleNode(String.Format("BookingCreateResult/Booking/HotelBooking/Room[RoomType/@code = '{0}']", oRoom.RoomTypeCode))
            oRoom.LocalCost = oRoomNode.SelectSingleNode("TotalSellingPrice/@amt").InnerText.ToSafeDecimal()
            oRoom.GrossCost = oRoomNode.SelectSingleNode("TotalSellingPrice/@amt").InnerText.ToSafeDecimal()

            'Cancellation charges
            For Each oCXL As XmlNode In oRoomNode.SelectNodes("CanxFees/Fee")
                Dim dStartDate As Date

                If oCXL.SelectSingleNode("@from") Is Nothing Then
                    dStartDate = Date.Now.Date
                Else
                    dStartDate = oCXL.SelectSingleNode("@from").InnerText.ToSafeDate
                End If

                Dim dEndDate As New Date(2099, 12, 25)

                Dim oNextStartDate As XmlNode = oCXL.NextSibling
                If oNextStartDate IsNot Nothing Then
                    dEndDate = oNextStartDate.SelectSingleNode("@from").InnerText.ToSafeDate.AddDays(-1)
                End If

                Dim nAmount As Decimal = oCXL.SelectSingleNode("Amount/@amt").InnerText.ToSafeDecimal()
                oPropertyDetails.Cancellations.AddNew(dStartDate, dEndDate, nAmount)
            Next

        Next

        oPropertyDetails.LocalCost = oPropertyDetails.Rooms.Sum(Function(r) r.LocalCost)

        oPropertyDetails.Cancellations.Solidify(SolidifyType.Sum)
        bAvailable = (oXML.SelectNodes("BookingCreateResult/Booking/HotelBooking/Room").Count = oPropertyDetails.Rooms.Count)

        Return bAvailable
    End Function

    Private Sub ExtractErrata(oResponse As XmlDocument, oPropertyDetails As PropertyDetails)
        Dim oErrataNodes As XmlNodeList = oResponse.SelectNodes("BookingCreateResult/Booking/HotelBooking/Room/Messages/Message[Type = 'Supplier Notes']/Text")
        If oErrataNodes?.Count > 0 Then
            For Each oErratum As XmlNode In oErrataNodes
                oPropertyDetails.Errata.AddNew("Important Informations", oErratum.InnerText)
            Next
        End If
    End Sub

    Private Function IThirdParty_SupportsLiveCancellation(searchDetails As IThirdPartyAttributeSearch, source As String) As Boolean Implements IThirdParty.SupportsLiveCancellation
        Return _settings.AllowCancellations(searchDetails)
    End Function

#Region "Stuff"

    Public ReadOnly Property SupportsRemarks As Boolean Implements IThirdParty.SupportsRemarks
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property SupportsBookingSearch As Boolean Implements IThirdParty.SupportsBookingSearch
        Get
            Return False
        End Get
    End Property

    Private Function IThirdParty_TakeSavingFromCommissionMargin(searchDetails As IThirdPartyAttributeSearch) As Boolean Implements IThirdParty.TakeSavingFromCommissionMargin
        Return False
    End Function

    Private Function IThirdParty_OffsetCancellationDays(searchDetails As IThirdPartyAttributeSearch) As Integer Implements IThirdParty.OffsetCancellationDays
        Return _settings.OffsetCancellationDays(searchDetails, False)
    End Function

    Public Function RequiresVCard(info As VirtualCardInfo) As Boolean Implements IThirdParty.RequiresVCard
        Return False
    End Function

    Public Function BookingSearch(oBookingSearchDetails As BookingSearchDetails) As ThirdPartyBookingSearchResults Implements IThirdParty.BookingSearch
        Return New ThirdPartyBookingSearchResults
    End Function

    Public Function BookingStatusUpdate(oPropertyDetails As PropertyDetails) As ThirdPartyBookingStatusUpdateResult Implements IThirdParty.BookingStatusUpdate
        Return New ThirdPartyBookingStatusUpdateResult
    End Function

    Public Function CreateReconciliationReference(sInputReference As String) As String Implements IThirdParty.CreateReconciliationReference
        Return ""
    End Function

    Public Sub EndSession(ByVal oPropertyDetails As PropertyDetails) Implements IThirdParty.EndSession
    End Sub

#End Region

End Class