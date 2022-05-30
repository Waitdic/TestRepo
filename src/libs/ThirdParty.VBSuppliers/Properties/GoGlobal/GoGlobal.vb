Imports System.Xml
Imports Intuitive
Imports Intuitive.Net.WebRequests
Imports Intuitive.Functions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports System.Text.RegularExpressions
Imports Intuitive.Helpers.Extensions
Imports System.Net.Http

Public Class GoGlobal
    Implements IThirdParty

#Region "Constructor"

    Public Sub New(settings As IGoGlobalSettings, support As ITPSupport, httpClient As HttpClient)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
        _httpClient = Ensure.IsNotNull(httpClient, NameOf(httpClient))
    End Sub

#End Region

#Region "Properties"

    Private ReadOnly _settings As IGoGlobalSettings

    Private ReadOnly _support As ITPSupport

    Private ReadOnly _httpClient As HttpClient

    Public ReadOnly Property SupportsBookingSearch As Boolean Implements IThirdParty.SupportsBookingSearch
        Get
            Return True
        End Get
    End Property

    Private Function SupportsLiveCancellation(searchDetails As IThirdPartyAttributeSearch, source As String) As Boolean Implements IThirdParty.SupportsLiveCancellation
        Return _settings.AllowCancellations(searchDetails)
    End Function

    Public ReadOnly Property SupportsRemarks As Boolean Implements IThirdParty.SupportsRemarks
        Get
            Return True
        End Get
    End Property
    Private Function TakeSavingFromCommissionMargin(searchDetails As IThirdPartyAttributeSearch) As Boolean Implements IThirdParty.TakeSavingFromCommissionMargin
        Return False
    End Function

    Private Function OffsetCancellationDays(searchDetails As IThirdPartyAttributeSearch) As Integer Implements IThirdParty.OffsetCancellationDays
        Return _settings.OffsetCancellationDays(searchDetails, False)
    End Function

    Public Function RequiresVCard(info As VirtualCardInfo) As Boolean Implements IThirdParty.RequiresVCard
        Return False
    End Function

    Public ReadOnly Property PreBookRequestNumber As Integer
        Get
            Return 9
        End Get
    End Property
    Public ReadOnly Property BookRequestNumber As Integer
        Get
            Return 2
        End Get
    End Property
    Public ReadOnly Property StatusConfirmationRequestNumber As Integer
        Get
            Return 5
        End Get
    End Property
    Public ReadOnly Property CancelRequestNumber As Integer
        Get
            Return 3
        End Get
    End Property
    Public ReadOnly Property CancellationCostRequestNumber As Integer
        Get
            Return 4
        End Get
    End Property

#End Region

#Region "Prebook"

    Public Function PreBook(oPropertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook

        Dim bReturn As Boolean = True

        Try
            Dim xPreBook As XDocument = SendRequest("Pre-Book", PreBookRequestNumber, ValuationRequest(oPropertyDetails, _settings), oPropertyDetails, _settings)
            bReturn = CostsAndCancellation(oPropertyDetails, xPreBook)
            oPropertyDetails.LocalCost = oPropertyDetails.Rooms.Sum(Function(r) r.LocalCost)

        Catch ex As Exception

            oPropertyDetails.Logs.AddNew(ThirdParties.GOGLOBAL, "PreBook Exception", ex.ToString)
            bReturn = False

        End Try

        Return bReturn

    End Function

#End Region

#Region "Book"

    Public Function Book(oPropertyDetails As PropertyDetails) As String Implements IThirdParty.Book

        Dim sReference As String = ""

        Try
            Dim xBook As XDocument = SendRequest("Book", BookRequestNumber, BookReservationRequest(oPropertyDetails, _settings), oPropertyDetails, _settings)
            sReference = xBook.Document.Element("Main").Element("GoBookingCode").Value

            Dim xStatus As XDocument = SendRequest("Status Confirmation", StatusConfirmationRequestNumber,
                                                   StatusRequest(oPropertyDetails, sReference, _settings), oPropertyDetails, _settings)

            If Not xStatus.Document.Element("Main").Element("GoBookingCode").Attribute("Status").Value.InList("C", "VCH") Then
                oPropertyDetails.Logs.AddNew(ThirdParties.GOGLOBAL, "Third Party / Book Failed", "Booking unsuccessful")

                sReference = "Failed"
            End If

        Catch ex As Exception

            oPropertyDetails.Logs.AddNew(ThirdParties.GOGLOBAL, "Book Exception", ex.ToString)
            sReference = "Failed"

        End Try
        Return sReference
    End Function

#End Region

#Region "Cancel"

    Public Function CancelBooking(oPropertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking

        Dim oThirdPartyCancellationResponse As New ThirdPartyCancellationResponse

        Try
            Dim oXML As XDocument = SendRequest("Cancel", CancelRequestNumber, CancelRequest(oPropertyDetails, _settings), oPropertyDetails, _settings)

        Catch ex As Exception

            oPropertyDetails.Logs.AddNew(ThirdParties.GOGLOBAL, "Cancellation Exception", ex.ToString)
            oThirdPartyCancellationResponse.TPCancellationReference = "Failed"

        End Try

        Return oThirdPartyCancellationResponse

    End Function

    Public Function GetCancellationCost(oPropertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost

        Dim oThirdPartyCancellationFeeResult As New ThirdPartyCancellationFeeResult

        Try
            Dim oXML As XDocument = SendRequest("Cancellation Cost", CancellationCostRequestNumber, ViewReservationRequest(oPropertyDetails), oPropertyDetails, _settings)

        Catch ex As Exception

            oPropertyDetails.Logs.AddNew(ThirdParties.GOGLOBAL, "Cancellation Cost Exception", ex.ToString)
            oThirdPartyCancellationFeeResult.Success = False

        End Try

        Return oThirdPartyCancellationFeeResult

    End Function

#End Region

#Region "BookingSearch"

    Public Function BookingSearch(oBookingSearchDetails As BookingSearchDetails) As ThirdPartyBookingSearchResults Implements IThirdParty.BookingSearch

        Dim oThirdPartyBookingSearchResults As New ThirdPartyBookingSearchResults

        Return oThirdPartyBookingSearchResults

    End Function

#End Region

#Region "Other Methods"

    Public Function CreateReconciliationReference(sInputReference As String) As String Implements IThirdParty.CreateReconciliationReference
        Return ""
    End Function

#Region "Booking Status Update"

    Public Function BookingStatusUpdate(ByVal oPropertyDetails As PropertyDetails) As ThirdPartyBookingStatusUpdateResult Implements IThirdParty.BookingStatusUpdate
        Return New ThirdPartyBookingStatusUpdateResult
    End Function

#End Region

#End Region
    Public Sub EndSession(oPropertyDetails As PropertyDetails) Implements IThirdParty.EndSession

    End Sub



#Region "Helpers"

    Private Shared Function StripInvalidCharacters(ByVal Line As String) As String

        Return Regex.Replace(Line, "[^A-Za-z- ]", String.Empty).ToProperCase()

    End Function

    Private Function SendRequest(ByVal RequestType As String, ByVal RequestNumber As Integer, ByVal XML As String,
                                        ByVal PropertyDetails As PropertyDetails, ByVal Settings As IGoGlobalSettings) As XDocument

        Dim xsi As XNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance")
        Dim xsd As XNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema")
        Dim soap As XNamespace = XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/")
        Dim ns As XNamespace = XNamespace.Get("http://www.goglobal.travel/")

        Dim xRequest As XDocument = New XDocument(
          New XDeclaration("1.0", "utf-8", "no"),
          New XElement(soap + "Envelope",
           New XAttribute(XNamespace.Xmlns + "xsi", xsi),
           New XAttribute(XNamespace.Xmlns + "xsd", xsd),
           New XAttribute(XNamespace.Xmlns + "soap", soap),
           New XElement(soap + "Body",
         New XElement(ns + "MakeRequest",
          New XAttribute("xmlns", ns),
          New XElement(ns + "requestType", RequestNumber),
          New XElement(ns + "xmlRequest", New XCData(XML))))))

        Dim oRequest As New Request
        With oRequest
            .EndPoint = Settings.URL(PropertyDetails)
            .SetRequest(xRequest.ToString)
            .Method = eRequestMethod.POST
            .Source = ThirdParties.GOGLOBAL
            .LogFileName = RequestType
            .CreateLog = True
            .UseGZip = True
            .Send(_httpClient)
        End With

        If oRequest.RequestLog <> "" Then
            PropertyDetails.Logs.AddNew(ThirdParties.GOGLOBAL, "GoGlobal " & RequestType & " Request", oRequest.RequestLog)
        End If

        If oRequest.ResponseLog <> "" Then
            PropertyDetails.Logs.AddNew(ThirdParties.GOGLOBAL, "GoGlobal " & RequestType & " Response", oRequest.ResponseLog)
        End If

        Dim XResponse As XDocument = XDocument.Parse(oRequest.ResponseString)

        Dim xInfo As XDocument = XDocument.Parse(XResponse.Document.Element(soap + "Envelope").Element(soap + "Body").Element(ns + "MakeRequestResponse").Element(ns + "MakeRequestResult").Value)
        Dim xHotel As XElement = xInfo.Document.Element("Root").Element("Main")

        Return XDocument.Parse(xHotel.ToString)

    End Function

    Private Shared Function ValuationRequest(ByVal PropertyDetails As PropertyDetails, ByVal Settings As IGoGlobalSettings) As String

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Dim xRequest As XDocument = New XDocument(New XElement("Root",
                        New XElement("Header",
                         New XElement("Agency", Settings.Agency(PropertyDetails)),
                         New XElement("User", Settings.User(PropertyDetails)),
                         New XElement("Password", Settings.Password(PropertyDetails)),
                         New XElement("Operation", "BOOKING_VALUATION_REQUEST"),
                         New XElement("OperationType", "Request")
                         ),
                      New XElement("Main",
                          New XElement("HotelSearchCode", PropertyDetails.Rooms(0).ThirdPartyReference.Split("|"c)(0)),
                          New XElement("ArrivalDate", PropertyDetails.ArrivalDate.ToString("yyyy-MM-dd")))))

        Return xRequest.ToString

    End Function

    Private Shared Function BookReservationRequest(ByVal PropertyDetails As PropertyDetails, ByVal Settings As IGoGlobalSettings) As String

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID
        Dim sGuestID As Integer = 1
        For Each oPassenger As Passenger In PropertyDetails.Rooms(0).Passengers
            oPassenger.TPID = sGuestID.ToString()
            sGuestID += 1
        Next

        Dim xRequest As XDocument = New XDocument(New XElement("Root",
                        New XElement("Header",
                         New XElement("Agency", Settings.Agency(PropertyDetails)),
                         New XElement("User", Settings.User(PropertyDetails)),
                         New XElement("Password", Settings.Password(PropertyDetails)),
                         New XElement("Operation", "BOOKING_INSERT_REQUEST"),
                         New XElement("OperationType", "Request")
                         ),
                      New XElement("Main",
                          New XElement("AgentReference", PropertyDetails.BookingReference),
                          New XElement("HotelSearchCode", PropertyDetails.Rooms(0).ThirdPartyReference.Split("|"c)(0)),
                          New XElement("ArrivalDate", PropertyDetails.ArrivalDate.ToString("yyyy-MM-dd")),
                          New XElement("Nights", PropertyDetails.Duration),
                          New XElement("NoAlternativeHotel", 1),
                          New XElement("Leader",
                              New XAttribute("LeaderPersonID", 1)),
                          New XElement("Rooms",
                              From oRoom In PropertyDetails.Rooms
                              Select New XElement("RoomType",
                                        New XAttribute("Adults", oRoom.Adults + oRoom.ChildAges.Where(Function(i) i > 10).Count),
                                       If(oRoom.Infants > 0, New XAttribute("Cots", oRoom.Infants), Nothing),
                                       New XElement("Room",
                                       New XAttribute("RoomID", -1 * oRoom.PropertyRoomBookingID),
                                       From oGuest In oRoom.Passengers.Where(Function(o) o.PassengerType.ToString <> "Infant")
                                       Select New XElement(If(oGuest.PassengerType.ToString = "Adult" OrElse oGuest.Age > 10, "PersonName", "ExtraBed"),
                                            StripInvalidCharacters(String.Format("{0} {1} {2}", oGuest.FirstName, oGuest.LastName, oGuest.Title)),
                                            New XAttribute("PersonID", oGuest.TPID),
                                            If(oGuest.PassengerType.ToString = "Child" AndAlso oGuest.Age < 11, New XAttribute("ChildAge", oGuest.Age), Nothing))))))))

        Return xRequest.ToString

    End Function

    Private Function ViewReservationRequest(ByVal PropertyDetails As PropertyDetails) As String

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Dim xRequest As XDocument = New XDocument(New XElement("Root",
                        New XElement("Header",
                         New XElement("Agency", _settings.Agency(PropertyDetails)),
                         New XElement("User", _settings.User(PropertyDetails)),
                         New XElement("Password", _settings.Password(PropertyDetails)),
                         New XElement("Operation", "BOOKING_SEARCH_REQUEST"),
                         New XElement("OperationType", "Request")
                         ),
                      New XElement("Main",
                          New XElement("GoBookingCode", PropertyDetails.SourceReference))))

        Return xRequest.ToString

    End Function

    Private Shared Function VoucherIssueRequest(ByVal PropertyDetails As PropertyDetails, ByVal sSourceReference As String, ByVal Settings As IGoGlobalSettings) As String

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Dim xRequest As XDocument = New XDocument(New XElement("Root",
                        New XElement("Header",
                         New XElement("Agency", Settings.Agency(PropertyDetails)),
                         New XElement("User", Settings.User(PropertyDetails)),
                         New XElement("Password", Settings.Password(PropertyDetails)),
                         New XElement("Operation", "VOUCHER_DETAILS_REQUEST"),
                         New XElement("OperationType", "Request")
                         ),
                      New XElement("Main",
                          New XElement("GoBookingCode", sSourceReference))))

        Return xRequest.ToString

    End Function

    Private Shared Function StatusRequest(ByVal PropertyDetails As PropertyDetails, ByVal sSourceReference As String, ByVal Settings As IGoGlobalSettings) As String

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Dim xRequest As XDocument = New XDocument(New XElement("Root",
                        New XElement("Header",
                         New XElement("Agency", Settings.Agency(PropertyDetails)),
                         New XElement("User", Settings.User(PropertyDetails)),
                         New XElement("Password", Settings.Password(PropertyDetails)),
                         New XElement("Operation", "BOOKING_STATUS_REQUEST"),
                         New XElement("OperationType", "Request")
                         ),
                      New XElement("Main",
                          New XElement("GoBookingCode", sSourceReference))))

        Return xRequest.ToString

    End Function

    Private Shared Function CancelRequest(ByVal PropertyDetails As PropertyDetails, ByVal Settings As IGoGlobalSettings) As String

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Dim xRequest As XDocument = New XDocument(New XElement("Root",
                        New XElement("Header",
                         New XElement("Agency", Settings.Agency(PropertyDetails)),
                         New XElement("User", Settings.User(PropertyDetails)),
                         New XElement("Password", Settings.Password(PropertyDetails)),
                         New XElement("Operation", "BOOKING_CANCEL_REQUEST"),
                         New XElement("OperationType", "Request")
                         ),
                      New XElement("Main",
                          New XElement("GoBookingCode", PropertyDetails.SourceReference))))

        Return xRequest.ToString

    End Function

    Private Shared Function CostsAndCancellation(ByVal PropertyDetails As PropertyDetails, ByVal xDoc As XDocument) As Boolean

        Dim bAvailable As Boolean = True
        Dim oCancellationsList As New Cancellations
        Dim oCancellation As New Cancellation

        Try

            'Have added the remarks recieved at search into the TPReference
            For Each oRoom As RoomDetails In PropertyDetails.Rooms
                If oRoom.ThirdPartyReference.Split("|"c).Count > 2 AndAlso oRoom.ThirdPartyReference.Split("|"c)(2) <> "" Then
                    PropertyDetails.Errata.AddNew("", oRoom.ThirdPartyReference.Split("|"c)(2))
                End If
            Next

            'Get the cost
            Dim xRates As XElement = xDoc.Document.Element("Main").Element("Rates")
            Dim nRates As Decimal = PropertyDetails.LocalCost
            If xRates IsNot Nothing AndAlso xRates.Value <> "" Then
                nRates = Regex.Match(xRates.Value, "\d+(?:.\d+)?").ToString.ToSafeDecimal()

                If nRates > 0 AndAlso nRates <> PropertyDetails.Rooms(0).LocalCost Then
                    For Each oRoom As RoomDetails In PropertyDetails.Rooms
                        oRoom.LocalCost = nRates / PropertyDetails.Rooms.Count
                        oRoom.GrossCost = nRates / PropertyDetails.Rooms.Count
                    Next
                End If
            End If

            oCancellation.Amount = nRates
            Dim sDeadline As String = xDoc.Document.Element("Main").Element("CancellationDeadline").Value

            'Now cancellations/errata
            Dim xRemarks As XElement = xDoc.Document.Element("Main").Element("Remarks")
            If xRemarks IsNot Nothing AndAlso xRemarks.Value <> "" Then
                Dim sErrata As String = xRemarks.Value

                'Check for cancelation stuff in errata
                Dim rMatch As Match = Regex.Match(sErrata, "(?:STARTING )([0-9]+[- /.][0-9][0-9][- /.][0-9]+)(?: CXL-PENALTY FEE IS )(?:PRICE OF )?([0-9.]+)(?:%| NIGHTS)")
                If rMatch IsNot Nothing AndAlso rMatch.Success Then

                    sDeadline = rMatch.Groups(1).ToString
                    Dim nValue As Decimal = rMatch.Groups(2).ToString.ToSafeDecimal()

                    'First try percentage
                    If rMatch.ToString.Contains("%") Then
                        oCancellation.Amount = nRates * (nValue / 100).ToSafeMoney()
                    Else
                        oCancellation.Amount = nRates * (nValue / PropertyDetails.Duration).ToSafeMoney()
                    End If

                End If

                PropertyDetails.Errata.AddNew("", sErrata)

            End If

            oCancellation.StartDate = sDeadline.ToSafeDate()
            oCancellation.EndDate = New Date(2099, 12, 31)

        Catch ex As Exception
            bAvailable = False

            'If it fails, assume it's non-refundable
            oCancellation.Amount = PropertyDetails.LocalCost
            oCancellation.StartDate = Date.Now.AddDays(-1)

        End Try

        oCancellationsList.Add(oCancellation)

        PropertyDetails.Cancellations = Cancellations.MergeMultipleCancellationPolicies(oCancellationsList)

        Return bAvailable

    End Function

#End Region

End Class