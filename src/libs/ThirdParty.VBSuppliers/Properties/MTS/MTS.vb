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

Public Class MTS
    Implements IThirdParty

#Region "Constructor"

    Public Sub New(settings As IMTSSettings, support As ITPSupport, httpClient As HttpClient)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
        _httpClient = Ensure.IsNotNull(httpClient, NameOf(httpClient))
    End Sub

#End Region

#Region "Properties"

    Private ReadOnly _settings As IMTSSettings

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

    Private Function IThirdParty_SupportsLiveCancellation(searchDetails As IThirdPartyAttributeSearch, source As String) As Boolean Implements IThirdParty.SupportsLiveCancellation
        Return _settings.AllowCancellations(searchDetails)
    End Function

    Private Function IThirdParty_TakeSavingFromCommissionMargin(searchDetails As IThirdPartyAttributeSearch) As Boolean Implements IThirdParty.TakeSavingFromCommissionMargin
        Return False
    End Function

    Private Function IThirdParty_OffsetCancellationDays(searchDetails As IThirdPartyAttributeSearch) As Integer Implements IThirdParty.OffsetCancellationDays
        Return _settings.OffsetCancellationDays(searchDetails, False)
    End Function

    Public Function RequiresVCard(info As VirtualCardInfo) As Boolean Implements IThirdParty.RequiresVCard
        Return False
    End Function

    Private ReadOnly overrideCountriesList As List(Of String) = New List(Of String)

    Private Function GetOverrideCountries(oPropertyDetails As PropertyDetails) As List(Of String)

        If overrideCountriesList.Count = 0 Then
            Dim sOverrideCountries As String = _settings.OverrideCountries(oPropertyDetails)

            If Not String.IsNullOrWhiteSpace(sOverrideCountries) Then
                'split the string and add each one to _overrideCountries
                For Each sCountry As String In sOverrideCountries.Split("|"c)
                    overrideCountriesList.Add(sCountry)
                Next
            Else
                overrideCountriesList.Add("United Arab Emirates")
                overrideCountriesList.Add("Turkey")
                overrideCountriesList.Add("Egypt")
            End If
        End If
        Return overrideCountriesList
    End Function
#End Region

#Region "Prebook"

    Public Function PreBook(ByVal PropertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook

        Dim bSuccess As Boolean = True

        Dim sbVerifyCart As New StringBuilder
        Dim oResponse As New XmlDocument

        Try
            With sbVerifyCart
                .Append("<OTA_HotelResRQ xmlns=""http://www.opentravel.org/OTA/2003/05"" EchoToken=""12866195988106211282233751"" ResStatus=""Quote"" Version=""0.1"" schemaLocation=""OTA_HotelResRQ.xsd"">")
                .Append(GeneratePosTag(PropertyDetails))
                .Append("<HotelReservations>")
                .Append("<HotelReservation>")
                .Append("<RoomStays>")

                Dim iCount As Integer = 0
                Dim iRoomCount As Integer = 1
                For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms
                    .Append("<RoomStay>")
                    .Append("<RoomTypes>")
                    .AppendFormat("<RoomType RoomTypeCode = ""{0}""></RoomType>", oRoomDetails.ThirdPartyReference.Split("|"c)(0))
                    .Append("</RoomTypes>")
                    .AppendFormat("<TimeSpan End = ""{0}"" Start = ""{1}""></TimeSpan>", PropertyDetails.DepartureDate.ToString("yyyy-MM-dd"), PropertyDetails.ArrivalDate.ToString("yyyy-MM-dd"))
                    .AppendFormat("<BasicPropertyInfo HotelCode = ""{0}""></BasicPropertyInfo>", PropertyDetails.TPKey)
                    .Append("<ResGuestRPHs>")
                    'need a new RPH for each guest; loop to add 1 for each new guest
                    For Each oPassenger As Passenger In oRoomDetails.Passengers
                        iCount = iCount + 1
                        Dim iGuestNumber As Integer = iCount
                        .AppendFormat("<ResGuestRPH RPH = ""{0}""></ResGuestRPH>", iGuestNumber)
                    Next
                    .Append("</ResGuestRPHs>")
                    .Append("<ServiceRPHs>")
                    .AppendFormat("<ServiceRPH RPH = ""{0}""></ServiceRPH>", iRoomCount)
                    .Append("</ServiceRPHs>")
                    .Append("</RoomStay>")
                    iRoomCount += 1
                Next
                iRoomCount = 1
                .Append("</RoomStays>")
                .Append("<Services>")
                For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms
                    .AppendFormat("<Service ServiceInventoryCode=""{0}"" ServiceRPH=""{1}""></Service>", oRoomDetails.ThirdPartyReference.Split("|"c)(1), iRoomCount)
                    iRoomCount += 1
                Next

                .Append("</Services>")
                .Append("<ResGuests>")

                'need to loop for each person
                Dim iGuestCounter As Integer = 0
                For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms
                    For Each oPassenger As Passenger In oRoomDetails.Passengers

                        iGuestCounter += 1

                        Dim AgeQualifyingCode As Integer
                        If oPassenger.PassengerType = PassengerType.Adult Then
                            AgeQualifyingCode = 10
                        End If
                        If oPassenger.PassengerType = PassengerType.Child Then
                            AgeQualifyingCode = 8
                        End If
                        If oPassenger.PassengerType = PassengerType.Infant Then
                            AgeQualifyingCode = 7
                        End If

                        'Dim iGuestNumber As Integer = iCount

                        .AppendFormat("<ResGuest AgeQualifyingCode = ""{0}"" ResGuestRPH = ""{1}"">", AgeQualifyingCode, iGuestCounter)

                        If AgeQualifyingCode = 8 Then
                            .AppendFormat("<GuestCounts><GuestCount Age=""{0}""/></GuestCounts>", oPassenger.Age)
                        End If

                        If AgeQualifyingCode = 7 Then
                            .AppendFormat("<GuestCounts><GuestCount Age=""1""/></GuestCounts>")
                        End If

                        .Append("</ResGuest>")
                    Next
                Next
                .Append("</ResGuests>")
                .Append("</HotelReservation>")
                .Append("</HotelReservations>")
                .Append("</OTA_HotelResRQ>")

            End With
            'get the add response 
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.BaseURL(PropertyDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.MTS
                .ContentType = ContentTypes.Application_x_www_form_urlencoded
                .LogFileName = "PreBook"
                .CreateLog = True
                .SetRequest(sbVerifyCart.ToString)
                .Send(_httpClient)
            End With

            oResponse.LoadXml(oWebRequest.ResponseXML.InnerXml.Replace(" xmlns=""http://www.opentravel.org/OTA/2003/05""", ""))

            'get the costs from the response
            Dim oCosts As XmlNodeList = oResponse.SelectNodes("OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/Total/@AmountAfterTax")

            If oCosts(0).InnerText.ToSafeMoney() <> Math.Round(PropertyDetails.TotalCost, 2, MidpointRounding.AwayFromZero) Then

                'Only returns total cost so divide by number of rooms
                Dim dCost As Decimal = oCosts(0).InnerText.ToSafeMoney() / PropertyDetails.Rooms.Count
                For Each oRoom As RoomDetails In PropertyDetails.Rooms
                    oRoom.LocalCost = dCost
                    oRoom.GrossCost = dCost
                Next
            End If

            'cancellation charges
            Dim oCancellations As New Cancellations
            oCancellations = GetCancellations(PropertyDetails, oResponse)

            For Each oCancellation As Cancellation In oCancellations
                PropertyDetails.Cancellations.Add(oCancellation)
            Next

            'Grab the Errata
            For Each oErrataNode As XmlNode In oResponse.SelectNodes("/OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/BasicPropertyInfo/VendorMessages/VendorMessage")
                PropertyDetails.Errata.AddNew(oErrataNode.SelectSingleNode("@Title").InnerText, oErrataNode.SelectSingleNode("SubSection/Paragraph/Text").InnerText)
            Next

        Catch ex As Exception

            bSuccess = False
            PropertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString)

        Finally
            'store the request and response xml on the property pre-booking
            If sbVerifyCart.ToString <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Pre-Book Request", sbVerifyCart.ToString)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Pre-Book Response", oResponse)
            End If
        End Try
        Return bSuccess
    End Function

#End Region

#Region "Book"

    Public Function Book(ByVal PropertyDetails As PropertyDetails) As String Implements IThirdParty.Book

        Dim sbRequest As New StringBuilder
        Dim oResponse As New XmlDocument
        Dim sReference As String = ""
        Dim overrideCountries As List(Of String) = GetOverrideCountries(PropertyDetails)

        Try
            With sbRequest
                .Append("<OTA_HotelResRQ xmlns=""http://www.opentravel.org/OTA/2003/05"" EchoToken=""12866195988106211282233751"" ResStatus=""Commit"" Version=""0.1"" schemaLocation=""OTA_HotelResRQ.xsd"">")
                .Append(GeneratePosTag(PropertyDetails))
                .Append("<HotelReservations>")
                .Append("<HotelReservation>")
                .Append("<RoomStays>")

                Dim iCount As Integer = 0
                Dim iRoomCount As Integer = 1
                For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

                    .Append("<RoomStay>")
                    .Append("<RoomTypes>")
                    .AppendFormat("<RoomType RoomTypeCode = ""{0}""></RoomType>", oRoomDetails.ThirdPartyReference.Split("|"c)(0))
                    .Append("</RoomTypes>")
                    .AppendFormat("<TimeSpan End = ""{0}"" Start = ""{1}""></TimeSpan>", PropertyDetails.DepartureDate.ToString("yyyy-MM-dd"), PropertyDetails.ArrivalDate.ToString("yyyy-MM-dd"))
                    .AppendFormat("<BasicPropertyInfo HotelCode = ""{0}""></BasicPropertyInfo>", PropertyDetails.TPKey)
                    .Append("<ResGuestRPHs>")

                    'need a new RPH for each guest; loop to add 1 for each new guest

                    For Each oPassenger As Passenger In oRoomDetails.Passengers

                        iCount = iCount + 1

                        Dim iGuestNumber As Integer = iCount

                        .AppendFormat("<ResGuestRPH RPH = ""{0}""></ResGuestRPH>", iGuestNumber)

                    Next
                    .Append("</ResGuestRPHs>")
                    .Append("<ServiceRPHs>")
                    .AppendFormat("<ServiceRPH RPH = ""{0}""></ServiceRPH>", iRoomCount)
                    .Append("</ServiceRPHs>")
                    .Append("</RoomStay>")
                    iRoomCount += 1
                Next
                .Append("</RoomStays>")
                .Append("<Services>")
                iRoomCount = 1
                For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms
                    .AppendFormat("<Service ServiceInventoryCode=""{0}"" ServiceRPH=""{1}""></Service>", oRoomDetails.ThirdPartyReference.Split("|"c)(1), iRoomCount)
                    iRoomCount += 1
                Next

                .Append("</Services>")
                .Append("<ResGuests>")

                'need to loop for each person
                Dim iGuestCounter As Integer = 0
                For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms
                    For Each oPassenger As Passenger In oRoomDetails.Passengers

                        iGuestCounter += 1

                        Dim AgeQualifyingCode As Integer
                        If oPassenger.PassengerType = PassengerType.Adult Then
                            AgeQualifyingCode = 10
                        End If
                        If oPassenger.PassengerType = PassengerType.Child Then
                            AgeQualifyingCode = 8
                        End If
                        If oPassenger.PassengerType = PassengerType.Infant Then
                            AgeQualifyingCode = 7
                        End If

                        'Dim iGuestNumber As Integer = iCount

                        .AppendFormat("<ResGuest AgeQualifyingCode = ""{0}"" ResGuestRPH = ""{1}"">", AgeQualifyingCode, iGuestCounter)

                        .Append("<Profiles><ProfileInfo><Profile><Customer><PersonName>")
                        .AppendFormat("<NamePrefix>{0}</NamePrefix>", oPassenger.Title)
                        .AppendFormat("<GivenName>{0}</GivenName>", oPassenger.FirstName)
                        .AppendFormat("<Surname>{0}</Surname>", oPassenger.LastName)
                        .Append("</PersonName></Customer></Profile></ProfileInfo></Profiles>")

                        If AgeQualifyingCode = 8 Then
                            .AppendFormat("<GuestCounts><GuestCount Age=""{0}""/></GuestCounts>", oPassenger.Age)
                        End If

                        If AgeQualifyingCode = 7 Then
                            .AppendFormat("<GuestCounts><GuestCount Age=""1""/></GuestCounts>")
                        End If

                        .Append("</ResGuest>")
                    Next
                Next

                .Append("</ResGuests>")

                If PropertyDetails.BookingReference <> "" OrElse PropertyDetails.BookingComments.Count > 0 Then

                    .Append("<ResGlobalInfo>")

                    If PropertyDetails.BookingReference <> "" Then

                        If overrideCountries.Contains(PropertyDetails.Rooms(0).ThirdPartyReference.Split("|"c)(2)) Then
                            Dim sID As String = _settings.OverRideID(PropertyDetails)
                            .Append("<HotelReservationIDs>")
                            .AppendFormat("<HotelReservationID ResID_SourceContext=""Client"" ResID_Source=""{0}"" ResID_Value=""{1}"" />",
                              sID, PropertyDetails.BookingReference.Replace(" "c, ""))
                            .Append("</HotelReservationIDs>")
                        End If
                    End If

                    If PropertyDetails.BookingComments.Count > 0 Then

                        .Append("<Comments>")

                        For Each oBookingComment As BookingComment In PropertyDetails.BookingComments

                            .Append("<Comment Name = ""Applicant Notice"">")
                            .AppendFormat("<Text>{0}</Text>", oBookingComment.Text)
                            .Append("</Comment>")

                        Next
                        .Append("</Comments>")
                    End If
                    .Append("</ResGlobalInfo>")
                End If
                .Append("</HotelReservation>")
                .Append("</HotelReservations>")
                .Append("</OTA_HotelResRQ>")

                'get the response 
                Dim oBookingRequest As New Request
                With oBookingRequest
                    .EndPoint = _settings.BaseURL(PropertyDetails)
                    .Method = eRequestMethod.POST
                    .Source = ThirdParties.MTS
                    .ContentType = ContentTypes.Application_x_www_form_urlencoded
                    .SetRequest(sbRequest.ToString)
                    .Send(_httpClient)
                End With

                oResponse = oBookingRequest.ResponseXML
                oResponse.LoadXml(oResponse.InnerXml.Replace(" xmlns=""http://www.opentravel.org/OTA/2003/05""", ""))

                'check for any errors and save the booking code
                'p63 of documentation
                If oResponse.SelectSingleNode("OTA_HotelResRS/Errors/Error") IsNot Nothing Then

                    sReference = "failed"

                Else

                    sReference = oResponse.SelectSingleNode("OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/HotelReservationIDs/HotelReservationID[@ResID_Source=""OTS""]/@ResID_Value").InnerText

                End If

            End With

        Catch ex As Exception

            sReference = "failed"
            PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString)

        Finally

            'store the request and response xml on the property booking
            If sbRequest.ToString <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Book Request", sbRequest.ToString)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Book Response", oResponse)
            End If

        End Try

        Return sReference

    End Function

#End Region

#Region "Cancellations"

    Public Function CancelBooking(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking

        Dim sbRequest As New StringBuilder
        Dim oCancellationRequest As New XmlDocument
        Dim oCancellationResponse As New XmlDocument
        Dim oThirdPartyCancellationResponse As New ThirdPartyCancellationResponse

        Dim sSourceReference As String
        If PropertyDetails.SourceReference IsNot Nothing Then
            sSourceReference = PropertyDetails.SourceReference
        Else
            Return oThirdPartyCancellationResponse
        End If

        Try
            'build the cancellation request 
            With sbRequest

                .Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
                .Append("<OTA_CancelRQ xmlns=""http://www.opentravel.org/OTA/2003/05"" Version=""0.1"" CancelType=""Commit"">")
                .Append(GeneratePosTag(PropertyDetails))
                .AppendFormat("<UniqueID Type=""14"" ID=""{0}"" ID_Context=""Internal""/>", sSourceReference)
                .Append("</OTA_CancelRQ>")

            End With
            'send the request 
            oCancellationRequest.LoadXml(sbRequest.ToString)

            'get the response
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.BaseURL(PropertyDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.MTS
                .ContentType = ContentTypes.Application_x_www_form_urlencoded
                .LogFileName = "Cancel"
                .CreateLog = True
                .SetRequest(oCancellationRequest)
                .Send(_httpClient)
            End With

            oCancellationResponse = oWebRequest.ResponseXML
            oCancellationResponse.LoadXml(oCancellationResponse.InnerXml.Replace(" xmlns=""http://www.opentravel.org/OTA/2003/05""", ""))


            If oCancellationResponse.SelectNodes("Errors").Count > 0 Then

                Throw New Exception("Cancellation request did not return success")

            Else

                If Not oCancellationResponse.SelectSingleNode("/OTA_CancelRS/@Status").InnerText = "Committed" Then

                    Throw New Exception("Cancellation request did not return success")

                Else

                    'Get a reference
                    oThirdPartyCancellationResponse.TPCancellationReference = oCancellationResponse.SelectSingleNode("/OTA_CancelRS/UniqueID/@ID").InnerText
                    oThirdPartyCancellationResponse.Success = True

                End If
            End If

        Catch ex As Exception

            oThirdPartyCancellationResponse.TPCancellationReference = "Failed"
            oThirdPartyCancellationResponse.Success = False

            PropertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString)

        Finally

            'store the request and response xml on the property booking
            If sbRequest.ToString <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Cancellation Request", sbRequest.ToString)
            End If

            If oCancellationResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Cancellation Response", oCancellationResponse)
            End If

        End Try

        Return oThirdPartyCancellationResponse

    End Function

    Public Function GetCancellations(ByVal PropertyDetails As PropertyDetails, ByVal oXML As XmlDocument) As Cancellations

        Dim oCancellations As New Cancellations

        Try

            If oXML.SelectSingleNode("OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/CancelPenalties/CancelPenalty/AmountPercent") IsNot Nothing Then

                'get cancellations in list so can sort them by release period
                For Each oXMLNode As XmlNode In oXML.SelectNodes("OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/CancelPenalties/CancelPenalty")

                    Dim oCancellation As New Cancellation

                    Dim iTotalAmount As Decimal = oXML.SelectSingleNode("OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/Total/@AmountAfterTax").InnerText.ToSafeMoney()
                    Dim iDailyAmount As Decimal = iTotalAmount / PropertyDetails.Duration
                    Dim iPercentage As Integer = oXMLNode.SelectSingleNode("AmountPercent/@Percent").InnerText.ToSafeInt()
                    Dim iNumberOfNights As Integer = SafeNodeValue(oXMLNode, "AmountPercent/@NmbrOfNights").tosafeint()

                    If (oXMLNode.SelectSingleNode("Deadline/@OffsetDropTime").InnerText = "BeforeArrival") = True Then

                        'Get amount
                        If oXMLNode.SelectSingleNode("AmountPercent/@Percent") IsNot Nothing Then
                            If iNumberOfNights > 0 Then
                                oCancellation.Amount = (0.01 * iPercentage * iDailyAmount * iNumberOfNights).ToSafeMoney()
                            Else
                                'full basis
                                oCancellation.Amount = (0.01 * iPercentage * iTotalAmount).ToSafeMoney()
                            End If


                        Else
                            If oXMLNode.SelectSingleNode("AmountPercent/@Amount") IsNot Nothing Then
                                oCancellation.Amount = oXMLNode.SelectSingleNode("AmountPercent/@Amount").InnerText.ToSafeMoney()

                            End If
                        End If

                        'get end date of cancelpenalty
                        oCancellation.EndDate = PropertyDetails.ArrivalDate

                        'get start date of cancelpenalty
                        Dim iDays As Integer

                        If oXMLNode.SelectSingleNode("Deadline/@OffsetTimeUnit").InnerText = "Day" Then

                            iDays = oXMLNode.SelectSingleNode("Deadline/@OffsetUnitMultiplier").InnerText.ToSafeInt()

                            oCancellation.StartDate = oCancellation.EndDate.AddDays(iDays * -1)

                            'If 'afterbooking' overlaps 'beforearrival', start beforearrival day after afterbooking finishes
                            If oXMLNode.SelectSingleNode("Deadline/@OffsetDropTime[AfterBooking]") IsNot Nothing Then

                                Dim AfterBookingEndDate As Date
                                AfterBookingEndDate = Date.Now.AddDays(SafeTypeExtensions.ToSafeInt(oXMLNode.SelectSingleNode("Deadline/@OffsetMultiplier").InnerText))

                                If AfterBookingEndDate >= oCancellation.StartDate Then
                                    oCancellation.StartDate = AfterBookingEndDate.AddDays(1)
                                End If

                            End If
                        End If
                    End If

                    oCancellations.Add(oCancellation)

                Next

                'need to make it so do not overlap; if they do, put end date as being start date of next
                For i As Integer = 0 To oCancellations.Count - 1
                    If i <> 0 Then
                        If oCancellations(i - 1).EndDate >= oCancellations(i).StartDate Then
                            oCancellations(i - 1).EndDate = oCancellations(i).StartDate.AddDays(-1)
                        End If
                    End If
                Next

            End If
            oCancellations.Solidify(SolidifyType.Sum)

        Catch ex As Exception
            PropertyDetails.Warnings.AddNew("Get Cancellations Exception", ex.ToString)
        End Try

        Return oCancellations

    End Function


    Public Function GetCancellationCost(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost

        Dim oResult As New ThirdPartyCancellationFeeResult
        Dim oCancelCostRequest As New XmlDocument
        Dim oCancelCostResponse As New XmlDocument

        Try

            'build the request
            Dim sbGetFees As New StringBuilder
            Dim sCountry As String = ""

            Dim drCountry As DataRow =
             SQL.GetDataRow("select Country from TPProperty inner join PropertyBooking on TPProperty.TPPropertyID = PropertyBooking.PropertyID where PropertyBooking.PropertyBookingID = {0}",
                            PropertyDetails.PropertyBookingID)

            '*********DON'T WANT THIS CONDITIONAL; SHOULD ALWAYS PICK UP COUNTRY. CHECK BEFORE LIVE THAT THIS WORKS
            If drCountry IsNot Nothing Then
                sCountry = SafeString(drCountry("Country"))
            End If

            'Build cancellationcost XML
            With sbGetFees

                .Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
                .Append("<OTA_CancelRQ xmlns=""http://www.opentravel.org/OTA/2003/05"" Version=""0.1"" CancelType=""Quote"">")
                .Append(GeneratePosTag(PropertyDetails))
                If PropertyDetails.SourceSecondaryReference <> "" Then
                    .AppendFormat("<UniqueID Type=""14"" ID=""{0}"" ID_Context=""Internal""/>", PropertyDetails.SourceSecondaryReference)
                Else
                    .AppendFormat("<UniqueID Type=""14"" ID=""{0}"" ID_Context=""Internal""/>", PropertyDetails.SourceReference)
                End If

                .Append("</OTA_CancelRQ>")

            End With

            'Send cancellation request to MTS and log
            oCancelCostRequest.LoadXml(sbGetFees.ToString)


            'get the add response 
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.BaseURL(PropertyDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.MTS
                .ContentType = ContentTypes.Application_x_www_form_urlencoded
                .LogFileName = "Cancellation Costs"
                .CreateLog = True
                .SetRequest(oCancelCostRequest)
                .Send(_httpClient)
            End With

            oCancelCostResponse = oWebRequest.ResponseXML
            oCancelCostResponse.LoadXml(oCancelCostResponse.InnerXml.Replace(" xmlns=""http://www.opentravel.org/OTA/2003/05""", ""))


            If oCancelCostResponse.SelectSingleNode("OTA_CancelRS/CancelInfoRS/CancelRules/CancelRule/@Amount") IsNot Nothing Then

                'get the cancellation cost
                oResult.Amount = oCancelCostResponse.SelectSingleNode("OTA_CancelRS/CancelInfoRS/CancelRules/CancelRule/@Amount").InnerText.ToSafeMoney()
                oResult.CurrencyCode = oCancelCostResponse.SelectSingleNode("OTA_CancelRS/CancelInfoRS/CancelRules/CancelRule/@CurrencyCode").InnerText.ToSafeString()
                oResult.Success = True

            End If

        Catch ex As Exception

            oResult.Success = False
            PropertyDetails.Warnings.AddNew("Cancellation Cost Exception", ex.ToString)

        Finally

            If oCancelCostRequest.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Pre-Cancel Request", oCancelCostRequest)
            End If

            If oCancelCostResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Pre-Cancel Response", oCancelCostResponse)
            End If

        End Try

        Return oResult

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


#Region "Request Components"

    Private Function GeneratePosTag(oPropertyDetails As PropertyDetails, Optional sCountry As String = "") As String

        Dim overrideCountries As List(Of String) = GetOverrideCountries(oPropertyDetails)
        Dim sbPosTag As New StringBuilder
        Dim sId As String

        If sCountry = "" Then
            If Not String.IsNullOrEmpty(oPropertyDetails.Rooms(0).ThirdPartyReference) Then
                sCountry = oPropertyDetails.Rooms(0).ThirdPartyReference.Split("|"c)(2)
            Else
                If Not String.IsNullOrEmpty(oPropertyDetails.ResortCode) Then
                    sCountry = oPropertyDetails.ResortCode.Split("|"c)(0)
                End If
            End If
        End If
        If overrideCountries.Contains(sCountry) AndAlso sCountry <> "" Then
            sId = _settings.OverRideID(oPropertyDetails)
        Else
            sId = _settings.ID(oPropertyDetails)
        End If

        With sbPosTag
            .Append("<POS>")
            .Append("<Source>")
            .AppendFormat("<RequestorID ID_Context = ""{0}"" ID = ""{1}"" Type = ""{2}""/>",
                      _settings.ID_Context(oPropertyDetails),
                      sId, _settings.Type(oPropertyDetails))
            .Append("<BookingChannel Type = ""2""/>")
            .Append("</Source>")
            .Append("<Source>")
            .AppendFormat("<RequestorID Type=""{0}"" ID=""{1}"" MessagePassword=""{2}""/>",
                      _settings.AuthenticationType(oPropertyDetails),
                      _settings.AuthenticationID(oPropertyDetails),
                      _settings.MessagePassword(oPropertyDetails))
            .Append("</Source>")
            .Append("</POS>")
        End With
        Return sbPosTag.ToString()
    End Function

#End Region

End Class
