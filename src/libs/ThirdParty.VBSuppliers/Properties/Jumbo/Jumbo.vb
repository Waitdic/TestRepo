Imports System.Xml
Imports Intuitive
Imports Intuitive.Net.WebRequests
Imports Intuitive.Functions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.Abstractions.Models
Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports System.Text
Imports Intuitive.XMLFunctions
Imports ThirdParty.Abstractions.Support
Imports System.Net.Http

Public Class Jumbo
    Implements IThirdParty

#Region "Constructor"

    Public Sub New(settings As IJumboSettings, support As ITPSupport, httpClient As HttpClient)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
        _httpClient = Ensure.IsNotNull(httpClient, NameOf(httpClient))
    End Sub

#End Region

#Region "Properties"

    Private ReadOnly _settings As IJumboSettings

    Private ReadOnly _support As ITPSupport

    Private ReadOnly _httpClient As HttpClient

    Public ReadOnly Property SupportsRemarks As Boolean Implements IThirdParty.SupportsRemarks
        Get
            Return True
        End Get
    End Property

    Public ReadOnly Property SupportsBookingSearch() As Boolean Implements IThirdParty.SupportsBookingSearch
        Get
            Return False
        End Get
    End Property

    Private Function SupportsLiveCancellation(searchDetails As IThirdPartyAttributeSearch, source As String) As Boolean Implements IThirdParty.SupportsLiveCancellation
        Return _settings.AllowCancellations(searchDetails)
    End Function

    Private Function TakeSavingFromCommissionMargin(searchDetails As IThirdPartyAttributeSearch) As Boolean Implements IThirdParty.TakeSavingFromCommissionMargin
        Return False
    End Function

    Private Function OffsetCancellationDays(searchDetails As IThirdPartyAttributeSearch) As Integer Implements IThirdParty.OffsetCancellationDays
        Return _settings.OffsetCancellationDays(searchDetails, False)
    End Function
    Public Function RequiresVCard(info As VirtualCardInfo) As Boolean Implements IThirdParty.RequiresVCard
        Return False
    End Function

#End Region

#Region "Prebook"

    Public Function PreBook(ByVal PropertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook

        Dim oWebRequest As New Request
        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID
        Dim bSuccess As Boolean = False

        Try

            Dim sRequest As String = BuildPreBookRequest(PropertyDetails)

            'get the response
            With oWebRequest
                .EndPoint = _settings.HotelBookingURL(PropertyDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.JUMBO
                .ContentType = ContentTypes.Text_xml
                .LogFileName = "PreBook"
                .CreateLog = True
                .SetRequest(sRequest)
                .Send(_httpClient)
            End With

            Dim oResponse As XmlDocument = CleanXMLNamespaces(oWebRequest.ResponseXML)

            If oResponse.SelectSingleNode("Envelope/Body/Fault") IsNot Nothing Then
                Throw New Exception(SafeNodeValue(oResponse, "Envelope/Body/Fault/faultstring"))
            End If

            'store the Errata if we have any
            Dim Errata As XmlNodeList = oResponse.SelectNodes("Envelope/Body/valuateExtendsV13Response/result/remarks[type = 'ERRATA']")
            For Each Erratum As XmlNode In Errata
                PropertyDetails.Errata.AddNew("Important Information", Erratum.SelectSingleNode("text").InnerText)
            Next

            'get the costs from the response and match up with the room bookings
            Dim nCost As Decimal = SafeMoney(SafeNodeValue(oResponse, "Envelope/Body/valuateExtendsV13Response/result/amount/value"))

            If nCost = 0 Then
                Return False
            End If

            If SafeMoney(PropertyDetails.LocalCost) <> nCost Then
                PropertyDetails.LocalCost = nCost
                PropertyDetails.Rooms.First().LocalCost = nCost
                PropertyDetails.Warnings.AddNew("Third Party / Prebook Price Changed", "Price Changed")
            End If

            'get the cancellation costs
            Me.GetCancellations(PropertyDetails, oResponse)

            bSuccess = True

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString)
            bSuccess = False

        Finally

            If oWebRequest.RequestLog <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JUMBO, "Jumbo Prebook Request", oWebRequest.RequestLog)
            End If

            If oWebRequest.ResponseLog <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JUMBO, "Jumbo Prebook Response", oWebRequest.ResponseLog)
            End If

        End Try

        Return bSuccess

    End Function

#End Region

#Region "Book"

    Public Function Book(ByVal PropertyDetails As PropertyDetails) As String Implements IThirdParty.Book

        Dim oBookingRequest As New Request
        Dim sReference As String = ""

        If CheckAvailability(PropertyDetails) Then
            Try
                Dim sRequest As String = BuildBookRequest(PropertyDetails)

                'send the request
                With oBookingRequest
                    .EndPoint = _settings.HotelBookingURL(PropertyDetails)
                    .Source = ThirdParties.JUMBO
                    .SOAP = True
                    .SetRequest(sRequest)
                    .Send(_httpClient)
                End With

                Dim oResponse As XmlDocument = CleanXMLNamespaces(oBookingRequest.ResponseXML)

                'jumbo throws an internal error if anything is wrong with the request so webrequest returns a blank string
                If oResponse.InnerText = "" Then
                    sReference = "failed"
                End If

                If Not oResponse.SelectSingleNode("Envelope/Body/confirmExtendsV13Response/result/basket/basketId") Is Nothing Then
                    sReference = oResponse.SelectSingleNode("Envelope/Body/confirmExtendsV13Response/result/basket/basketId").InnerText
                Else
                    sReference = "failed"
                End If

            Catch ex As Exception

                PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString)
                sReference = "failed"

            Finally

                'store the request and response xml on the property booking
                If oBookingRequest.RequestLog <> "" Then
                    PropertyDetails.Logs.AddNew(ThirdParties.JUMBO, "Jumbo Book Request", oBookingRequest.RequestLog)
                End If

                If oBookingRequest.ResponseLog <> "" Then
                    PropertyDetails.Logs.AddNew(ThirdParties.JUMBO, "Jumbo Book Response", oBookingRequest.ResponseLog)
                End If

            End Try

        Else
            sReference = "failed"
            PropertyDetails.Warnings.AddNew("Availability Issue", "Selected room is no longer available at the selected price")
        End If

        Return sReference

    End Function

    Public Function CheckAvailability(ByVal PropertyDetails As PropertyDetails) As Boolean

        Dim oWebRequest As New Request
        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID
        Dim bAvailable As Boolean = False

        Try

            Dim sRequest As String = BuildPreBookRequest(PropertyDetails)

            'get the response
            With oWebRequest
                .EndPoint = _settings.HotelBookingURL(PropertyDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.JUMBO
                .ContentType = ContentTypes.Text_xml
                .LogFileName = "BookAvailability"
                .CreateLog = True
                .SetRequest(sRequest)
                .Send(_httpClient)
            End With

            Dim oResponse As XmlDocument = CleanXMLNamespaces(oWebRequest.ResponseXML)

            If oResponse.SelectSingleNode("Envelope/Body/Fault") IsNot Nothing Then
                Throw New Exception(SafeNodeValue(oResponse, "Envelope/Body/Fault/faultstring"))
            End If

            'get the costs from the response and match up with the room bookings
            Dim nCost As Decimal = SafeMoney(SafeNodeValue(oResponse, "Envelope/Body/valuateExtendsV13Response/result/amount/value"))

            If SafeMoney(PropertyDetails.LocalCost) = nCost Then
                bAvailable = True
            Else
                Throw New Exception("Change in room price")
            End If

        Catch ex As Exception

            bAvailable = False
            PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString)
            PropertyDetails.Logs.AddNew(ThirdParties.JUMBO, "Jumbo Book Exception", ex.ToString)

        Finally

            If oWebRequest.RequestLog <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JUMBO, "Jumbo Book Availability Request", oWebRequest.RequestLog)
            End If

            If oWebRequest.ResponseLog <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JUMBO, "Jumbo Book Availability Response", oWebRequest.ResponseLog)
            End If

        End Try

        Return bAvailable

    End Function

#End Region

#Region "Cancellations"

    Public Function CancelBooking(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID
        Dim oRequest As New XmlDocument
        Dim oResponse As New XmlDocument
        Dim oThirdPartyCancellationResponse As New ThirdPartyCancellationResponse

        Try

            'build up the cancellation request
            Dim sbCancellationRequest As New StringBuilder

            With sbCancellationRequest

                .Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
                .Append("<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""")
                .Append(" xmlns:tns=""http://xtravelsystem.com/v1_0rc1/basket/types"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">")
                .Append("<soap:Body>")
                .Append("<tns:cancel>")
                .Append("<CancelRQ_1>")
                .AppendFormat("<agencyCode>{0}</agencyCode>", GetCredentials(PropertyDetails, PropertyDetails.LeadGuestNationalityID, "AgencyCode", _settings))
                .AppendFormat("<brandCode>{0}</brandCode>", GetCredentials(PropertyDetails, PropertyDetails.LeadGuestNationalityID, "BrandCode", _settings))
                .AppendFormat("<pointOfSaleId>{0}</pointOfSaleId>", GetCredentials(PropertyDetails, PropertyDetails.LeadGuestNationalityID, "POS", _settings))
                .AppendFormat("<basketId>{0}</basketId>", PropertyDetails.SourceReference)
                .Append("<comment/>")
                .Append("<language>en</language>")
                .Append("<userId/>")
                .Append("</CancelRQ_1>")
                .Append("</tns:cancel>")
                .Append("</soap:Body>")
                .Append("</soap:Envelope>")

            End With

            oRequest.LoadXml(sbCancellationRequest.ToString)

            'get the response
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.BasketHandlerURL(PropertyDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.JUMBO
                .ContentType = ContentTypes.Text_xml
                .LogFileName = "Cancellation"
                .CreateLog = True
                .SetRequest(sbCancellationRequest.ToString)
                .Send(_httpClient)
            End With

            'send the request
            oResponse = CleanXMLNamespaces(oWebRequest.ResponseXML)

            If oResponse.SelectSingleNode("Envelope/Body/Error") Is Nothing AndAlso oResponse.SelectSingleNode("Envelope/Body/cancelResponse/result/status").InnerText = "CANCELLED" Then

                'populate the cancellation response
                With oThirdPartyCancellationResponse
                    .CostRecievedFromThirdParty = True
                    .Success = True
                    .TPCancellationReference = oResponse.SelectSingleNode("Envelope/Body/cancelResponse/result/basketId").InnerText
                    .Amount = SafeMoney(oResponse.SelectSingleNode("Envelope/Body/cancelResponse/result/total/value").InnerText)
                    .CurrencyCode = oResponse.SelectSingleNode("Envelope/Body/cancelResponse/result/total/currencyCode").InnerText
                End With

            End If

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString)
            oThirdPartyCancellationResponse.Success = False

        Finally

            'save the xml request and response to the propertybooking
            If oRequest.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JUMBO, "Jumbo Cancellation Request", oRequest)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JUMBO, "Jumbo Cancellation Response", oResponse)
            End If

        End Try

        Return oThirdPartyCancellationResponse

    End Function

    Public Function GetCancellationCost(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost

        Return New ThirdPartyCancellationFeeResult

    End Function

#End Region

#Region "Booking Search"

    Public Function BookingSearch(ByVal oBookingSearchDetails As BookingSearchDetails) As ThirdPartyBookingSearchResults Implements IThirdParty.BookingSearch
        Return New ThirdPartyBookingSearchResults
    End Function

    Public Function CreateReconciliationReference(ByVal sInputReference As String) As String Implements IThirdParty.CreateReconciliationReference
        Return ""
    End Function

#End Region

#Region "Booking Status Update"

    Public Function BookingStatusUpdate(ByVal oPropertyDetails As PropertyDetails) As ThirdPartyBookingStatusUpdateResult Implements IThirdParty.BookingStatusUpdate
        Return New ThirdPartyBookingStatusUpdateResult
    End Function

#End Region

    Public Sub EndSession(oPropertyDetails As PropertyDetails) Implements IThirdParty.EndSession

    End Sub

#Region "Helpers"

#Region "TPReference Helper"

    Private Class TPReference
        Public RoomTypeCode As String
        Public MealBasisCode As String

        Public Sub New(ByVal TPReference As String)
            Dim aParts() As String = TPReference.Split("|"c)
            Me.RoomTypeCode = aParts(0)
            Me.MealBasisCode = aParts(1)
        End Sub
    End Class

#End Region

#Region "GetCancellationCosts"

    Public Sub GetCancellations(ByRef PropertyDetails As PropertyDetails, ByVal oPreBookResponse As XmlDocument)

        Dim oCancellations As New Cancellations

        Try

            'get the cancellation costs
            For Each oRoomNode As XmlNode In oPreBookResponse.SelectNodes("Envelope/Body/valuateExtendsV13Response/result/occupations")

                Dim oTempCancellations As New Cancellations
                Dim nRoomRate As Decimal = SafeMoney(oRoomNode.SelectSingleNode("amount/value").InnerText)

                For Each oCancellationNode As XmlNode In oRoomNode.SelectNodes("cancellationComments[type='Cancellation term']")

                    Dim iCancellationStart As Integer = SafeInt(oCancellationNode.SelectSingleNode("text").InnerText.Split("-"c)(0))
                    Dim iCancellationPercentage As Integer = SafeInt(oCancellationNode.SelectSingleNode("text").InnerText.Split("-"c)(1).Replace("%", ""))

                    Dim oCancellation As New Cancellation

                    'add to the collection
                    oCancellation.Amount = nRoomRate * iCancellationPercentage / 100
                    oCancellation.StartDate = DateAdd(DateInterval.Day, -iCancellationStart, PropertyDetails.ArrivalDate)
                    oCancellation.EndDate = PropertyDetails.ArrivalDate

                    oTempCancellations.Add(oCancellation)

                Next

                oTempCancellations.Sort(Function(oCancellation, oOtherCancellation) oCancellation.StartDate.CompareTo(oOtherCancellation.StartDate))

                'get the end date for each of the cancellations
                For Each oCancellation As Cancellation In oTempCancellations

                    If oTempCancellations.IndexOf(oCancellation) = oTempCancellations.Count - 1 Then
                        oCancellations.AddNew(oCancellation.StartDate, oCancellation.EndDate, oCancellation.Amount)
                    Else
                        oCancellations.AddNew(oCancellation.StartDate,
                         DateAdd(DateInterval.Day, -1, oTempCancellations.Item(oTempCancellations.IndexOf(oCancellation) + 1).StartDate),
                         oCancellation.Amount)
                    End If

                Next

            Next

            oCancellations.Solidify(SolidifyType.Sum)

        Catch ex As Exception
            'no need to do anything here - we'll just return the empty class if it fails
        End Try

        PropertyDetails.Cancellations = oCancellations

    End Sub

#End Region

#End Region

#Region "Request Builders"

    Public Function BuildPreBookRequest(ByVal PropertyDetails As PropertyDetails) As String

        Dim sbPreBookRequest As New StringBuilder
        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        With sbPreBookRequest
            'header
            .Append("<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:typ=""http://xtravelsystem.com/v1_0rc1/hotel/types"">")
            .Append("<soapenv:Header/>")
            .Append("<soapenv:Body>")
            .Append("<typ:valuateExtendsV13>")

            'body
            .Append("<ValuationRQ_1>")
            .AppendFormat("<agencyCode>{0}</agencyCode>", GetCredentials(PropertyDetails, PropertyDetails.LeadGuestNationalityID, "AgencyCode", _settings))
            .AppendFormat("<brandCode>{0}</brandCode>", GetCredentials(PropertyDetails, PropertyDetails.LeadGuestNationalityID, "BrandCode", _settings))
            .AppendFormat("<pointOfSaleId>{0}</pointOfSaleId>", GetCredentials(PropertyDetails, PropertyDetails.LeadGuestNationalityID, "POS", _settings))
            .AppendFormat("<checkin>{0}</checkin>", Format(PropertyDetails.ArrivalDate, "yyyy-MM-ddThh:mm:ss"))
            .AppendFormat("<checkout>{0}</checkout>", Format(DateAdd(DateInterval.Day, PropertyDetails.Duration, PropertyDetails.ArrivalDate), "yyyy-MM-ddThh:mm:ss"))
            .AppendFormat("<establishmentId>{0}</establishmentId>", PropertyDetails.TPKey)
            .AppendFormat("<language>{0}</language>", "en")

            Dim oBaseHelper As TPReference

            For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

                oBaseHelper = New TPReference(oRoomDetails.ThirdPartyReference)

                .Append("<occupancies>")
                .AppendFormat("<adults>{0}</adults>", oRoomDetails.Adults)
                .AppendFormat("<boardTypeCode>{0}</boardTypeCode>", oBaseHelper.MealBasisCode)
                .AppendFormat("<children>{0}</children>", oRoomDetails.Children + oRoomDetails.Infants)

                For Each oPassenger As Passenger In oRoomDetails.Passengers

                    If oPassenger.PassengerType = PassengerType.Child OrElse oPassenger.PassengerType = PassengerType.Infant Then
                        .AppendFormat("<childrenAges>{0}</childrenAges>", oPassenger.Age)
                    End If
                Next

                .AppendFormat("<numberOfRooms>{0}</numberOfRooms>", "1")
                .AppendFormat("<roomTypeCode>{0}</roomTypeCode>", oBaseHelper.RoomTypeCode)
                .Append("</occupancies>")

            Next

            .Append("<onlyOnline>true</onlyOnline>")
            .Append("</ValuationRQ_1>")

            'footer
            .Append("</typ:valuateExtendsV13>")
            .Append("</soapenv:Body>")
            .Append("</soapenv:Envelope>")
        End With

        Return sbPreBookRequest.ToString

    End Function

    Public Function BuildBookRequest(ByVal PropertyDetails As PropertyDetails) As String

        Dim sbBookRequest As New StringBuilder
        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        'header
        With sbBookRequest
            .Append("<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:typ=""http://xtravelsystem.com/v1_0rc1/hotel/types"">")
            .Append("<soapenv:Header/>")
            .Append("<soapenv:Body>")
            .Append("<typ:confirmExtendsV13>")

            'body
            .Append("<ConfirmRQ_1>")
            .AppendFormat("<agencyCode>{0}</agencyCode>", GetCredentials(PropertyDetails, PropertyDetails.LeadGuestNationalityID, "AgencyCode", _settings))
            .AppendFormat("<brandCode>{0}</brandCode>", GetCredentials(PropertyDetails, PropertyDetails.LeadGuestNationalityID, "BrandCode", _settings))
            .AppendFormat("<pointOfSaleId>{0}</pointOfSaleId>", GetCredentials(PropertyDetails, PropertyDetails.LeadGuestNationalityID, "POS", _settings))
            .AppendFormat("<checkin>{0}</checkin>", Format(PropertyDetails.ArrivalDate, "yyyy-MM-ddThh:mm:ss"))
            .AppendFormat("<checkout>{0}</checkout>", Format(DateAdd(DateInterval.Day, PropertyDetails.Duration, PropertyDetails.ArrivalDate), "yyyy-MM-ddThh:mm:ss"))
            .AppendFormat("<establishmentId>{0}</establishmentId>", PropertyDetails.TPKey)
            .AppendFormat("<language>{0}</language>", "en")

            Dim sTitular As String = ""
            Dim oBaseHelper As TPReference

            For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

                oBaseHelper = New TPReference(oRoomDetails.ThirdPartyReference)

                .Append("<occupancies>")
                .AppendFormat("<adults>{0}</adults>", oRoomDetails.Adults)
                .AppendFormat("<boardTypeCode>{0}</boardTypeCode>", oBaseHelper.MealBasisCode)
                .AppendFormat("<children>{0}</children>", oRoomDetails.Children + oRoomDetails.Infants)

                sTitular = oRoomDetails.Passengers(0).FirstName & " " & oRoomDetails.Passengers(0).LastName

                For Each oPassenger As Passenger In oRoomDetails.Passengers
                    If oPassenger.PassengerType = PassengerType.Child OrElse oPassenger.PassengerType = PassengerType.Infant Then
                        .AppendFormat("<childrenAges>{0}</childrenAges>", oPassenger.Age)
                    End If
                Next

                .AppendFormat("<numberOfRooms>{0}</numberOfRooms>", "1")
                .AppendFormat("<roomTypeCode>{0}</roomTypeCode>", oBaseHelper.RoomTypeCode)
                .Append("</occupancies>")

            Next

            .Append("<onlyOnline>true</onlyOnline>")
            .Append("<basketId/>")
            .Append("<closeBasket>true</closeBasket>")

            If PropertyDetails.BookingComments.Count <> 0 Then

                Dim sBookingComment As String = String.Join(" ", PropertyDetails.BookingComments.Select(Function(comment) comment.Text))
                .Append("<comments>")
                .AppendFormat("<text>{0}</text>", sBookingComment.Trim())
                .Append("<type>Guest requests</type>")
                .Append("</comments>")

            End If

            For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

                .Append("<paxList>")

                For Each oPassenger As Passenger In oRoomDetails.Passengers
                    .AppendFormat("<paxNames>{0}</paxNames>", oPassenger.FirstName & " " & oPassenger.LastName)
                Next

                oBaseHelper = New TPReference(oRoomDetails.ThirdPartyReference)

                .AppendFormat("<roomTypeCode>{0}</roomTypeCode>", oBaseHelper.RoomTypeCode)
                .Append("</paxList>")

            Next

            'reset this to the first person on the booking
            sTitular = PropertyDetails.Rooms(0).Passengers(0).FirstName & " " & PropertyDetails.Rooms(0).Passengers(0).LastName

            .AppendFormat("<titular>{0}</titular>", sTitular)
            .Append("<userId />")
            .Append("</ConfirmRQ_1>")

            'footer
            .Append("</typ:confirmExtendsV13>")
            .Append("</soapenv:Body>")
            .Append("</soapenv:Envelope>")
        End With

        Return sbBookRequest.ToString

    End Function

#End Region

#Region "Credentials"

    Public Shared Function GetCredentials(ByVal SearchDetails As IThirdPartyAttributeSearch, ByVal SearchNationalityID As Integer, ByVal Type As String, ByVal Settings As IJumboSettings) As String

        'credentials
        Dim sAgencyCode As String = Settings.AgencyCode(SearchDetails)
        Dim sBrandCode As String = Settings.BrandCode(SearchDetails)
        Dim sPOS As String = Settings.POS(SearchDetails)

        Dim sNationalityBasedCredentials As String = Settings.NationalityBasedCredentials(SearchDetails)
        If sNationalityBasedCredentials <> "" AndAlso sNationalityBasedCredentials.Split("#"c).Count > 0 Then
            'find the credentials with the correct nationality
            For Each sNationalityBasedCredential As String In sNationalityBasedCredentials.Split("#"c)
                If sNationalityBasedCredential.Split("|"c).Count = 4 Then
                    Dim sNationalities As String = sNationalityBasedCredential.Split("|"c)(3)

                    If sNationalities.Split(","c).Contains(SearchNationalityID.ToString) Then

                        sBrandCode = sNationalityBasedCredential.Split("|"c)(0)
                        sPOS = sNationalityBasedCredential.Split("|"c)(1)
                        sAgencyCode = sNationalityBasedCredential.Split("|"c)(2)

                        Exit For
                    End If
                End If
            Next
        End If

        Select Case Type.ToLower
            Case "agencycode"
                Return sAgencyCode
            Case "brandcode"
                Return sBrandCode
            Case "pos"
                Return sPOS
            Case Else
                Return ""
        End Select
    End Function

#End Region

End Class