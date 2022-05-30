Imports System.Xml
Imports Intuitive
Imports Intuitive.Net.WebRequests
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.Abstractions.Models
Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports System.Text


Public Class DOTW
    Implements IThirdParty

#Region "Constructor"

    Private _settings As IDOTWSettings

    Private _support As ITPSupport

    Public Sub New(settings As IDOTWSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

#End Region

#Region "Properties"

    Private Function SupportsLiveCancellation(searchDetails As IThirdPartyAttributeSearch, source As String) As Boolean Implements IThirdParty.SupportsLiveCancellation
        Return _settings.AllowCancellations(searchDetails)
    End Function

    Public ReadOnly Property SupportsRemarks As Boolean Implements IThirdParty.SupportsRemarks
        Get
            Return False
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

    Public Function RequiresVCard(info As VirtualCardInfo) As Boolean Implements IThirdParty.RequiresVCard
        Return False
    End Function

#End Region

#Region "PreBook"

    Public Function PreBook(ByVal PropertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook

        Try

            Me.GetAllocationReferences(PropertyDetails)
            Me.BlockRooms(PropertyDetails)

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString)
            Return False

        End Try

        Return True

    End Function

#Region "Sub Methods of the pre book"

    Private Sub GetAllocationReferences(ByVal PropertyDetails As PropertyDetails)

        'get the room rates so we can get the stupidly long allocationDetails code
        Dim sRequest As String = Me.BuildPreBookRequest(PropertyDetails)
        Dim oRequest As New XmlDocument
        oRequest.LoadXml(sRequest)

        Dim oHeaders As New Intuitive.Net.WebRequests.RequestHeaders
        If _settings.UseGZip(PropertyDetails) Then
            oHeaders.AddNew("Accept-Encoding", "gzip")
        End If


        'Get the response 
        Dim oWebRequest As New Request
        With oWebRequest
            .EndPoint = _settings.ServerURL(PropertyDetails)
            .Method = eRequestMethod.POST
            .Source = ThirdParties.DOTW
            .Headers = oHeaders
            .LogFileName = "Prebook Room"
            .SetRequest(sRequest)
            .ContentType = ContentTypes.Text_xml
            .CreateLog = True
            .Send()
        End With

        Dim oResponse As XmlDocument
        oResponse = oWebRequest.ResponseXML

        oResponse.InnerXml = DOTWSupport.StripNameSpaces(oResponse.InnerXml)


        'check for a valid response
        Dim oSuccessNode As XmlNode = oResponse.SelectSingleNode("result/successful")
        If oSuccessNode Is Nothing OrElse oSuccessNode.InnerText <> "TRUE" Then
            Throw New Exception("booking response does not return success")
        End If


        'loop through each room and get the relevant allocationDetails string and append to TPReference
        Dim iRoomRunNo As Integer = 0
        For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

            'get the room code and meal basis for the predicate
            Dim sRoomTypeCode As String = oRoomDetails.ThirdPartyReference.Split("|"c)(0)
            Dim sMealBasis As String = oRoomDetails.ThirdPartyReference.Split("|"c)(1)

            'build the predicate
            Dim sPredicate As String =
             String.Format("/result/hotel/rooms/room[@runno='{0}']/roomType[@roomtypecode='{1}']/rateBases/rateBasis[@id='{2}']/",
              iRoomRunNo, sRoomTypeCode, sMealBasis)

            'grab the allocationDetails using the predicate
            Dim oAllocationNode As XmlNode = oResponse.SelectSingleNode(sPredicate & "allocationDetails")
            If oAllocationNode Is Nothing Then Throw New Exception("Allocation Details could not be found in prebook")

            'assign the allocation details to the TPReference
            oRoomDetails.ThirdPartyReference &= "|" & oAllocationNode.InnerText

            'increment for each room
            iRoomRunNo += 1

            'add errata
            Dim sTariffNotes As String = XMLFunctions.SafeNodeValue(oResponse, sPredicate & "tariffNotes")
            If sTariffNotes <> "" Then
                PropertyDetails.Errata.AddNew("Important Information", sTariffNotes)
            End If

        Next

        'store the request and response xml on the property booking
        PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Availability Request", sRequest)
        PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Availability Response", oResponse)

    End Sub

    Private Sub BlockRooms(ByVal PropertyDetails As PropertyDetails)

        'block the rooms
        Dim sRequest As String = Me.BuildBlockRequest(PropertyDetails)
        Dim oRequest As New XmlDocument
        oRequest.LoadXml(sRequest)

        Dim oHeaders As New Intuitive.Net.WebRequests.RequestHeaders
        If _settings.UseGZip(PropertyDetails) Then
            oHeaders.AddNew("Accept-Encoding", "gzip")
        End If


        'Get the response 
        Dim oWebRequest As New Request
        With oWebRequest
            .EndPoint = _settings.ServerURL(PropertyDetails)
            .Method = eRequestMethod.POST
            .Source = ThirdParties.DOTW
            .Headers = oHeaders
            .LogFileName = "Prebook Block Room"
            .SetRequest(sRequest)
            .ContentType = ContentTypes.Text_xml
            .CreateLog = True
            .Send()
        End With

        Dim oResponse As XmlDocument
        oResponse = oWebRequest.ResponseXML
        oResponse.InnerXml = DOTWSupport.StripNameSpaces(oResponse.InnerXml)


        'check for a valid response
        Dim oSuccessNode As XmlNode = oResponse.SelectSingleNode("result/successful")
        If oSuccessNode Is Nothing OrElse oSuccessNode.InnerText <> "TRUE" Then
            Throw New Exception("block response does not return success")
        End If


        Dim iRoomRunNo As Integer = 0
        Dim nNewLocalCost As Decimal = 0

        'loop through each room and get the relevant allocationDetails string and append to TPReference
        For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

            'get the room code and meal basis for the predicate
            Dim sRoomTypeCode As String = oRoomDetails.ThirdPartyReference.Split("|"c)(0)
            Dim sMealBasis As String = oRoomDetails.ThirdPartyReference.Split("|"c)(1)

            Dim sCurrentAllocationDetails As String = oRoomDetails.ThirdPartyReference.Split("|"c)(2)

            'We get one status = checked per room booked. This is the one we want. Use it. Please.
            Dim sPredicate As String = String.Format("/result/hotel/rooms/room[@runno='{0}']/roomType[@roomtypecode='{1}']/rateBases/rateBasis[@id='{2}'][status='checked']",
              iRoomRunNo, sRoomTypeCode, sMealBasis)

            Dim oResultNode As XmlNode = oResponse.SelectSingleNode(sPredicate)

            If oResultNode Is Nothing Then Throw New Exception("room type could not be blocked")

            'grab the allocationDetails using the predicate
            Dim oAllocationNode As XmlNode = oResultNode.SelectSingleNode("allocationDetails")
            If oAllocationNode Is Nothing Then Throw New Exception("Allocation Details could not be found in prebook")


            'assign the allocation details to the TPReference
            oRoomDetails.ThirdPartyReference = oRoomDetails.ThirdPartyReference.Replace(sCurrentAllocationDetails, oAllocationNode.InnerText)

            'Check for Price Changes for each room booking		
            Dim nRoomCost As Decimal = 0
            Try
                nRoomCost = oResultNode.SelectSingleNode("total/formatted").InnerText.ToSafeDecimal()

                If _settings.UseMinimumSellingPrice(PropertyDetails) AndAlso
                 Not oResultNode.SelectSingleNode("totalMinimumSelling/formatted") Is Nothing Then
                    nRoomCost = oResultNode.SelectSingleNode("totalMinimumSelling/formatted").InnerText.ToSafeDecimal()
                End If
            Catch ex As Exception
                'probably no minimumselling price listed, ignore exception and use the normal total 
            End Try

            If nRoomCost <> oRoomDetails.LocalCost AndAlso nRoomCost <> 0 Then

                oRoomDetails.LocalCost = nRoomCost
                oRoomDetails.GrossCost = nRoomCost

            End If

            nNewLocalCost = nNewLocalCost + oRoomDetails.LocalCost


            'increment the roomrunno 
            iRoomRunNo += 1

        Next

        'have to recalculate costs after price changes or cancellations will use the wrong cost!!!
        If PropertyDetails.LocalCost <> nNewLocalCost Then
            PropertyDetails.LocalCost = nNewLocalCost
        End If

        'get the cancellation policy
        PropertyDetails.Cancellations.AddRange(Me.GetCancellationPolicy(PropertyDetails, oResponse))

        'store the request and response xml on the property booking
        PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Pre-Book Request", sRequest)
        PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Pre-Book Response", oResponse)

    End Sub

#End Region

#End Region

#Region "Book"

    Public Function Book(ByVal PropertyDetails As PropertyDetails) As String Implements IThirdParty.Book

        Dim sReturnReference As String = ""
        Dim oResponse As New XmlDocument
        Dim oRequest As New XmlDocument

        Try

            'build request
            Dim sRequest As String = Me.BuildBookingRequest(PropertyDetails)

            Dim oHeaders As New Intuitive.Net.WebRequests.RequestHeaders
            If _settings.UseGZip(PropertyDetails) Then
                oHeaders.AddNew("Accept-Encoding", "gzip")
            End If

            'Get the response 
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.ServerURL(PropertyDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.DOTW
                .Headers = oHeaders
                .LogFileName = "Book"
                .SetRequest(sRequest)
                .ContentType = ContentTypes.Text_xml
                .CreateLog = True
                .Send(PropertyDetails)
            End With

            oRequest.LoadXml(sRequest)
            oResponse = oWebRequest.ResponseXML

            'check according to documentation that there is a success node with the value TRUE in it
            Dim oSuccessNode As XmlNode = oResponse.SelectSingleNode("result/successful")
            If oSuccessNode Is Nothing OrElse oSuccessNode.InnerText <> "TRUE" Then
                Throw New Exception("booking response does not return success")
            End If

            'now get booking nodes
            Dim oBookings As XmlNode = oResponse.SelectSingleNode("result/returnedCode")

            If Not oBookings Is Nothing Then

                'concatenate the various references for each room component into a booking comment
                Dim oReferenceNodeList As XmlNodeList = oResponse.SelectNodes("/result/bookings/booking/bookingReferenceNumber")

                If oReferenceNodeList.Count > 1 Then

                    'create a booking comment on this property booking with all the room references in it.
                    Dim oSBReferences As New StringBuilder
                    oSBReferences.Append("DOTW room booking references ")

                    For iNode As Integer = 0 To oReferenceNodeList.Count - 1
                        oSBReferences.Append(oReferenceNodeList(iNode).InnerText)
                        If iNode <> oReferenceNodeList.Count - 1 Then oSBReferences.Append(", ")
                    Next

                    PropertyDetails.BookingComments.AddNew(oSBReferences.ToString)

                End If

                'get the reference of the booking which is displayed on their website as a reference
                sReturnReference = oReferenceNodeList(0).InnerText
                PropertyDetails.SourceSecondaryReference = oResponse.SelectSingleNode("result/returnedCode").InnerText

            Else
                Throw New Exception("no bookings found in booking response xml")
            End If

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString)
            sReturnReference = "failed"

        Finally

            'store the request and response xml on the property booking
            If oRequest.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Book Request", oRequest)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Book Response", oResponse)
            End If

        End Try

        Return sReturnReference

    End Function


#End Region

#Region "Get Cancellation Policy"

    Public Function GetCancellationPolicy(ByVal PropertyDetails As PropertyDetails, ByVal oRoomXML As XmlDocument) As Cancellations

        'create an array variable to hold the policy for each room
        Dim aPolicies(PropertyDetails.Rooms.Count - 1) As Cancellations


        'loop through the rooms
        Dim iLoop As Integer = 0
        For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

            'get the room code and meal basis for the predicate
            Dim sRoomTypeCode As String = oRoomDetails.ThirdPartyReference.Split("|"c)(0)
            Dim sMealBasis As String = oRoomDetails.ThirdPartyReference.Split("|"c)(1)

            'build the predicate
            Dim sPredicate As String =
             String.Format("/result/hotel/rooms/room[@runno='{0}']/roomType[@roomtypecode='{1}']/rateBases/rateBasis[@id='{2}']",
              iLoop, sRoomTypeCode, sMealBasis)


            'get the cancellation deadline if it exists - we have to strip this out of a text string because dotw are idiots
            Dim dCancellationDeadline As Date = DateTimeExtensions.EmptyDate

            Dim oCancellationNode As XmlNode = oRoomXML.SelectSingleNode(sPredicate & "/cancellation")
            If oCancellationNode IsNot Nothing AndAlso oCancellationNode.InnerText.StartsWith("Cancellation Deadline: ") Then
                dCancellationDeadline = oCancellationNode.InnerText.Substring(23).Replace(" hrs", "").ToSafeDate().Date
            End If

            'Dim oTotalCostNode As XmlNode = oRoomXML.SelectSingleNode(sPredicate & "/total")

            'add the rules into the policy for this room
            'in version 2 they have added a no show policy element for some of the properties which doesn't have a to or from date just a charge so we will add our or dates
            'as we go through if this is case
            aPolicies(iLoop) = New Cancellations

            For Each oRuleNode As XmlNode In oRoomXML.SelectNodes(sPredicate & "/cancellationRules/rule")
                Dim oFromDateNode As XmlNode = oRuleNode.SelectSingleNode("fromDate")
                Dim oToDateNode As XmlNode = oRuleNode.SelectSingleNode("toDate")
                Dim oAmountNode As XmlNode = oRuleNode.SelectSingleNode("charge")

                Dim bNonRefundable As Boolean = False
                If oRuleNode.SelectSingleNode("cancelRestricted") IsNot Nothing Then
                    bNonRefundable = oRuleNode.SelectSingleNode("cancelRestricted").InnerText.ToSafeBoolean()
                End If

                Dim bNoShowPolicy As Boolean = False
                If oRuleNode.SelectSingleNode("noShowPolicy") IsNot Nothing Then
                    bNoShowPolicy = oRuleNode.SelectSingleNode("noShowPolicy").InnerText.ToSafeBoolean()
                End If

                'get the start date
                Dim dStartDate As Date
                If oFromDateNode IsNot Nothing Then
                    'these always come back with the time as HH:mm:01 but the end dates come back as HH:mm:00, so I'm taking off a second
                    dStartDate = oFromDateNode.InnerText.ToSafeDate().AddSeconds(-1).Date
                ElseIf bNoShowPolicy = True Then
                    dStartDate = PropertyDetails.ArrivalDate.Date
                Else
                    dStartDate = Date.Now
                End If

                'get the end date
                Dim dEndDate As Date
                If oToDateNode IsNot Nothing Then
                    dEndDate = oToDateNode.InnerText.ToSafeDate().Date.AddDays(-1) 'take off a day so our date bands don't overlap
                Else
                    dEndDate = If(dCancellationDeadline <> DateTimeExtensions.EmptyDate, dCancellationDeadline, New Date(2099, 12, 31))
                End If

                'get the amount
                Dim nAmount As Decimal = 0
                If oAmountNode IsNot Nothing Then
                    nAmount = oAmountNode.FirstChild.Value.ToSafeDecimal()
                ElseIf bNonRefundable Then
                    nAmount = oRoomDetails.LocalCost
                End If


                'add the rule into the policy
                aPolicies(iLoop).AddNew(dStartDate, dEndDate, nAmount)

            Next


            'call solidify on the policy
            aPolicies(iLoop).Solidify(SolidifyType.Sum, New Date(2099, 12, 31), oRoomDetails.LocalCost)


            'increment the loop counter 
            iLoop += 1

        Next


        'merge the policies and return
        Return Cancellations.MergeMultipleCancellationPolicies(aPolicies)

    End Function

#End Region

#Region "Cancellation"

    Public Function CancelBooking(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking

        Dim oThirdPartyCancellationResponse As New ThirdPartyCancellationResponse

        Dim sCancellationReference As String = ""
        Dim sRequest As String = ""
        Dim oResponse As New XmlDocument
        Dim sCanxRequest As String = ""
        Dim oCanxResponse As New XmlDocument

        Try

            'get costs and service numbers
            'build request
            sRequest = Me.BuildCancellationCostRequest(PropertyDetails.SourceSecondaryReference, PropertyDetails)

            'Get the response 
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.ServerURL(PropertyDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.DOTW
                .LogFileName = "Precancel"
                .SetRequest(sRequest)
                .ContentType = ContentTypes.Text_xml
                .CreateLog = True
                .Send()
            End With

            oResponse = oWebRequest.ResponseXML


            'check according to documentation that there is a success node with the value TRUE in it
            Dim oSuccessNode As XmlNode = oResponse.SelectSingleNode("result/successful")
            If oSuccessNode Is Nothing OrElse oSuccessNode.InnerText <> "TRUE" Then
                Throw New Exception("cancellation request did not return success")
            End If


            'get service numbers
            Dim oCancellationDetails As New Generic.Dictionary(Of String, String)
            oCancellationDetails = Me.GetCancellationDetails(oResponse)


            'now make the actual cancellation
            'build request
            sCanxRequest = Me.BuildCancellationRequest(PropertyDetails.SourceSecondaryReference,
             PropertyDetails.CancellationAmount, PropertyDetails, oCancellationDetails)

            'Get the response 
            Dim oCancellationWebRequest As New Request
            With oCancellationWebRequest
                .EndPoint = _settings.ServerURL(PropertyDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.DOTW
                .LogFileName = "Cancel"
                .SetRequest(sCanxRequest)
                .ContentType = ContentTypes.Text_xml
                .CreateLog = True
                .Send()
            End With

            oCanxResponse = oCancellationWebRequest.ResponseXML

            'check according to documentation that there is a success node with the value TRUE in it
            oSuccessNode = oCanxResponse.SelectSingleNode("result/successful")
            If oSuccessNode Is Nothing OrElse oSuccessNode.InnerText <> "TRUE" Then
                Throw New Exception("cancellation request did not return success")
            End If

            If SafeTypeExtensions.ToSafeInt(_settings.Version(PropertyDetails)) = 2 Then
                Dim oProductsLeft As XmlNode = oCanxResponse.SelectSingleNode("result/productsLeftOnItinerary ")
                If oProductsLeft IsNot Nothing AndAlso SafeTypeExtensions.ToSafeInt(oProductsLeft.InnerText) <> 0 Then
                    Throw New Exception("cancellation request did not cancel all components")
                End If
            End If

            'no cancellation reference is given, so use the time stamp as others do.
            oThirdPartyCancellationResponse.TPCancellationReference = DateTime.Now.ToString("yyyyMMddhhmm")
            oThirdPartyCancellationResponse.Success = True

        Catch ex As Exception
            PropertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString)
            oThirdPartyCancellationResponse.TPCancellationReference = ""
            oThirdPartyCancellationResponse.Success = False

        Finally

            'store the request and response xml on the property booking
            If sRequest <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW PreCancellation Request", sRequest)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW PreCancellation Response", oResponse)
            End If

            If sCanxRequest <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Cancellation Request", sCanxRequest)
            End If

            If oCanxResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Cancellation Response", oCanxResponse)
            End If

        End Try

        Return oThirdPartyCancellationResponse

    End Function

    Private Function GetCancellationDetails(ByVal oXML As XmlDocument) As Dictionary(Of String, String)

        Dim oCancellationDetails As New Dictionary(Of String, String)

        For Each oNode As XmlNode In oXML.SelectNodes("result/services/service")
            oCancellationDetails.Add(oNode.Attributes("code").Value.ToSafeString(), oNode.SelectSingleNode("cancellationPenalty/charge/text()").InnerText.ToSafeString())
        Next

        Return oCancellationDetails

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


#Region "Cancellation Cost"

    Public Function GetCancellationCost(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost

        Dim oResult As New ThirdPartyCancellationFeeResult
        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID
        Dim oRequest As New XmlDocument
        Dim oResponse As New XmlDocument

        Try

            'build request
            Dim sRequest As String = Me.BuildCancellationCostRequest(PropertyDetails.SourceSecondaryReference, PropertyDetails)

            oRequest.LoadXml(sRequest)

            'Get the response 
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.ServerURL(PropertyDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.DOTW
                .LogFileName = "Cancellation Cost"
                .SetRequest(sRequest)
                .ContentType = ContentTypes.Text_xml
                .CreateLog = True
                .Send()
            End With

            oResponse = oWebRequest.ResponseXML


            'check according to documentation that there is a success node with the value TRUE in it
            Dim oSuccessNode As XmlNode = oResponse.SelectSingleNode("result/successful")
            If oSuccessNode Is Nothing OrElse oSuccessNode.InnerText <> "TRUE" Then
                Throw New Exception("cancellation cost response does not return success")
            End If


            'get the cancellation cost from this booking
            Dim oCostNodes As XmlNodeList = oResponse.SelectNodes("result/services/service")
            If oCostNodes Is Nothing OrElse oCostNodes.Count = 0 Then
                Throw New Exception("cancellation costs request not in expected format")
            End If

            Dim nAmount As Decimal = 0
            For Each oCostNode As XmlNode In oCostNodes
                nAmount += oCostNode.SelectSingleNode("cancellationPenalty/charge/text()").InnerText.ToSafeDecimal()
            Next

            'grab the currency from the first node
            oResult.Amount = nAmount
            oResult.CurrencyCode = oResponse.SelectSingleNode("/result/services[1]/service/cancellationPenalty/currencyShort").InnerText.ToSafeString()
            oResult.Success = True

        Catch ex As Exception
            PropertyDetails.Warnings.AddNew("Get Cancellation Cost Exception", ex.ToString)

        Finally

            If oRequest.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "Get Cancellation Cost Request", oRequest)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "Get Cancellation Cost Response", oResponse)
            End If

        End Try

        Return oResult

    End Function

#End Region

#Region "Build requests"

    Private Function BuildBookingRequest(ByVal PropertyDetails As PropertyDetails) As String
        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Dim oSB As New StringBuilder

        With oSB
            .AppendLine("<customer>")
            .AppendFormat("<username>{0}</username>", _settings.Username(PropertyDetails)).AppendLine()
            .AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(PropertyDetails))).AppendLine()
            .AppendFormat("<id>{0}</id>", _settings.CompanyCode(PropertyDetails)).AppendLine()
            .AppendLine("<source>1</source>")
            .AppendLine("<product>hotel</product>")
            .AppendLine("<request command=""confirmbooking"">")
            .AppendLine("<bookingDetails>")
            .AppendFormat("<fromDate>{0}</fromDate>", PropertyDetails.ArrivalDate.ToString("yyyy-MM-dd")).AppendLine()
            .AppendFormat("<toDate>{0}</toDate>", PropertyDetails.DepartureDate.ToString("yyyy-MM-dd")).AppendLine()
            .AppendFormat("<currency>{0}</currency>", DOTWSupport.GetCurrencyCode(PropertyDetails.CurrencyID,
              PropertyDetails, _settings))
            .AppendFormat("<productId>{0}</productId>", PropertyDetails.TPKey)

            Dim sCustomerReference As String = PropertyDetails.BookingReference.Trim

            If _settings.SendTradeReference(PropertyDetails) Then
                sCustomerReference = PropertyDetails.TradeReference
            End If

            If sCustomerReference = "" Then
                sCustomerReference = DateTime.Now.ToString("yyyyMMddhhmmss")
            End If

            .AppendFormat("<customerReference>{0}</customerReference>", sCustomerReference)
            .AppendFormat("<rooms no="" {0}"">", PropertyDetails.Rooms.Count)

            Dim iRoomRunNo As Integer = 0
            For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

                Dim sRoomTypeCode As String = oRoomDetails.ThirdPartyReference.Split("|"c)(0)
                Dim sMealBasis As String = oRoomDetails.ThirdPartyReference.Split("|"c)(1)
                Dim sAllocationDetails As String = oRoomDetails.ThirdPartyReference.Split("|"c)(2)

                Dim iAdults As Integer = oRoomDetails.Passengers.TotalAdults
                Dim iChildren As Integer = 0

                For Each oPassenger As Passenger In oRoomDetails.Passengers
                    If oPassenger.PassengerType = PassengerType.Child OrElse oPassenger.PassengerType = PassengerType.Infant Then
                        If oPassenger.Age > 12 Then
                            iAdults += 1
                        Else
                            iChildren += 1
                        End If
                    End If
                Next

                .AppendFormat("<room runno="" {0}"">", iRoomRunNo)
                .AppendFormat("<roomTypeCode>{0}</roomTypeCode>", sRoomTypeCode)
                .AppendFormat("<selectedRateBasis>{0}</selectedRateBasis>", sMealBasis)

                .AppendFormat("<allocationDetails>{0}</allocationDetails>", sAllocationDetails)
                .AppendFormat("<adultsCode>{0}</adultsCode>", iAdults)
                .AppendFormat("<children no="" {0}"">", iChildren)

                Dim iChildAgeRunNo As Integer = 0
                For Each oPassenger As Passenger In oRoomDetails.Passengers
                    If (oPassenger.PassengerType = PassengerType.Child AndAlso oPassenger.Age <= 12) OrElse oPassenger.PassengerType = PassengerType.Infant Then
                        .AppendFormat("<child runno=""{0}"">{1}</child>", iChildAgeRunNo, If(oPassenger.Age = 0, 1, oPassenger.Age))
                        iChildAgeRunNo += 1
                    End If
                Next

                .AppendFormat("</children>")
                .AppendLine("<extraBed>0</extraBed>")
                If _settings.Version(PropertyDetails) = "2" Then
                    Dim sNationality As String = GetNationality(PropertyDetails.NationalityID, PropertyDetails, _support, _settings)
                    Dim sCountryCode As String = GetCountryOfResidence(sNationality, PropertyDetails, _settings)

                    If sNationality <> "" Then
                        .AppendFormatLine("<passengerNationality>{0}</passengerNationality>", sNationality)
                    End If

                    If sCountryCode <> "" Then
                        .AppendFormatLine("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", sCountryCode)
                    End If
                End If
                .AppendLine("<passengersDetails>")

                Dim iGuestRunNo As Integer = 0
                For Each oPassenger As Passenger In oRoomDetails.Passengers

                    'get the guest title
                    Dim iTitleID As Integer

                    If oPassenger.PassengerType = PassengerType.Child OrElse oPassenger.PassengerType = PassengerType.Infant Then
                        iTitleID = DOTWSupport.GetTitleID("Child", PropertyDetails.SalesChannelID)
                    Else
                        iTitleID = DOTWSupport.GetTitleID(oPassenger.Title, PropertyDetails.SalesChannelID)
                    End If

                    .AppendFormat("<passenger leading=""{0}"">", If(iGuestRunNo = 0, "yes", "no"))
                    .AppendFormat("<salutation>{0}</salutation>", iTitleID)
                    .AppendFormat("<firstName>{0}</firstName>", DOTWSupport.CleanName(oPassenger.FirstName, _support))
                    .AppendFormat("<lastName>{0}</lastName>", DOTWSupport.CleanName(oPassenger.LastName, _support))
                    .AppendLine("</passenger>")
                    iGuestRunNo += 1

                Next

                .AppendLine("</passengersDetails>")
                .AppendLine("</room>")

                iRoomRunNo += 1

            Next

            .AppendLine("</rooms>")
            .AppendLine("</bookingDetails>")
            .AppendLine("</request>")
            .AppendLine("</customer>")
        End With

        Return oSB.ToString

    End Function

    Private Function BuildCancellationCostRequest(ByVal BookingReference As String, ByVal SearchDetails As IThirdPartyAttributeSearch) As String

        Dim oSB As New StringBuilder

        With oSB
            .AppendLine("<customer>")
            .AppendFormat("<username>{0}</username>", _settings.Username(SearchDetails)).AppendLine()
            .AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(SearchDetails))).AppendLine()
            .AppendFormat("<id>{0}</id>", _settings.CompanyCode(SearchDetails)).AppendLine()
            .AppendLine("<source>1</source>")
            .AppendLine("<request command=""deleteitinerary"">")
            .AppendLine("<bookingDetails>")
            .AppendLine("<bookingType>1</bookingType>")
            .AppendFormat("<bookingCode>{0}</bookingCode>", BookingReference)
            .AppendLine("<confirm>no</confirm> ")
            .AppendLine("</bookingDetails>")
            .AppendLine("</request>")
            .AppendLine("</customer>")
        End With

        Return oSB.ToString

    End Function

    Private Function BuildCancellationRequest(ByVal BookingReference As String, ByVal CancellationAmount As Decimal,
    ByVal SearchDetails As IThirdPartyAttributeSearch, ByVal CancellationDetails As Generic.Dictionary(Of String, String)) As String

        Dim oSB As New StringBuilder

        With oSB
            .AppendLine("<customer>")
            .AppendFormat("<username>{0}</username>", _settings.Username(SearchDetails)).AppendLine()
            .AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(SearchDetails))).AppendLine()
            .AppendFormat("<id>{0}</id>", _settings.CompanyCode(SearchDetails)).AppendLine()
            .AppendLine("<source>1</source>")
            .AppendLine("<request command=""deleteitinerary"">")
            .AppendLine("<bookingDetails>")
            .AppendLine("<bookingType>1</bookingType>")
            .AppendFormat("<bookingCode>{0}</bookingCode>", BookingReference)
            .AppendLine("<confirm>yes</confirm>")
            .AppendLine("<testPricesAndAllocation>")

            For Each oDetail As Generic.KeyValuePair(Of String, String) In CancellationDetails
                .AppendFormat("<service referencenumber=""{0}"">", oDetail.Key).AppendLine()
                .AppendFormat("<penaltyApplied>{0}</penaltyApplied>", oDetail.Value).AppendLine()
                .AppendLine("</service>")
            Next

            .AppendLine("</testPricesAndAllocation>")
            .AppendLine("</bookingDetails>")
            .AppendLine("</request>")
            .AppendLine("</customer>")
        End With

        Return oSB.ToString

    End Function

    Private Function BuildPreBookRequest(ByVal PropertyDetails As PropertyDetails) As String

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Dim oSB As New StringBuilder

        With oSB
            .AppendLine("<customer>")
            .AppendFormat("<username>{0}</username>", _settings.Username(PropertyDetails))
            .AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(PropertyDetails)))
            .AppendFormat("<id>{0}</id>", _settings.CompanyCode(PropertyDetails))
            .AppendLine("<source>1</source>")
            .AppendLine("<product>hotel</product>")
            .AppendLine("<request command = ""getrooms"">")
            .AppendLine("<bookingDetails>")
            .AppendFormat("<fromDate>{0}</fromDate>", PropertyDetails.ArrivalDate.ToString("yyyy-MM-dd"))
            .AppendFormat("<toDate>{0}</toDate>", PropertyDetails.DepartureDate.ToString("yyyy-MM-dd"))
            .AppendFormat("<currency>{0}</currency>", DOTWSupport.GetCurrencyCode(PropertyDetails.CurrencyID, PropertyDetails, _settings))
            .AppendFormat("<rooms no = ""{0}"">", PropertyDetails.Rooms.Count)

            Dim iRoomRunNo As Integer = 0
            For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

                Dim sRoomTypeCode As String = oRoomDetails.ThirdPartyReference.Split("|"c)(0)
                Dim sMealBasis As String = oRoomDetails.ThirdPartyReference.Split("|"c)(1)

                Dim iAdults As Integer = oRoomDetails.AdultsSetAgeOrOver(13)
                Dim iChildren As Integer = oRoomDetails.ChildrenUnderSetAge(13)

                .AppendFormat("<room runno=""{0}"">", iRoomRunNo)
                .AppendFormat("<adultsCode>{0}</adultsCode>", iAdults)
                .AppendFormat("<children no=""{0}"">", iChildren)

                Dim iChildAgeRunNo As Integer = 0
                For Each iAge As Integer In oRoomDetails.Passengers.ChildAgesUnderSetAge(13)

                    .AppendFormat("<child runno=""{0}"">{1}</child>", iChildAgeRunNo, iAge)
                    iChildAgeRunNo += 1

                Next

                .AppendLine("</children>")
                .AppendLine("<extraBed>0</extraBed>")
                .AppendFormat("<rateBasis>{0}</rateBasis>", sMealBasis)

                If _settings.Version(PropertyDetails) = "2" Then
                    Dim sNationality As String = GetNationality(PropertyDetails.NationalityID, PropertyDetails, _support, _settings)
                    Dim sCountryCode As String = GetCountryOfResidence(sNationality, PropertyDetails, _settings)

                    If sNationality <> "" Then
                        .AppendFormatLine("<passengerNationality>{0}</passengerNationality>", sNationality)
                    End If

                    If sCountryCode <> "" Then
                        .AppendFormatLine("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", sCountryCode)
                    End If
                End If

                .AppendLine("</room>")
                iRoomRunNo += 1

            Next

            .AppendLine("</rooms>")
            .AppendFormat("<productId>{0}</productId>", PropertyDetails.TPKey)
            .AppendLine("</bookingDetails>")
            .AppendLine("</request>")
            .AppendLine("</customer>")

        End With

        Return oSB.ToString

    End Function

    Private Function BuildBlockRequest(ByVal PropertyDetails As PropertyDetails) As String

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Dim oSB As New StringBuilder

        With oSB

            .AppendLine("<customer>")
            .AppendFormat("<username>{0}</username>", _settings.Username(PropertyDetails))
            .AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(PropertyDetails)))
            .AppendFormat("<id>{0}</id>", _settings.CompanyCode(PropertyDetails))
            .AppendLine("<source>1</source>")
            .AppendLine("<product>hotel</product>")
            .AppendLine("<request command = ""getrooms"">")
            .AppendLine("<bookingDetails>")
            .AppendFormat("<fromDate>{0}</fromDate>", PropertyDetails.ArrivalDate.ToString("yyyy-MM-dd"))
            .AppendFormat("<toDate>{0}</toDate>", PropertyDetails.DepartureDate.ToString("yyyy-MM-dd"))
            .AppendFormat("<currency>{0}</currency>", DOTWSupport.GetCurrencyCode(PropertyDetails.CurrencyID, PropertyDetails, _settings))
            .AppendFormat("<rooms no = ""{0}"">", PropertyDetails.Rooms.Count)

            Dim iRoomRunNo As Integer = 0
            For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

                Dim sRoomTypeCode As String = oRoomDetails.ThirdPartyReference.Split("|"c)(0)
                Dim sMealBasis As String = oRoomDetails.ThirdPartyReference.Split("|"c)(1)
                Dim sAllocationDetail As String = oRoomDetails.ThirdPartyReference.Split("|"c)(2)

                Dim iAdults As Integer = oRoomDetails.AdultsSetAgeOrOver(13)
                Dim iChildren As Integer = oRoomDetails.ChildrenUnderSetAge(13)

                .AppendFormat("<room runno=""{0}"">", iRoomRunNo)
                .AppendFormat("<adultsCode>{0}</adultsCode>", iAdults)
                .AppendFormat("<children no=""{0}"">", iChildren)

                Dim iChildAgeRunNo As Integer = 0
                For Each iAge As Integer In oRoomDetails.Passengers.ChildAgesUnderSetAge(13)

                    .AppendFormat("<child runno=""{0}"">{1}</child>", iChildAgeRunNo, iAge)
                    iChildAgeRunNo += 1

                Next

                .AppendLine("</children>")
                .AppendLine("<extraBed>0</extraBed>")
                .AppendLine("<rateBasis>-1</rateBasis>")

                If _settings.Version(PropertyDetails) = "2" Then
                    Dim sNationality As String = GetNationality(PropertyDetails.NationalityID, PropertyDetails, _support, _settings)
                    Dim sCountryCode As String = GetCountryOfResidence(sNationality, PropertyDetails, _settings)

                    If sNationality <> "" Then
                        .AppendFormatLine("<passengerNationality>{0}</passengerNationality>", sNationality)
                    End If

                    If sCountryCode <> "" Then
                        .AppendFormatLine("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", sCountryCode)
                    End If

                End If

                .AppendLine("<roomTypeSelected>")
                .AppendFormat("<code>{0}</code>", sRoomTypeCode)
                .AppendFormat("<selectedRateBasis>{0}</selectedRateBasis>", sMealBasis)
                .AppendFormat("<allocationDetails>{0}</allocationDetails>", sAllocationDetail)
                .AppendLine("</roomTypeSelected>")
                .AppendLine("</room>")

                iRoomRunNo += 1

            Next

            .AppendLine("</rooms>")
            .AppendFormat("<productId>{0}</productId>", PropertyDetails.TPKey)
            .AppendLine("</bookingDetails>")
            .AppendLine("</request>")
            .AppendLine("</customer>")

        End With

        Return oSB.ToString

    End Function

#End Region

#Region "Nationality and country of residence"

    Public Shared Function GetNationality(ByVal NationalityISOCode As String, ByVal SearchDetails As IThirdPartyAttributeSearch, ByVal Support As ITPSupport, ByVal Settings As IDOTWSettings) As String
        Dim sNationality As String = ""

        sNationality = Support.TPNationalityLookup(ThirdParties.DOTW, NationalityISOCode)

        If sNationality = "" Then
            sNationality = Settings.CustomerNationality(SearchDetails)
        End If

        Return sNationality
    End Function

    Public Shared Function GetCountryOfResidence(ByVal sCountryCode As String, ByVal SearchDetails As IThirdPartyAttributeSearch, ByVal Settings As IDOTWSettings) As String

        If sCountryCode = "" Then
            sCountryCode = Settings.CustomerCountryCode(SearchDetails)
        End If

        Return sCountryCode
    End Function

#End Region

End Class
