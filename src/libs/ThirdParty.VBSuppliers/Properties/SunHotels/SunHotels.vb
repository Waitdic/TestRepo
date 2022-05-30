Imports System.Xml
Imports Intuitive
Imports Intuitive.Net.WebRequests
Imports Intuitive.Functions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports System.Text
Imports Intuitive.XMLFunctions
Imports ThirdParty.Abstractions.Support
Imports ThirdParty.Abstractions.Search.Models
Imports iVector.Search.Property
Imports System.Net.Http
Imports Intuitive.Helpers.Extensions

Public Class SunHotels
    Implements IThirdParty

#Region "Constructor"

    Public Sub New(settings As ISunHotelsSettings, support As ITPSupport, httpClient As HttpClient)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
        _httpClient = Ensure.IsNotNull(httpClient, NameOf(httpClient))
    End Sub

#End Region

#Region "Properties"

    Private ReadOnly _settings As ISunHotelsSettings

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

#Region "PreBook"

    Public Function PreBook(ByVal PropertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        'build the search url
        Dim sbPrebookURL As New StringBuilder
        Dim oPrebookResponseXML As New XmlDocument

        Try
            With sbPrebookURL

                .Append(_settings.PreBookURL(PropertyDetails))
                .AppendFormat("userName={0}", _settings.Username(PropertyDetails))
                .AppendFormat("&password={0}", _settings.Password(PropertyDetails))
                .AppendFormat("&language={0}", _settings.Language(PropertyDetails))
                .AppendFormat("&currency={0}", _settings.Currency(PropertyDetails))
                .AppendFormat("&checkInDate={0}", SunHotels.GetSunHotelsDate(PropertyDetails.ArrivalDate))
                .AppendFormat("&checkOutDate={0}", SunHotels.GetSunHotelsDate(PropertyDetails.DepartureDate))
                .AppendFormat("&roomId={0}", PropertyDetails.Rooms(0).ThirdPartyReference.Split("_"c)(0))
                .AppendFormat("&rooms={0}", PropertyDetails.Rooms.Count)
                .AppendFormat("&adults={0}", PropertyDetails.Rooms(0).Adults)
                .AppendFormat("&children={0}", PropertyDetails.Rooms(0).Children)
                If PropertyDetails.Rooms(0).Children > 0 Then
                    .AppendFormat("&childrenAges={0}", PropertyDetails.Rooms(0).GetChildAgeCsv())
                Else
                    .AppendFormat("&childrenAges=")
                End If
                .AppendFormat("&infant={0}", SunHotels.IsInfantIncluded(PropertyDetails.Rooms(0)))
                .AppendFormat("&mealId={0}", PropertyDetails.Rooms(0).ThirdPartyReference.Split("_"c)(1))
                .AppendFormat("&customerCountry={0}", _settings.Nationality(PropertyDetails))
                .Append("&B2C=")
                .Append("&searchPrice=")
                .AppendFormat("&showPriceBreakdown=0")
                .Append("&blockSuperDeal=0")

            End With

            'send the request to SunHotels
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = sbPrebookURL.ToString
                .Method = eRequestMethod.GET
                .Source = ThirdParties.SUNHOTELS
                .ContentType = ContentTypes.Text_xml
                .LogFileName = "Prebook"
                .CreateLog = True
                .TimeoutInSeconds = 100
                .Send(_httpClient)
            End With

            oPrebookResponseXML = oWebRequest.ResponseXML

            'strip out anything we don't need
            oPrebookResponseXML = CleanXMLNamespaces(oPrebookResponseXML)

            If oPrebookResponseXML.SelectSingleNode("preBookResult/Error") IsNot Nothing Then
                PropertyDetails.Warnings.AddNew("Prebook Failed", XMLFunctions.SafeNodeValue(oPrebookResponseXML, "preBookResult/Error/Message"))
                Return False
            End If

            'store the Errata if we have any
            Dim Errata As XmlNodeList = oPrebookResponseXML.SelectNodes("preBookResult/Notes/Note")
            For Each Erratum As XmlNode In Errata
                PropertyDetails.Errata.AddNew("Important Information", Erratum.SelectSingleNode("text").InnerText)
            Next

            'recheck the price in case it has changed
            '** needs to be changed if we implement multi-rooms in the future**
            Dim nPrice As Decimal = 0
            Dim oCancellations As New Cancellations
            nPrice = SafeMoney(Intuitive.XMLFunctions.SafeNodeValue(oPrebookResponseXML, "preBookResult/Price"))
            PropertyDetails.TPRef1 = Intuitive.XMLFunctions.SafeNodeValue(oPrebookResponseXML, "preBookResult/PreBookCode")

            'override the cancellations

            Dim iCancellationPolicyCount As Integer = 0

            For Each oCancellationPolicy As XmlNode In oPrebookResponseXML.SelectNodes("preBookResult/CancellationPolicies/CancellationPolicy")

                iCancellationPolicyCount += 1

                Dim oHours As New TimeSpan(SafeInt(oCancellationPolicy.SelectSingleNode("deadline").InnerText), 0, 0)
                Dim dStartDate As Date

                'for 100% cancel;lations we don't get an hours before
                'so force the start date to be from now
                If oHours.TotalHours = 0 Then
                    dStartDate = Date.Now.Date
                Else
                    dStartDate = PropertyDetails.ArrivalDate.Subtract(oHours)
                End If

                Dim nCancellationCost As Decimal = SafeDecimal(nPrice * SafeDecimal(oCancellationPolicy.SelectSingleNode("percentage").InnerText) / 100)

                'take the end date of the next cancellation policy otherwise set it a long way off
                Dim dEndDate As Date
                If oPrebookResponseXML.SelectSingleNode(String.Format("preBookResult/CancellationPolicies/CancellationPolicy[{0}]", iCancellationPolicyCount + 1)) IsNot Nothing Then

                    'the hours of the end dates need to be rounded to the nearest 24 hours to stop them overlapping with the start of the next cancellation policy and making
                    'the charges add together
                    Dim oEndHours As New TimeSpan(RoundHoursUpToTheNearest24Hours(SafeInt(oPrebookResponseXML.SelectSingleNode(String.Format(
                           "preBookResult/CancellationPolicies/CancellationPolicy[{0}]/deadline", iCancellationPolicyCount + 1)).InnerText)), 0, 0)

                    dEndDate = PropertyDetails.ArrivalDate.Subtract(oEndHours)
                    dEndDate = dEndDate.AddDays(-1)

                Else
                    dEndDate = New Date(2099, 1, 1)

                End If

                oCancellations.AddNew(dStartDate, dEndDate, nCancellationCost)

            Next

            PropertyDetails.Cancellations = oCancellations

            If nPrice > 0 AndAlso nPrice <> PropertyDetails.LocalCost Then
                PropertyDetails.Rooms(0).GrossCost = nPrice
                PropertyDetails.Rooms(0).LocalCost = nPrice
                PropertyDetails.GrossCost = nPrice
                PropertyDetails.LocalCost = nPrice
                PropertyDetails.AddLog(ThirdParties.SUNHOTELS, "Third Party / Prebook Price Changed")
            End If
            Return True

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("SunHotels Exception", ex.ToString)
            Return False

        Finally

            'store the request and response xml on the property booking
            If sbPrebookURL.ToString <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels PreBook Request", sbPrebookURL.ToString)
            End If

            If oPrebookResponseXML.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Prebook Response", oPrebookResponseXML)
            End If

        End Try

    End Function

#End Region

#Region "Book"

    Public Function Book(ByVal PropertyDetails As PropertyDetails) As String Implements IThirdParty.Book

        '**NOTE - THIS ONLY WORKS FOR SINGLE ROOM BOOKINGS**

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID
        Dim sRequestURL As String = ""
        Dim oResponse As New XmlDocument
        Dim sBookingReference As String = ""

        Try

            'split out the room reference and the meal type
            Dim sRoomID As String = PropertyDetails.Rooms(0).ThirdPartyReference.Split("_"c)(0)

            'Grab the BookingReference If we have it
            Dim siVectorReference As String = ""
            If PropertyDetails.BookingReference = "" Then
                siVectorReference = _settings.SupplierReference(PropertyDetails)
            Else
                siVectorReference = PropertyDetails.BookingReference
            End If

            'build the book request url
            Dim sbBookingRequestURL As New StringBuilder

            With sbBookingRequestURL

                .Append(_settings.BookURL(PropertyDetails))
                .AppendFormat("userName={0}", _settings.Username(PropertyDetails))
                .AppendFormat("&password={0}", _settings.Password(PropertyDetails))
                .AppendFormat("&currency={0}", _settings.Currency(PropertyDetails))
                .AppendFormat("&language={0}", _settings.Language(PropertyDetails))
                .AppendFormat("&email={0}", _settings.EmailAddress(PropertyDetails))
                .AppendFormat("&checkInDate={0}", GetSunHotelsDate(PropertyDetails.ArrivalDate))
                .AppendFormat("&checkOutDate={0}", GetSunHotelsDate(PropertyDetails.DepartureDate))
                .AppendFormat("&roomId={0}", sRoomID)
                .AppendFormat("&rooms={0}", PropertyDetails.Rooms.Count)
                .AppendFormat("&adults={0}", PropertyDetails.Adults)
                .AppendFormat("&children={0}", PropertyDetails.Children)
                .AppendFormat("&infant={0}", SunHotels.IsInfantIncluded(PropertyDetails.Rooms(0)))
                .AppendFormat("&yourRef={0}", siVectorReference)

                If PropertyDetails.BookingComments.Count <> 0 Then
                    .AppendFormat("&specialrequest={0}", PropertyDetails.BookingComments(0).Text)
                Else
                    .Append("&specialrequest=")
                End If

                .AppendFormat("&mealId={0}", PropertyDetails.Rooms(0).ThirdPartyReference.Split("_"c)(1))

                Dim iAdults As Integer = 1
                Dim iChildren As Integer = 1

                'add the adults that in the booking
                For Each oPassenger As Passenger In PropertyDetails.Rooms(0).Passengers
                    If oPassenger.PassengerType = PassengerType.Adult Then

                        .AppendFormat("&adultGuest{0}FirstName={1}", iAdults, oPassenger.FirstName)
                        .AppendFormat("&adultGuest{0}LastName={1}", iAdults, oPassenger.LastName)
                        iAdults += 1

                    End If
                Next

                'add empty elements for all the other adults up to 9
                For i As Integer = 1 To 9
                    If i >= iAdults Then
                        .AppendFormat("&adultGuest{0}FirstName=", i)
                        .AppendFormat("&adultGuest{0}LastName=", i)
                    End If
                Next

                'add the children
                For Each oPassenger As Passenger In PropertyDetails.Rooms(0).Passengers
                    If oPassenger.PassengerType = PassengerType.Child AndAlso oPassenger.Age <= 17 Then

                        .AppendFormat("&childrenGuest{0}FirstName={1}", iChildren, oPassenger.FirstName)
                        .AppendFormat("&childrenGuest{0}LastName={1}", iChildren, oPassenger.LastName)
                        .AppendFormat("&childrenGuestAge{0}={1}", iChildren, oPassenger.Age)
                        iChildren += 1

                    End If
                Next

                'add empty elements for all the other adults up to 9
                For i As Integer = 1 To 9
                    If i >= iChildren Then
                        .AppendFormat("&childrenGuest{0}FirstName=", i)
                        .AppendFormat("&childrenGuest{0}LastName=", i)
                        .AppendFormat("&childrenGuestAge{0}=", i)
                    End If
                Next
                .AppendFormat("&paymentMethodId={0}", PropertyDetails.Rooms(0).ThirdPartyReference.Split("_"c)(3))
                .AppendFormat(GetCardDetails(PropertyDetails.Rooms(0).ThirdPartyReference.Split("_"c)(3), PropertyDetails))
                .Append("&customerEmail=")
                .Append("&invoiceRef=")
                .Append("&commissionAmountInHotelCurrency=")
                .AppendFormat("&customerCountry={0}", _settings.Nationality(PropertyDetails))
                .Append("&B2C=")
                .AppendFormat("&PreBookCode={0}", PropertyDetails.TPRef1)

            End With

            sRequestURL = sbBookingRequestURL.ToString

            Dim oWebRequest As New Request
            With oWebRequest
                .Source = ThirdParties.SUNHOTELS
                .EndPoint = sRequestURL
                .Method = eRequestMethod.GET
                .ContentType = ContentTypes.Text_xml
                .Send(_httpClient)
            End With

            oResponse = CleanXMLNamespaces(oWebRequest.ResponseXML)

            If oResponse.SelectSingleNode("bookResult/booking/Error") Is Nothing Then

                'grab the booking reference
                sBookingReference = oResponse.SelectSingleNode("bookResult/booking/bookingnumber").InnerText

            Else

                sBookingReference = "failed"

            End If

        Catch ex As Exception

            sBookingReference = "failed"
            PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString)

        Finally

            'store the request and response xml on the property booking
            If sRequestURL <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Book Request", sRequestURL)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Book Response", oResponse)
            End If

        End Try

        Return sBookingReference

    End Function

#End Region

#Region "Cancellations"

    Public Function CancelBooking(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking

        Dim oThirdPartyCancellationResponse As New ThirdPartyCancellationResponse

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID
        Dim sbCancellationRequestURL As New StringBuilder
        Dim oCancellationResponseXML As New XmlDocument

        Try

            'build the cancellation url
            With sbCancellationRequestURL
                .Append(_settings.CancelURL(PropertyDetails))
                .AppendFormat("userName={0}", _settings.Username(PropertyDetails))
                .AppendFormat("&password={0}", _settings.Password(PropertyDetails))
                .AppendFormat("&bookingID={0}", PropertyDetails.SourceReference.ToString)
                .AppendFormat("&language={0}", _settings.Language(PropertyDetails))
            End With

            'Send the request
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = sbCancellationRequestURL.ToString
                .Method = eRequestMethod.GET
                .Source = ThirdParties.SUNHOTELS
                .ContentType = ContentTypes.Text_xml
                .LogFileName = "Cancel"
                .CreateLog = True
                .TimeoutInSeconds = 100
                .Send(_httpClient)
            End With

            oCancellationResponseXML = oWebRequest.ResponseXML

            'tidy up the response
            oCancellationResponseXML = CleanXMLNamespaces(oCancellationResponseXML)

            'Check for success
            Dim oResultCodeNode As XmlNode = oCancellationResponseXML.SelectSingleNode("result/Code")
            If oResultCodeNode IsNot Nothing AndAlso oResultCodeNode.InnerText.ToSafeInt <> -1 Then
                oThirdPartyCancellationResponse.TPCancellationReference = Now.ToString("dd MMM/HH:mm")
                oThirdPartyCancellationResponse.Success = True
            End If

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString)
            oThirdPartyCancellationResponse.Success = False

        Finally

            'store the request and response xml on the property booking
            If sbCancellationRequestURL.ToString <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Cancellation Request", sbCancellationRequestURL.ToString)
            End If

            If oCancellationResponseXML.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Cancellation Response", oCancellationResponseXML)
            End If

        End Try

        Return oThirdPartyCancellationResponse

    End Function

    Public Function GetCancellationCost(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost
        Return GetCancellationCost(PropertyDetails)
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

    Public Shared Function GetSunHotelsDate(ByVal dDate As Date) As String

        Return dDate.ToString("yyyy-MM-dd")

    End Function

    Public Shared Function IsInfantIncluded(searchDetails As SearchDetails) As Integer
        Dim iInfantIncluded As Integer = 0
        If searchDetails.TotalInfants > 0 Then
            iInfantIncluded = 1
        End If
        Return iInfantIncluded
    End Function

    Public Shared Function IsInfantIncluded(roomDetails As Models.Property.Booking.RoomDetails) As Integer
        Dim iInfantIncluded As Integer = 0
        If roomDetails.Infants > 0 Then
            iInfantIncluded = 1
        End If
        Return iInfantIncluded
    End Function

    Public Shared Function GetAdultsFromSearchDetails(ByVal SearchDetails As SearchDetails) As String

        Dim iAdultCount As Integer = 0

        For Each oRoom As RoomDetail In SearchDetails.RoomDetails

            iAdultCount += oRoom.Adults

            For Each iAge As Integer In oRoom.ChildAges
                If iAge > 17 Then iAdultCount += 1
            Next

        Next

        Return iAdultCount.ToString

    End Function

    Public Shared Function GetChildrenFromSearchDetails(ByVal SearchDetails As SearchDetails) As String

        Dim iChildCount As Integer = 0

        For Each oRoom As RoomDetail In SearchDetails.RoomDetails

            For Each iAge As Integer In oRoom.ChildAges
                If iAge <= 17 Then iChildCount += 1
            Next

        Next

        Return iChildCount.ToString

    End Function

    Public Shared Function GetChildrenAges(ByVal SearchDetails As SearchDetails) As String

        Dim sChildAges As String = ""
        Dim sb As New StringBuilder

        For Each oRoom As RoomDetail In SearchDetails.RoomDetails
            sb.Append(oRoom.ChildAgeCSV)
        Next
        sChildAges = String.Join(",", sb.ToString)
        Return sChildAges

    End Function

    Public Shared Function RoundHoursUpToTheNearest24Hours(ByVal Hours As Integer) As Integer

        Dim i As Integer
        i = Math.Round((Hours / 24).ToSafeDecimal(), 0, MidpointRounding.AwayFromZero).ToSafeInt()

        Return i * 24

    End Function

    Public Function GetCardDetails(ByVal PaymentMethodId As String, ByVal PropertyDetails As PropertyDetails) As String
        Dim sbCardDetails As New StringBuilder
        sbCardDetails.Append("&creditCardType=")
        sbCardDetails.Append("&creditCardNumber=")
        sbCardDetails.Append("&creditCardHolder=")
        sbCardDetails.Append("&creditCardCVV2=")
        sbCardDetails.Append("&creditCardExpYear=")
        sbCardDetails.Append("&creditCardExpMonth=")
        Return sbCardDetails.ToString
    End Function

#End Region

End Class