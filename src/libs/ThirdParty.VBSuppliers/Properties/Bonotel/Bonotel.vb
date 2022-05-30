Imports Intuitive
Imports Intuitive.Net.WebRequests
Imports Intuitive.Functions
Imports Intuitive.XMLFunctions
Imports System.Text
Imports System.Xml
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Models
Imports System.DateTime
Imports Intuitive.Helpers.Extensions

Public Class Bonotel
    Implements IThirdParty

#Region "Constructor"

    Public Sub New(settings As IBonotelSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

#End Region

#Region "Properties"

    Private ReadOnly Property _settings As IBonotelSettings

    Private ReadOnly Property _support As ITPSupport

    Public Function SupportsLiveCancellation(searchDetails As IThirdPartyAttributeSearch, source As String) As Boolean Implements IThirdParty.SupportsLiveCancellation
        Return _settings.AllowCancellations(searchDetails, False)
    End Function

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

    Private Function TakeSavingFromCommissionMargin(searchDetails As IThirdPartyAttributeSearch) As Boolean Implements IThirdParty.TakeSavingFromCommissionMargin
        Return False
    End Function

    Private Function OffsetCancellationDays(searchDetails As IThirdPartyAttributeSearch) As Integer Implements IThirdParty.OffsetCancellationDays
        Return _settings.OffsetCancellationDays(searchDetails, False)
    End Function

    Private Function RequiresVCard(info As VirtualCardInfo) As Boolean Implements IThirdParty.RequiresVCard
        Return False
    End Function

#End Region

#Region "PreBook"

    'We don't have a prebook in their interface so just calculate the costs so that this works in the xml gateway
    Public Function PreBook(ByVal PropertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook

        Dim iSalesChannelId As Integer = PropertyDetails.SalesChannelID
        Dim iBrandId As Integer = PropertyDetails.BrandID

        'Get Cancelation Policy
        Dim bSuccess As Boolean = CalculateCancellationPolicy(PropertyDetails)

        PropertyDetails.LocalCost = PropertyDetails.Rooms.Sum(Function(r) r.LocalCost)

        Return bSuccess

    End Function

#End Region

#Region "Book"

    Public Function Book(ByVal PropertyDetails As PropertyDetails) As String Implements IThirdParty.Book

        Dim oRequest As New XmlDocument
        Dim oResponse As New XmlDocument
        Dim sReference As String = ""

        PropertyDetails.LocalCost = PropertyDetails.Rooms.Sum(Function(r) r.LocalCost)

        Try

            Dim oBaseHelper As New TPReference(PropertyDetails.Rooms(0).ThirdPartyReference)

            Dim sb As New StringBuilder

            sb.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
            sb.Append("<reservationRequest returnCompeleteBookingDetails=""Y"">")
            sb.Append("<control>")
            sb.AppendFormat("<userName>{0}</userName>", _settings.Username(PropertyDetails))
            sb.AppendFormat("<passWord>{0}</passWord>", _settings.Password(PropertyDetails))
            sb.Append("</control>")

            sb.AppendFormat("<reservationDetails timeStamp=""{0}"">", Now.ToString("yyyymmddThh:mm:ss"))

            sb.AppendFormat("<confirmationType>CON</confirmationType>")
            sb.AppendFormat("<tourOperatorOrderNumber>{0}</tourOperatorOrderNumber>", Now.ToString("yyyymmddThh:mm:ss"))
            sb.AppendFormat("<checkIn>{0}</checkIn>", PropertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"))
            sb.AppendFormat("<checkOut>{0}</checkOut>", PropertyDetails.DepartureDate.ToString("dd-MMM-yyyy"))
            sb.AppendFormat("<noOfRooms>{0}</noOfRooms>", PropertyDetails.Rooms.Count)
            sb.AppendFormat("<noOfNights>{0}</noOfNights>", PropertyDetails.Duration)
            sb.AppendFormat("<hotelCode>{0}</hotelCode>", PropertyDetails.TPKey)
            sb.AppendFormat("<total currency=""{0}"">{1}</total>", oBaseHelper.CurrencyCode, PropertyDetails.LocalCost)
            sb.AppendFormat("<totalTax currency=""{0}"">{1}</totalTax>", oBaseHelper.CurrencyCode, oBaseHelper.TotalTax)

            Dim iRoomNumber As Integer = 1
            For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

                oBaseHelper = New TPReference(oRoomDetails.ThirdPartyReference)

                'room
                sb.Append("<roomData>")
                sb.AppendFormat("<roomNo>{0}</roomNo>", iRoomNumber)
                sb.AppendFormat("<roomCode>{0}</roomCode>", oBaseHelper.RoomCode)
                sb.AppendFormat("<roomTypeCode>{0}</roomTypeCode>", oBaseHelper.RoomTypeCode)
                sb.AppendFormat("<bedTypeCode>{0}</bedTypeCode>", oBaseHelper.BedTypeCode)
                sb.AppendFormat("<ratePlanCode>{0}</ratePlanCode>", oBaseHelper.MealBasis)
                sb.AppendFormat("<noOfAdults>{0}</noOfAdults>", oRoomDetails.Passengers.TotalAdults)
                sb.AppendFormat("<noOfChildren>{0}</noOfChildren>", oRoomDetails.Passengers.TotalChildren + oRoomDetails.Passengers.TotalInfants)
                sb.Append("<occupancy>")

                'guest
                For Each oPassenger As Passenger In oRoomDetails.Passengers

                    sb.Append("<guest>")
                    sb.AppendFormat("<title>{0}</title>", oPassenger.Title)
                    sb.AppendFormat("<firstName>{0}</firstName>", oPassenger.FirstName)
                    sb.AppendFormat("<lastName>{0}</lastName>", oPassenger.LastName)
                    If oPassenger.PassengerType = PassengerType.Child OrElse oPassenger.PassengerType = PassengerType.Infant Then
                        sb.AppendFormat("<age>{0}</age>", oPassenger.Age)
                    End If
                    sb.Append("</guest>")

                Next

                sb.Append("</occupancy>")
                sb.Append("</roomData>")

                iRoomNumber += 1

            Next

            sb.Append("<comment>")

            'any comments for the hotel
            For Each oComment As BookingComment In PropertyDetails.BookingComments
                sb.AppendFormat("<hotel>{0}</hotel>", oComment.Text)
            Next

            'any other comments from the customer
            sb.Append("<customer></customer>")
            sb.Append("</comment>")
            sb.Append("</reservationDetails>")
            sb.Append("</reservationRequest>")


            oRequest.LoadXml(sb.ToString)

            'send the request to Bonotel
            'Dim sLoggingType As String = ThirdPartyData.LoggingType(ThirdParties.BONOTEL, PropertyDetails.SalesChannelID, PropertyDetails.BrandID)
            'Dim bCreateLogs As Boolean = sLoggingType <> "None"
            '			Dim bCreateLogs As Boolean = sLoggingType <> "None"

            '#If DEBUG Then
            '			bCreateLogs = True
            '#End If

            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = (_settings.URL(PropertyDetails) & "GetReservation.do")
                .Method = eRequestMethod.POST
                .SetRequest(oRequest)
                .ContentType = ContentTypes.Text_xml
                .Source = ThirdParties.BONOTEL
                .TimeoutInSeconds = _settings.BookTimeout(PropertyDetails)
                .Send(PropertyDetails)
            End With

            oResponse = oWebRequest.ResponseXML


            If oResponse.InnerText <> "" Then
                oResponse.LoadXml(oResponse.InnerXml)
            Else
                sReference = "failed"
            End If

            'Get the reference
            If Not oResponse.SelectSingleNode("/reservationresponse/referenceno") Is Nothing Then
                sReference = oResponse.SelectSingleNode("/reservationresponse/referenceno").InnerText
            ElseIf Not oResponse.SelectSingleNode("/reservationResponse/referenceNo") Is Nothing Then
                sReference = oResponse.SelectSingleNode("/reservationResponse/referenceNo").InnerText
            Else
                sReference = "failed"
            End If

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString)

            sReference = "failed"

        Finally

            'store the request and response xml on the property booking
            If oRequest IsNot Nothing Then
                PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Book Request", oRequest)
            End If

            If oResponse IsNot Nothing Then
                PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Book Response", oResponse)
            End If

        End Try

        Return sReference

    End Function

#End Region

#Region "Cancellation"

    Public Function CancelBooking(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking

        Dim oThirdPartyCancellationResponse As New ThirdPartyCancellationResponse
        Dim oRequest As New XmlDocument
        Dim oResponse As New XmlDocument

        Try

            'Create XML for cancellation request
            Dim sRequest As String
            sRequest = BuildCancellationRequest(PropertyDetails.SourceReference, PropertyDetails)

            'Log request
            oRequest.LoadXml(sRequest)


            '			'Get the logging type
            '			Dim sLoggingType As String = ThirdPartyData.LoggingType(ThirdParties.BONOTEL, PropertyDetails.SalesChannelID, PropertyDetails.BrandID)
            '			Dim bCreateLogs As Boolean = sLoggingType <> "None"

            '#If DEBUG Then
            '			bCreateLogs = True
            '#End If


            'Send the request
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.URL(PropertyDetails) & "GetCancellation.do"
                .Method = eRequestMethod.POST
                .SetRequest(oRequest)
                .ContentType = ContentTypes.Text_xml
                .Source = ThirdParties.BONOTEL
                .CreateLog = PropertyDetails.CreateLogs
                .LogFileName = "Cancellation"
                .Send()
            End With

            oResponse = oWebRequest.ResponseXML


            'Get the reference
            If Not oResponse.SelectSingleNode("cancellationResponse") Is Nothing Then
                If oResponse.SelectSingleNode("cancellationResponse").Attributes("status").Value = "Y" Then

                    oThirdPartyCancellationResponse.TPCancellationReference = oResponse.SelectSingleNode("cancellationResponse/cancellationNo").InnerText
                    oThirdPartyCancellationResponse.Success = True

                End If
            End If

        Catch ex As Exception
            PropertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString)

        Finally

            'Store the request and response xml on the property booking
            If oRequest.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Cancellation Request", oRequest)
            End If

            If oResponse IsNot Nothing Then
                PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Cancellation Response", oResponse)
            End If

        End Try

        Return oThirdPartyCancellationResponse

    End Function

    Public Function GetCancellationCost(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost
        Return New ThirdPartyCancellationFeeResult
    End Function

    Private Function BuildCancellationRequest(ByVal BookingReference As String, ByVal SearchDetails As IThirdPartyAttributeSearch) As String

        Dim sb As New StringBuilder

        sb.Append("<?xml version=""1.0"" encoding=""utf-8"" ?>")
        sb.Append("<cancellationRequest>")
        sb.Append("<control>")
        sb.AppendFormat("<userName>{0}</userName>", _settings.Username(SearchDetails))
        sb.AppendFormat("<passWord>{0}</passWord>", _settings.Password(SearchDetails))
        sb.Append("</control>")
        sb.AppendFormat("<supplierReferenceNo>{0}</supplierReferenceNo>", BookingReference)
        sb.Append("<cancellationReason/>")
        sb.Append("<cancellationNotes/>")
        sb.Append("</cancellationRequest>")

        Return sb.ToString

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


#Region "Third Party Reference Helper"

    Private Class TPReference
        Public RoomCode As String
        Public RoomTypeCode As String
        Public BedTypeCode As String
        Public CurrencyCode As String
        Public MealBasis As String
        Public TotalTax As String

        Public Sub New(ByVal TPReference As String)
            Dim aParts() As String = TPReference.Split("|"c)
            Me.RoomCode = aParts(0)
            Me.RoomTypeCode = aParts(1)
            Me.BedTypeCode = aParts(2)
            Me.CurrencyCode = aParts(3)
            Me.MealBasis = aParts(4)
            Me.TotalTax = aParts(5)
        End Sub
    End Class

#End Region

#Region "Search Hotel Again for Cancellation Charges"

    Public Function CalculateCancellationPolicy(ByVal PropertyDetails As PropertyDetails) As Boolean

        Dim bSuccess As Boolean = True
        Dim oRequest As New XmlDocument
        Dim oResponse As New XmlDocument

        Try

            'Send the request to Bonotel
            '			Dim sLoggingType As String = ThirdPartyData.LoggingType(ThirdParties.BONOTEL, PropertyDetails.BrandID, PropertyDetails.SalesChannelID)
            '			Dim bCreateLogs As Boolean = sLoggingType <> "None"

            '#If DEBUG Then
            '			bCreateLogs = True
            '#End If

            oRequest.LoadXml(GetAvailabilityRequest(PropertyDetails, PropertyDetails))

            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = (_settings.URL(PropertyDetails) & "GetAvailability.do")
                .Method = eRequestMethod.POST
                .Source = ThirdParties.BONOTEL
                .LogFileName = "CancellationCharges"
                .CreateLog = PropertyDetails.CreateLogs
                .SetRequest(oRequest)
                .ContentType = ContentTypes.Text_xml
                .Send()
            End With

            oResponse = oWebRequest.ResponseXML
            For Each oRoomNode As XmlNode In oResponse.SelectNodes("availabilityResponse/hotelList/hotel/roomInformation")
                Dim sFeeType As String = ""
                Dim sFeeIndicator As String = ""
                Dim sErrataTitle As String = ""
                Dim sErrataDescription As String = ""
                If oRoomNode.SelectNodes("rateInformation/hotelFees/hotelFee").Count > 0 Then
                    For Each oHotelFeeNode As XmlNode In oRoomNode.SelectNodes("rateInformation/hotelFees/hotelFee")
                        sFeeType = oHotelFeeNode.SelectSingleNode("feeType").InnerText
                        Dim sRequiredFee As String = oHotelFeeNode.SelectSingleNode("requiredFee").InnerText
                        Dim sFeeMethod As String = oHotelFeeNode.SelectSingleNode("feeMethod").InnerText
                        sFeeIndicator = GetFeeIndicator(sRequiredFee, sFeeMethod)
                        sErrataTitle = String.Format("{0} Fee - {1}", sFeeType, sFeeIndicator)
                        Dim sAmount As String = oHotelFeeNode.SelectSingleNode("feeTotal").InnerText
                        Dim sConditions As String = oHotelFeeNode.SelectSingleNode("conditions").InnerText
                        sErrataDescription = String.Format("{0}{1}", sAmount, sConditions)
                        PropertyDetails.Errata.AddNew(GetNiceName(sErrataTitle), sErrataDescription)
                    Next
                End If

            Next

            'Transform response
            Dim oResult As New XmlDocument
            oResult = Intuitive.XMLFunctions.XMLStringTransform(oResponse, BonotelRes.BonotelRes.BonotelCancellationPolicy)

            'Take all the policies out of the xsl
            Dim oPolicies As New List(Of CancellationPolicy)

            'Loads All the policies from the xsl into an array of Cancellation Policies
            For Each oPolicy As XmlNode In oResult.SelectNodes("/Results/Policies/Policy")
                Dim oPolicyLoad As New CancellationPolicy
                With oPolicyLoad
                    .AmendmentType = oPolicy.SelectSingleNode("AmendmentType").InnerText
                    .PolicyBasedOn = oPolicy.SelectSingleNode("PolicyBasedOn").InnerText
                    .PolicyBasedOnValue = oPolicy.SelectSingleNode("PolicyBasedOnValue").InnerText
                    .CancellationType = oPolicy.SelectSingleNode("CancellationType").InnerText
                    .StayDateRequirement = oPolicy.SelectSingleNode("StayDateRequirement").InnerText
                    .ArrivalRange = oPolicy.SelectSingleNode("ArrivalRange").InnerText
                    .ArrivalRangeValue = oPolicy.SelectSingleNode("ArrivalRangeValue").InnerText
                    .PolicyFee = oPolicy.SelectSingleNode("PolicyFee").InnerText.Replace("$", "").ToSafeDecimal()

                    .NoShowBasedOn = oPolicy.SelectSingleNode("NoShowPolicy/NoShowBasedOn").InnerText
                    .NoShowBasedOnValue = oPolicy.SelectSingleNode("NoShowPolicy/NoShowBasedOnValue").InnerText
                    .NoShowPolicyFee = oPolicy.SelectSingleNode("NoShowPolicy/NoShowPolicyFee").InnerText.Replace("$", "").ToSafeDecimal()

                End With

                oPolicies.Add(oPolicyLoad)

            Next

            'Loads The Policies into the OverrideSupplierCancellations Class
            Dim oCancellations As New Cancellations

            For Each oPolicy As CancellationPolicy In oPolicies
                Dim bSpecialFlag As Boolean = False

                If oPolicy.AmendmentType = "Cancel" Then

                    'Checks if there is a special policy that overlaps the normal policy
                    If oPolicy.CancellationType = "Special" Then

                        bSpecialFlag = True

                        If oPolicy.ArrivalRange = "Less Than" Then
                            Dim iDays As Integer = oPolicy.ArrivalRangeValue.ToSafeInt

                            oCancellations.AddNew(PropertyDetails.ArrivalDate.AddDays(-iDays),
                                                  PropertyDetails.ArrivalDate.AddDays(-1), oPolicy.PolicyFee)

                            oCancellations.AddNew(PropertyDetails.ArrivalDate, New Date(2099, 1, 1), oPolicy.NoShowPolicyFee)

                        ElseIf oPolicy.ArrivalRange = "Greater Than" Then

                            oCancellations.AddNew(PropertyDetails.ArrivalDate, New Date(2099, 1, 1), oPolicy.NoShowPolicyFee)

                        ElseIf oPolicy.ArrivalRange = "Any" Then

                            oCancellations.AddNew(Now, PropertyDetails.ArrivalDate.AddDays(-1), oPolicy.PolicyFee)

                            oCancellations.AddNew(PropertyDetails.ArrivalDate, New Date(2099, 1, 1), oPolicy.NoShowPolicyFee)

                        End If

                        'Normal Policy
                    ElseIf oPolicy.CancellationType = "Normal" And bSpecialFlag = False Then

                        If oPolicy.ArrivalRange = "Less Than" Then
                            Dim iDays As Integer = oPolicy.ArrivalRangeValue.ToSafeInt

                            oCancellations.AddNew(PropertyDetails.ArrivalDate.AddDays(-iDays),
                                                  PropertyDetails.ArrivalDate.AddDays(-1), oPolicy.PolicyFee)

                            oCancellations.AddNew(PropertyDetails.ArrivalDate, New Date(2099, 1, 1), oPolicy.NoShowPolicyFee)

                        ElseIf oPolicy.ArrivalRange = "Greater Than" Then

                            oCancellations.AddNew(PropertyDetails.ArrivalDate, New Date(2099, 1, 1), oPolicy.NoShowPolicyFee)

                        ElseIf oPolicy.ArrivalRange = "Any" Then

                            oCancellations.AddNew(Now, PropertyDetails.ArrivalDate.AddDays(-1), oPolicy.PolicyFee)

                            oCancellations.AddNew(PropertyDetails.ArrivalDate, New Date(2099, 1, 1), oPolicy.NoShowPolicyFee)

                        End If

                    End If

                End If

                bSpecialFlag = False

            Next

            oCancellations.Solidify(SolidifyType.Sum)
            PropertyDetails.Cancellations = oCancellations

            'check for price changes here
            For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms
                Dim oBaseHelper As New TPReference(oRoomDetails.ThirdPartyReference)

                For Each oRoomNode As XmlNode In oWebRequest.ResponseXML.SelectNodes("availabilityResponse/hotelList/hotel/roomInformation")
                    'find the correct node via room code
                    If SafeNodeValue(oRoomNode, "roomCode") = oBaseHelper.RoomCode And SafeNodeValue(oRoomNode, "rateInformation/ratePlanCode") = oBaseHelper.MealBasis Then
                        Dim nNewPrice As Decimal = SafeMoney(SafeNodeValue(oRoomNode, "rateInformation/totalRate"))
                        If nNewPrice <> oRoomDetails.LocalCost Then
                            oRoomDetails.GrossCost = nNewPrice
                            oRoomDetails.LocalCost = nNewPrice
                        End If
                    End If

                Next
            Next

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Cancellation Costs Exception", ex.ToString)
            bSuccess = False

        Finally

            If oRequest.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Cancellation Cost Request", oRequest)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Cancellation Cost Response", oResponse)
            End If

        End Try

        Return bSuccess

    End Function


    Public Function GetAvailabilityRequest(ByVal SearchDetails As IThirdPartyAttributeSearch, ByVal PropertyDetails As PropertyDetails) As String
        Dim sb As New StringBuilder

        sb.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
        sb.Append("<availabilityRequest cancelpolicy=""Y"" hotelfees=""Y"">")


        sb.Append("<control>")
        sb.AppendFormat("<userName>{0}</userName>", _settings.Username(SearchDetails))
        sb.AppendFormat("<passWord>{0}</passWord>", _settings.Password(SearchDetails))
        sb.Append("</control>")


        sb.AppendFormat("<checkIn>{0}</checkIn>", PropertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"))
        sb.AppendFormat("<checkOut>{0}</checkOut>", PropertyDetails.DepartureDate.ToString("dd-MMM-yyyy"))
        sb.AppendFormat("<noOfRooms>{0}</noOfRooms>", PropertyDetails.Rooms.Count)
        sb.AppendFormat("<noOfNights>{0}</noOfNights>", PropertyDetails.Duration)
        sb.AppendFormat("<hotelCodes>")
        sb.AppendFormat("<hotelCode>{0}</hotelCode>", PropertyDetails.TPKey)
        sb.AppendFormat("</hotelCodes>")


        sb.AppendFormat("<roomsInformation>")

        Dim iRoomNumber As Integer = 1
        For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

            Dim oBaseHelper As New TPReference(oRoomDetails.ThirdPartyReference)

            sb.AppendFormat("<roomInfo>")
            sb.AppendFormat("<roomTypeId>{0}</roomTypeId>", oBaseHelper.RoomTypeCode)
            sb.AppendFormat("<bedTypeId>{0}</bedTypeId> ", oBaseHelper.BedTypeCode)
            sb.AppendFormat("<adultsNum>{0}</adultsNum>", oRoomDetails.Passengers.TotalAdults)
            sb.AppendFormat("<childNum>{0}</childNum>", oRoomDetails.Passengers.TotalChildren + oRoomDetails.Passengers.TotalInfants)

            If oRoomDetails.Passengers.TotalChildren + oRoomDetails.Passengers.TotalInfants > 0 Then

                sb.AppendFormat("<childAges>")

                For Each oPassenger As Passenger In oRoomDetails.Passengers

                    If oPassenger.PassengerType = PassengerType.Child OrElse oPassenger.PassengerType = PassengerType.Infant Then
                        sb.AppendFormat("<childAge>{0}</childAge>", oPassenger.Age)
                    End If
                Next

                sb.AppendFormat("</childAges>")

            End If

            sb.AppendFormat("</roomInfo>")

        Next

        sb.AppendFormat("</roomsInformation>")
        sb.Append("</availabilityRequest>")

        Return sb.ToString

    End Function

    Public Function GetFeeIndicator(ByVal FeeIndicator As String, ByVal FeeMethod As String) As String
        Dim sIndicator As String = ""
        If FeeIndicator = "No" Then
            sIndicator = "Optional"
        ElseIf FeeMethod = "Exclusive" Then
            sIndicator = "Payable Locally"
        ElseIf FeeMethod = "Inclusive" Then
            sIndicator = "Included"
        End If
        Return sIndicator
    End Function

    Public Class CancellationPolicy

        Public AmendmentType As String
        Public PolicyBasedOn As String
        Public PolicyBasedOnValue As String
        Public CancellationType As String
        Public StayDateRequirement As String
        Public ArrivalRange As String
        Public ArrivalRangeValue As String
        Public PolicyFee As Decimal

        Public NoShowBasedOn As String
        Public NoShowBasedOnValue As String
        Public NoShowPolicyFee As Decimal

    End Class

#End Region

End Class


