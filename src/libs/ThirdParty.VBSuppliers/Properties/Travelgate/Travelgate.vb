Imports System.Xml
Imports Intuitive
Imports Intuitive.Net.WebRequests
Imports Intuitive.Functions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.Abstractions.Models
Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions.Lookups
Imports System.Text
Imports Intuitive.XMLFunctions
Imports ThirdParty.Abstractions.Support
Imports System.Text.RegularExpressions
Imports System.Web
Imports ThirdParty.Abstractions.Constants
Imports System.Net.Http

Public MustInherit Class Travelgate
    Implements IThirdParty

#Region "Constructor"

    Public Sub New(settings As ITravelgateSettings, support As ITPSupport, httpClient As HttpClient)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
        _httpClient = Ensure.IsNotNull(httpClient, NameOf(httpClient))
    End Sub

#End Region

#Region "Properties"

    Public MustOverride Property Source As String

    Private ReadOnly _settings As ITravelgateSettings

    Private ReadOnly _support As ITPSupport

    Private ReadOnly _httpClient As HttpClient

    Private Function OffsetCancellationDays(searchDetails As IThirdPartyAttributeSearch) As Integer Implements IThirdParty.OffsetCancellationDays
        Return _settings.OffsetCancellationDays(searchDetails, False)
    End Function

    Public ReadOnly Property SupportsBookingSearch As Boolean Implements IThirdParty.SupportsBookingSearch
        Get
            Return False
        End Get
    End Property

    Private Function SupportsLiveCancellation(searchDetails As IThirdPartyAttributeSearch, source As String) As Boolean Implements IThirdParty.SupportsLiveCancellation
        Return _settings.AllowCancellations(searchDetails)
    End Function

    Public ReadOnly Property SupportsRemarks As Boolean Implements IThirdParty.SupportsRemarks
        Get
            Return False
        End Get
    End Property

    Private Function TakeSavingFromCommissionMargin(searchDetails As IThirdPartyAttributeSearch) As Boolean Implements IThirdParty.TakeSavingFromCommissionMargin
        Return False
    End Function

    Public Function RequiresVCard(info As VirtualCardInfo) As Boolean Implements IThirdParty.RequiresVCard
        Return _settings.RequiresVCard(info)
    End Function

    Private ReadOnly Property ReferenceDelimiter(SearchDetails As IThirdPartyAttributeSearch) As Char
        Get
            Dim sReferenceDelimiter As String = _settings.ReferenceDelimiter(SearchDetails, False)
            If sReferenceDelimiter = "" Then
                sReferenceDelimiter = "~"
            End If

            Return sReferenceDelimiter.Chars(0)
        End Get
    End Property


#End Region

#Region "Prebook"
    Public Function PreBook(oPropertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook

        Dim oRequestXML As New XmlDocument
        Dim oResponseXML As New XmlDocument
        Dim iSalesChannelID As Integer = oPropertyDetails.SalesChannelID
        Dim iBrandID As Integer = oPropertyDetails.BrandID
        Dim bSuccess As Boolean = False

        Try

            'Send request
            Dim sRequest As String = GenericRequest(BuildPrebookRequest(oPropertyDetails), oPropertyDetails)
            oRequestXML.LoadXml(sRequest)
            oResponseXML = SendRequest(sRequest, "Prebook", iSalesChannelID, iBrandID, oPropertyDetails)

            'Retrieve, decode and clean provider response
            Dim sDecodedProviderResponse As String = SafeNodeValue(oResponseXML, "Envelope/Body/ValuationResponse/ValuationResult/providerRS/rs")
            Dim oProviderResponse As New XmlDocument
            oProviderResponse.LoadXml(sDecodedProviderResponse)

            'Responses only contain a cost for the entire booking - so no individual room prices
            'If there's a difference in total we split the new cost against all the rooms
            Dim nBookingCost As Decimal = oProviderResponse.SelectSingleNode("ValuationRS/Price/@amount").InnerText.ToSafeDecimal()

            If nBookingCost = 0 Then
                Return False
            End If

            If nBookingCost <> oPropertyDetails.GrossCost Then

                Dim nPerRoomCost As Decimal = nBookingCost / oPropertyDetails.Rooms.Count

                For Each oRoomDetails As RoomDetails In oPropertyDetails.Rooms
                    oRoomDetails.LocalCost = nPerRoomCost
                    oRoomDetails.GrossCost = nPerRoomCost
                Next

            End If

            oPropertyDetails.LocalCost = nBookingCost

            Dim errata As String = SafeNodeValue(oProviderResponse, "ValuationRS/Remarks")
            If Not String.IsNullOrWhiteSpace(errata) Then
                oPropertyDetails.Errata.Add(New Erratum("Remarks", Me.RemoveHtmlTags(errata)))
            End If

            'Cancellations
            For Each oCancellationNode As XmlNode In oProviderResponse.SelectNodes("ValuationRS/CancelPenalties/CancelPenalty")

                Dim iCancellationHours As Integer = oCancellationNode.SelectSingleNode("HoursBefore").InnerText.ToSafeInt()
                Dim sCancellationType As String = oCancellationNode.SelectSingleNode("Penalty/@type").InnerText
                Dim nCancellationValue As Decimal = oCancellationNode.SelectSingleNode("Penalty").InnerText.ToSafeDecimal()
                Dim nCancellationCost As Decimal = 0

                'Three cancellation types: Importe - we are given a fixed fee; Porcentaje - a percentage of room cost; and Noches - a number of nights.
                Select Case sCancellationType
                    Case "Importe"
                        nCancellationCost = nCancellationValue
                    Case "Porcentaje"
                        nCancellationCost = nBookingCost * (nCancellationValue / 100)
                    Case "Noches"
                        nCancellationCost = (nBookingCost / oPropertyDetails.Duration) * nCancellationValue
                End Select

                Dim dFromDate As Date = oPropertyDetails.ArrivalDate.AddHours(-1 * iCancellationHours)
                Dim dEndDate As Date = oPropertyDetails.ArrivalDate
                If dFromDate > dEndDate Then
                    dEndDate = New Date(2099, 12, 31)
                End If

                Dim oCancellationPolicy As New Cancellation(dFromDate, dEndDate, nCancellationCost)

                oPropertyDetails.Cancellations.Add(oCancellationPolicy)

            Next

            'If no cancellation policies but considered a non-refundable rate, set a 100% cancellation policy effective from today
            Dim bNonRefundableRate As Boolean = SafeNodeValue(oProviderResponse, "ValuationRS/CancelPenalties/@nonRefundable").ToSafeBoolean()

            If oPropertyDetails.Cancellations.Count = 0 AndAlso bNonRefundableRate Then

                Dim oCancellationPolicy As New Cancellation(Date.Now, New Date(2099, 12, 31), nBookingCost)
                oPropertyDetails.Cancellations.Add(oCancellationPolicy)

            End If

            oPropertyDetails.Cancellations.Solidify(SolidifyType.Max)

            'Lastly, grab any parameters we may need for the booking, put an encrypted version in the TPRef
            If Not oProviderResponse.SelectSingleNode("ValuationRS/Parameters") Is Nothing Then
                Dim sEncryptedParameters As String = Encrypt(oProviderResponse.SelectSingleNode("ValuationRS/Parameters").OuterXml)
                oPropertyDetails.TPRef1 = sEncryptedParameters
            Else
                oPropertyDetails.TPRef1 = ""
            End If

            bSuccess = True

        Catch ex As Exception
            oPropertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString)
            bSuccess = False
        End Try

        'Lastly, add any logs
        If oRequestXML.InnerXml <> "" Then
            oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, String.Format("{0} PreBook Request", oPropertyDetails.Source), oRequestXML)
        End If

        If oResponseXML.InnerXml <> "" Then
            oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, String.Format("{0} PreBook Response", oPropertyDetails.Source), oResponseXML)
        End If

        Return bSuccess
    End Function
#End Region

#Region "Book"
    Public Function Book(oPropertyDetails As PropertyDetails) As String Implements IThirdParty.Book

        Dim oRequestXML As New XmlDocument
        Dim oResponseXML As New XmlDocument
        Dim sReference As String = String.Empty
        Dim iSalesChannelID As Integer = oPropertyDetails.SalesChannelID
        Dim iBrandID As Integer = oPropertyDetails.BrandID

        Try

            'Send request
            Dim sRequest As String = GenericRequest(BuildBookRequest(oPropertyDetails), oPropertyDetails)
            oRequestXML.LoadXml(sRequest)
            oResponseXML = SendRequest(sRequest, "Book", iSalesChannelID, iBrandID, oPropertyDetails)

            'Retrieve, decode and clean provider response
            Dim sDecodedProviderResponse As String = SafeNodeValue(oResponseXML, "Envelope/Body/ReservationResponse/ReservationResult/providerRS/rs")
            Dim oProviderResponse As New XmlDocument
            oProviderResponse.LoadXml(sDecodedProviderResponse)

            If oProviderResponse.SelectSingleNode("ReservationRS/ResStatus").InnerText.ToString = "OK" Then

                Dim sBookingReference As String = oProviderResponse.SelectSingleNode("ReservationRS/ProviderLocator").InnerText.ToString
                sReference = sBookingReference
            Else
                sReference = "Failed"
            End If

        Catch ex As Exception
            oPropertyDetails.Warnings.AddNew("Book Exception", ex.ToString)
            sReference = "Failed"
        End Try

        'Lastly, add any logs
        If oRequestXML.InnerXml <> "" Then
            oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, String.Format("{0} Book Request", oPropertyDetails.Source), oRequestXML)
        End If

        If oResponseXML.InnerXml <> "" Then
            oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, String.Format("{0} Book Response", oPropertyDetails.Source), oResponseXML)
        End If

        Return sReference

    End Function

#End Region

#Region "Cancellation"
    Public Function CancelBooking(oPropertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking

        Dim oRequestXML As New XmlDocument
        Dim oResponseXML As New XmlDocument
        Dim oCancellationResponse As New ThirdPartyCancellationResponse
        Dim iSalesChannelID As Integer = oPropertyDetails.SalesChannelID
        Dim iBrandID As Integer = oPropertyDetails.BrandID

        Try

            'Send request
            Dim sRequest As String = GenericRequest(BuildCancellationRequest(oPropertyDetails), oPropertyDetails)
            oRequestXML.LoadXml(sRequest)
            oResponseXML = SendRequest(sRequest, "Cancellation", iSalesChannelID, iBrandID, oPropertyDetails)

            'Retrieve, decode and clean provider response
            Dim sDecodedProviderResponse As String = SafeNodeValue(oResponseXML, "Envelope/Body/CancelResponse/CancelResult/providerRS/rs")
            Dim oProviderResponse As New XmlDocument
            oProviderResponse.LoadXml(sDecodedProviderResponse)

            If XMLFunctions.SafeNodeValue(oProviderResponse, "CancelRS/TransactionStatus/ResStatus") = "CN" Then

                Dim sCancellationReference As String = XMLFunctions.SafeNodeValue(oProviderResponse, "CancelId")

                If sCancellationReference <> "" Then
                    oCancellationResponse.TPCancellationReference = sCancellationReference
                Else
                    oCancellationResponse.TPCancellationReference = oPropertyDetails.SourceReference
                End If

                oCancellationResponse.Success = True
            End If

        Catch ex As Exception
            oPropertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString)
            oCancellationResponse.Success = False
        End Try

        'Lastly, add any logs
        If oRequestXML.InnerXml <> "" Then
            oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, String.Format("{0} Cancellation Request", oPropertyDetails.Source), oRequestXML)
        End If

        If oResponseXML.InnerXml <> "" Then
            oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, String.Format("{0} Cancellation Response", oPropertyDetails.Source), oResponseXML)
        End If

        Return oCancellationResponse

    End Function

    Public Function GetCancellationCost(oPropertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost
        Return New ThirdPartyCancellationFeeResult
    End Function
#End Region

#Region "Booking Search"

    Public Function BookingSearch(oBookingSearchDetails As BookingSearchDetails) As ThirdPartyBookingSearchResults Implements IThirdParty.BookingSearch
        Return New ThirdPartyBookingSearchResults
    End Function

    Public Function CreateReconciliationReference(sInputReference As String) As String Implements IThirdParty.CreateReconciliationReference
        Return ""
    End Function

    Public Function BookingStatusUpdate(oPropertyDetails As PropertyDetails) As ThirdPartyBookingStatusUpdateResult Implements IThirdParty.BookingStatusUpdate
        Return Nothing
    End Function
#End Region

#Region "Helpers"

    Public Function SendRequest(ByVal RequestString As String, ByVal RequestType As String, ByVal SalesChannelID As Integer, ByVal BrandID As Integer, ByVal PropertyDetails As PropertyDetails) As XmlDocument

        Dim oResponseXML As New XmlDocument

        Dim oWebRequest As New Request

        With oWebRequest

            Select Case RequestType.ToLower
                Case "prebook"
                    .SoapAction = _settings.PrebookSOAPAction(PropertyDetails)
                Case "book"
                    .SoapAction = _settings.BookSOAPAction(PropertyDetails)
                Case "cancellation"
                    .SoapAction = _settings.CancelSOAPAction(PropertyDetails)
            End Select

            .EndPoint = _settings.URL(PropertyDetails)
            .Method = eRequestMethod.POST
            .Source = PropertyDetails.Source
            .LogFileName = RequestType
            .CreateLog = True
            .SetRequest(RequestString)
            .UseGZip = True
            .Send(_httpClient)
        End With

        oResponseXML = CleanXMLNamespaces(oWebRequest.ResponseXML)

        Return oResponseXML

    End Function

    Public Function RemoveHtmlTags(ByVal text As String) As String
        Return Regex.Replace(text, "<(?:[^>=]|='[^']*'|=""[^""]*""|=[^'""][^\s>]*)*>", "")
    End Function

#End Region

#Region "Request builders"

    Public Function BuildPrebookRequest(ByVal PropertyDetails As PropertyDetails) As String

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Dim sbPrebookRequest As New StringBuilder

        'Create valuation request
        With sbPrebookRequest
            .Append("<ns:Valuation>")
            .Append("<ns:valuationRQ>")
            .Append("<ns:timeoutMilliseconds>180000</ns:timeoutMilliseconds>") ' max general timeout 180s
            .Append("<ns:version>1</ns:version>")

            .Append("<ns:providerRQ>")
            .AppendFormat("<ns:code>{0}</ns:code>", _settings.ProviderCode(PropertyDetails))
            .AppendFormat("<ns:id>{0}</ns:id>", 1)
            .Append("<ns:rqXML>")

            .Append("<ValuationRQ>")

            .Append("<timeoutMilliseconds>179700</timeoutMilliseconds>") ' has to be lower than general timeout
            .Append("<source>")
            .AppendFormat("<languageCode>{0}</languageCode>", _settings.LanguageCode(PropertyDetails))
            .Append("</source>")
            .Append("<filterAuditData>")
            .Append("<registerTransactions>true</registerTransactions>")
            .Append("</filterAuditData>")

            .Append("<Configuration>")
            .AppendFormat("<User>{0}</User>", _settings.ProviderUsername(PropertyDetails))
            .AppendFormat("<Password>{0}</Password>", _settings.ProviderPassword(PropertyDetails))
            .Append(AppendURLs(PropertyDetails))
            .Append(HttpUtility.HtmlDecode(_settings.Parameters(PropertyDetails)))
            .Append("</Configuration>")

            .AppendFormat("<StartDate>{0}</StartDate>", PropertyDetails.ArrivalDate.ToString("dd/MM/yyyy"))
            .AppendFormat("<EndDate>{0}</EndDate>", PropertyDetails.DepartureDate.ToString("dd/MM/yyyy"))

            'Add hotel option parameters
            'Check if we have any first before we add the parameters node
            'Don't do multiroom bookings for rooms across different options, so can just pluck out the first room's parameters
            Dim sReferenceDelimiter As String = _settings.ReferenceDelimiter(PropertyDetails, False)
            If sReferenceDelimiter = "" Then
                sReferenceDelimiter = "~"
            End If
            Dim sEncryptedParameters As String = PropertyDetails.Rooms(0).ThirdPartyReference.Split(sReferenceDelimiter.Chars(0))(4)

            If sEncryptedParameters <> "" Then
                Dim oParameters As New XmlDocument
                oParameters.LoadXml(Decrypt(sEncryptedParameters))
                .Append(oParameters.OuterXml)
            End If

            .AppendFormat("<MealPlanCode>{0}</MealPlanCode>", PropertyDetails.Rooms(0).ThirdPartyReference.Split(sReferenceDelimiter.Chars(0))(7))
            .AppendFormat("<HotelCode>{0}</HotelCode>", PropertyDetails.TPKey)
            .AppendFormat("<PaymentType>{0}</PaymentType>", PropertyDetails.Rooms(0).ThirdPartyReference.Split(sReferenceDelimiter.Chars(0))(3))
            .Append("<OptionType>Hotel</OptionType>")
            Dim sNationality As String = ""
            Dim sNationalityLookupValue As String = _support.TPNationalityLookup(ThirdParties.TRAVELGATE, PropertyDetails.NationalityID)
            Dim sDefaultNationality As String = _settings.DefaultNationality(PropertyDetails, False)

            If Not String.IsNullOrEmpty(sNationalityLookupValue) Then
                sNationality = sNationalityLookupValue
            ElseIf sDefaultNationality <> "" Then
                sNationality = sDefaultNationality
            End If
            If sNationality <> "" Then
                .AppendFormat("<Nationality>{0}</Nationality>", sNationality)
            End If

            .Append("<Rooms>")

            Dim iRoomID As Integer = 1
            For Each oRoom As RoomDetails In PropertyDetails.Rooms
                .AppendFormat("<Room id = ""{0}"" roomCandidateRefId = ""{3}"" code = ""{1}"" description = ""{2}""/>",
                oRoom.ThirdPartyReference.Split(sReferenceDelimiter.Chars(0))(0), oRoom.ThirdPartyReference.Split(sReferenceDelimiter.Chars(0))(1), oRoom.ThirdPartyReference.Split(sReferenceDelimiter.Chars(0))(2), iRoomID)

                iRoomID += 1
            Next

            .Append("</Rooms>")
            .Append("<RoomCandidates>")

            iRoomID = 1
            Dim iPassengerID As Integer

            For Each oRoom As RoomDetails In PropertyDetails.Rooms
                .AppendFormat("<RoomCandidate id = ""{0}"">", iRoomID)
                .Append("<Paxes>")

                iPassengerID = 1 ' every room starts with PaxID 1
                For Each oPassenger As Passenger In oRoom.Passengers
                    Dim iAge As Integer
                    Select Case oPassenger.PassengerType
                        Case PassengerType.Adult
                            iAge = 30
                        Case PassengerType.Child
                            iAge = oPassenger.Age
                        Case PassengerType.Infant
                            iAge = 1
                    End Select

                    .AppendFormat("<Pax age = ""{0}"" id = ""{1}""/>", iAge, iPassengerID)

                    iPassengerID += 1
                Next

                .Append("</Paxes>")
                .Append("</RoomCandidate>")

                iRoomID += 1
            Next
            .Append("</RoomCandidates>")
            .Append("</ValuationRQ>")

            .Append("</ns:rqXML>")
            .Append("</ns:providerRQ>")
            .Append("</ns:valuationRQ>")
            .Append("</ns:Valuation>")

        End With

        Return sbPrebookRequest.ToString

    End Function

    Public Function BuildBookRequest(ByVal oPropertyDetails As PropertyDetails) As String

        Dim iSalesChannelId As Integer = oPropertyDetails.SalesChannelID
        Dim iBrandId As Integer = oPropertyDetails.BrandID
        Dim sSource As String = oPropertyDetails.Source

        Dim cDelimiter As Char = Me.ReferenceDelimiter(oPropertyDetails)

        Dim sbBookRequest As New StringBuilder

        Dim sPaymentType As String = oPropertyDetails.Rooms(0).ThirdPartyReference.Split(cDelimiter)(3)

        With sbBookRequest
            .Append("<ns:Reservation>")
            .Append("<ns:reservationRQ>")
            .Append("<ns:timeoutMilliseconds>180000</ns:timeoutMilliseconds>") ' max general timeout 180s
            .Append("<ns:version>1</ns:version>")

            .Append("<ns:providerRQ>")
            .AppendFormat("<ns:code>{0}</ns:code>", _settings.ProviderCode(oPropertyDetails))
            .AppendFormat("<ns:id>{0}</ns:id>", 1)
            .Append("<ns:rqXML>")

            .Append("<ReservationRQ>")

            .Append("<timeoutMilliseconds>179700</timeoutMilliseconds>") ' has to be lower than general timeout
            .Append("<source>")
            .AppendFormat("<languageCode>{0}</languageCode>", _settings.LanguageCode(oPropertyDetails))
            .Append("</source>")
            .Append("<filterAuditData>")
            .Append("<registerTransactions>true</registerTransactions>")
            .Append("</filterAuditData>")

            .Append("<Configuration>")
            .AppendFormat("<User>{0}</User>", _settings.ProviderUsername(oPropertyDetails))
            .AppendFormat("<Password>{0}</Password>", _settings.ProviderPassword(oPropertyDetails))
            .Append(AppendURLs(oPropertyDetails))
            .Append(HttpUtility.HtmlDecode(_settings.Parameters(oPropertyDetails)))
            .Append("</Configuration>")
            If (_settings.SendGUIDReference(oPropertyDetails)) Then
                .AppendFormat("<ClientLocator>{0}</ClientLocator>", oPropertyDetails.BookingReference.TrimEnd() & Guid.NewGuid.ToString)
            Else
                .AppendFormat("<ClientLocator>{0}</ClientLocator>", oPropertyDetails.BookingReference)
            End If
            .AppendLine(BuildCardDetails(oPropertyDetails))

            .AppendFormat("<StartDate>{0}</StartDate>", oPropertyDetails.ArrivalDate.ToString("dd/MM/yyyy"))
            .AppendFormat("<EndDate>{0}</EndDate>", oPropertyDetails.DepartureDate.ToString("dd/MM/yyyy"))

            'Add hotel option parameters
            'Check if we have any first before we add the parameters node
            'Don't do multiroom bookings for rooms across different options, so can just pluck out the first room's parameters
            Dim sEncryptedParameters As String = oPropertyDetails.TPRef1

            If sEncryptedParameters <> "" Then
                Dim oParameters As New XmlDocument
                oParameters.LoadXml(Decrypt(sEncryptedParameters))
                .Append(oParameters.OuterXml)
            End If

            .AppendFormat("<MealPlanCode>{0}</MealPlanCode>", oPropertyDetails.Rooms(0).ThirdPartyReference.Split(cDelimiter)(7))
            .AppendFormat("<HotelCode>{0}</HotelCode>", oPropertyDetails.TPKey)

            Dim sNationality As String = ""
            Dim sNationalityLookupValue As String = _support.TPNationalityLookup(Source, oPropertyDetails.NationalityID)
            Dim sDefaultNationality As String = _settings.DefaultNationality(oPropertyDetails, False)

            If Not String.IsNullOrEmpty(sNationalityLookupValue) Then
                sNationality = sNationalityLookupValue
            ElseIf sDefaultNationality <> "" Then
                sNationality = sDefaultNationality
            End If
            If sNationality <> "" Then
                .AppendFormat("<Nationality>{0}</Nationality>", sNationality)
            End If

            Dim sCurrencyCode As String = _support.TPCurrencyLookup(sSource, oPropertyDetails.CurrencyCode)
            'send gross cost as gross net down can be used (local is the net)
            .AppendFormat("<Price currency = ""{0}"" amount = ""{1}"" binding = ""{2}"" commission = ""{3}""/>", sCurrencyCode, oPropertyDetails.GrossCost,
                oPropertyDetails.Rooms(0).ThirdPartyReference.Split(cDelimiter)(6), oPropertyDetails.Rooms(0).ThirdPartyReference.Split(cDelimiter)(5))
            .Append("<ResGuests>")
            .Append("<Guests>")

            Dim iPassengerCount As Integer
            Dim iRoomCount As Integer = 1
            For Each oRoom As RoomDetails In oPropertyDetails.Rooms

                iPassengerCount = 1
                For Each oPassenger As Passenger In oRoom.Passengers

                    .AppendFormat("<Guest roomCandidateId = ""{0}"" paxId = ""{1}"">", iRoomCount, iPassengerCount)
                    .AppendFormat("<GivenName>{0}</GivenName>", oPassenger.FirstName)
                    .AppendFormat("<SurName>{0}</SurName>", oPassenger.LastName)
                    .Append("</Guest>")

                    iPassengerCount += 1
                Next
                iRoomCount += 1
            Next
            .Append("</Guests>")
            .Append("</ResGuests>")
            .AppendFormat("<PaymentType>{0}</PaymentType>", sPaymentType)

            .Append("<Rooms>")

            iRoomCount = 1

            For Each oRoom As RoomDetails In oPropertyDetails.Rooms
                .AppendFormat("<Room id = ""{0}"" roomCandidateRefId = ""{3}"" code = ""{1}"" description = ""{2}""/>",
                oRoom.ThirdPartyReference.Split(cDelimiter)(0), oRoom.ThirdPartyReference.Split(cDelimiter)(1), oRoom.ThirdPartyReference.Split(cDelimiter)(2), iRoomCount)

                iRoomCount += 1
            Next
            .Append("</Rooms>")
            .Append("<RoomCandidates>")

            iRoomCount = 1
            Dim iPassengerID As Integer

            For Each oRoom As RoomDetails In oPropertyDetails.Rooms
                .AppendFormat("<RoomCandidate id = ""{0}"">", iRoomCount)
                .Append("<Paxes>")

                iPassengerID = 1
                For Each oPassenger As Passenger In oRoom.Passengers
                    Dim iAge As Integer
                    Select Case oPassenger.PassengerType
                        Case PassengerType.Adult
                            iAge = 30
                        Case PassengerType.Child
                            iAge = oPassenger.Age
                        Case PassengerType.Infant
                            iAge = 1
                    End Select

                    .AppendFormat("<Pax age = ""{0}"" id = ""{1}""/>", iAge, iPassengerID)

                    iPassengerID += 1
                Next

                .Append("</Paxes>")
                .Append("</RoomCandidate>")

                iRoomCount += 1
            Next

            .Append("</RoomCandidates>")
            If oPropertyDetails.BookingComments.ToString() <> "" Then
                .Append("<Remarks>")
                .Append(oPropertyDetails.BookingComments.ToString().Trim())
                .Append("</Remarks>")
            End If
            .Append("</ReservationRQ>")

            .Append("</ns:rqXML>")
            .Append("</ns:providerRQ>")
            .Append("</ns:reservationRQ>")
            .Append("</ns:Reservation>")
        End With

        Return sbBookRequest.ToString

    End Function

    Public Function BuildCancellationRequest(ByVal PropertyDetails As PropertyDetails) As String
        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Dim sbCancellationRequest As New StringBuilder

        With sbCancellationRequest
            .Append("<ns:Cancel>")
            .Append("<ns:cancelRQ>")
            .Append("<ns:timeoutMilliseconds>180000</ns:timeoutMilliseconds>") ' max general timeout 180s
            .Append("<ns:version>1</ns:version>")

            .Append("<ns:providerRQ>")
            .AppendFormat("<ns:code>{0}</ns:code>", _settings.ProviderCode(PropertyDetails))
            .AppendFormat("<ns:id>{0}</ns:id>", 1)
            .Append("<ns:rqXML>")

            .AppendFormat("<CancelRQ  hotelCode=""{0}"">", PropertyDetails.TPKey)

            .Append("<timeoutMilliseconds>179700</timeoutMilliseconds>") ' has to be lower than general timeout
            .Append("<source>")
            .AppendFormat("<languageCode>{0}</languageCode>", _settings.LanguageCode(PropertyDetails))
            .Append("</source>")
            .Append("<filterAuditData>")
            .Append("<registerTransactions>true</registerTransactions>")
            .Append("</filterAuditData>")

            .Append("<Configuration>")
            .AppendFormat("<User>{0}</User>", _settings.ProviderUsername(PropertyDetails))
            .AppendFormat("<Password>{0}</Password>", _settings.ProviderPassword(PropertyDetails))
            .Append(AppendURLs(PropertyDetails))
            .Append(HttpUtility.HtmlDecode(_settings.Parameters(PropertyDetails)))
            .Append("</Configuration>")

            .Append("<Locators>")
            .AppendFormat("<Client>{0}</Client>", PropertyDetails.BookingReference)
            .AppendFormat("<Provider>{0}</Provider>", PropertyDetails.SourceReference)
            .Append("</Locators>")
            .AppendFormat("<StartDate>{0}</StartDate>", PropertyDetails.ArrivalDate.ToString("dd/MM/yyyy"))
            .AppendFormat("<EndDate>{0}</EndDate>", PropertyDetails.DepartureDate.ToString("dd/MM/yyyy"))
            .Append("</CancelRQ>")

            .Append("</ns:rqXML>")
            .Append("</ns:providerRQ>")
            .Append("</ns:cancelRQ>")
            .Append("</ns:Cancel>")
        End With

        Return sbCancellationRequest.ToString

    End Function

    Public Function GenericRequest(ByVal SpecificRequest As String, ByVal SearchDetails As IThirdPartyAttributeSearch) As String

        Dim sbRequest As New StringBuilder
        With sbRequest

            .Append("<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" ")
            .Append("xmlns:ns=""http://schemas.xmltravelgate.com/hub/2012/06"" ")
            .Append("xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">")
            .Append("<soapenv:Header>")
            .Append("<wsse:Security>")
            .Append("<wsse:UsernameToken>")
            .AppendFormat("<wsse:Username>{0}</wsse:Username>", _settings.Username(SearchDetails))
            .AppendFormat("<wsse:Password>{0}</wsse:Password>", _settings.Password(SearchDetails))
            .Append("</wsse:UsernameToken>")
            .Append("</wsse:Security>")
            .Append("</soapenv:Header>")
            .Append("<soapenv:Body>")
            .Append(SpecificRequest)
            .Append("</soapenv:Body>")
            .Append("</soapenv:Envelope>")

        End With
        Return sbRequest.ToString

    End Function

    Public Function AppendURL(ByVal URLType As String, ByVal Source As String, ByVal SearchDetails As IThirdPartyAttributeSearch) As String

        Dim sURL As String = _settings.URL(SearchDetails)

        If sURL = "" Then
            Return ""
        Else
            Dim sNodeName As String = ""
            Select Case URLType
                Case "URLReservation"
                    sNodeName = "UrlReservation"
                Case "URLGeneric"
                    sNodeName = "UrlGeneric"
                Case "URLAvail"
                    sNodeName = "UrlAvail"
                Case "URLValuation"
                    sNodeName = "UrlValuation"
            End Select

            Return String.Format("<{0}>{1}</{0}>", sNodeName, sURL)
        End If

    End Function

    Public Function BuildCardDetails(ByVal oPropertyDetails As PropertyDetails) As String

        Dim iSalesChannelId As Integer = oPropertyDetails.SalesChannelID
        Dim iBrandId As Integer = oPropertyDetails.BrandID
        Dim sSource As String = oPropertyDetails.Source

        Dim cDelimiter As Char = Me.ReferenceDelimiter(oPropertyDetails)

        Dim sPaymentType As String = oPropertyDetails.Rooms(0).ThirdPartyReference.Split(cDelimiter)(3)
        Dim bRequiresVCard As Boolean = _settings.RequiresVCard(oPropertyDetails)

        Dim sb As New StringBuilder

        With sb

            If bRequiresVCard AndAlso (sPaymentType = "CardBookingPay" Or sPaymentType = "CardCheckInPay") Then

                Dim sCardHolderName As String = oPropertyDetails.GeneratedVirtualCard.CardHolderName
                If sCardHolderName = "" Then
                    sCardHolderName = _settings.CardHolderName(oPropertyDetails)
                End If

                .AppendLine("<CardInfo>")
                .AppendLine($"<CardCode>{_support.TPCreditCardLookup(sSource, oPropertyDetails.GeneratedVirtualCard.CardTypeID)}</CardCode>")
                .AppendLine($"<Number>{oPropertyDetails.GeneratedVirtualCard.CardNumber}</Number>")
                .AppendLine($"<Holder>{sCardHolderName}</Holder>")
                .AppendLine("<ValidityDate>")
                .AppendLine($"<Month>{oPropertyDetails.GeneratedVirtualCard.ExpiryMonth.PadLeft(2, "0"c)}</Month>")
                .AppendLine($"<Year>{oPropertyDetails.GeneratedVirtualCard.ExpiryYear.Substring(2)}</Year>")
                .AppendLine("</ValidityDate>")
                .AppendLine($"<CVC>{oPropertyDetails.GeneratedVirtualCard.CVV}</CVC>")
                .AppendLine("</CardInfo>")

            ElseIf Not bRequiresVCard Then

                Dim sEncrytedCardDetails As String = _settings.EncryptedCardDetails(oPropertyDetails)
                If String.IsNullOrWhiteSpace(sEncrytedCardDetails) Then Return String.Empty

                'Card details are encryted in the form 'CardCode|CardNumber|ExpiryDate|CVC|CardHolderName'
                Dim sDecryptedCardDetails As String() = Intuitive.Functions.Decrypt(sEncrytedCardDetails).Split("|"c)
                If sDecryptedCardDetails.Count() <> 5 Then Return String.Empty

                Dim dtExpiryDate As DateTime = sDecryptedCardDetails(2).ToSafeDate()

                .AppendLine("<CardInfo>")
                .AppendLine($"<CardCode>{sDecryptedCardDetails(0)}</CardCode>")
                .AppendLine($"<Number>{sDecryptedCardDetails(1)}</Number>")
                .AppendLine($"<Holder>{sDecryptedCardDetails(4)}</Holder>")
                .AppendLine("<ValidityDate>")
                .AppendLine($"<Month>{dtExpiryDate.Month.ToString().PadLeft(2, "0"c)}</Month>")
                .AppendLine($"<Year>{dtExpiryDate.Year}</Year>")
                .AppendLine("</ValidityDate>")
                .AppendLine($"<CVC>{sDecryptedCardDetails(3)}</CVC>")
                .AppendLine("</CardInfo>")

            End If
        End With

        Return sb.ToString()
    End Function

#End Region

#Region "Helper class"

    Public Function AppendURLs(ByVal SearchDetails As IThirdPartyAttributeSearch) As String

        Dim sbURLXML As New StringBuilder

        With sbURLXML
            .AppendFormat("<UrlReservation>{0}</UrlReservation>", _settings.UrlReservation(SearchDetails))
            .AppendFormat("<UrlGeneric>{0}</UrlGeneric>", _settings.UrlGeneric(SearchDetails))
            .AppendFormat("<UrlAvail>{0}</UrlAvail>", _settings.UrlAvail(SearchDetails))
            .AppendFormat("<UrlValuation>{0}</UrlValuation>", _settings.UrlValuation(SearchDetails))
        End With

        Return sbURLXML.ToString

    End Function

#End Region

#Region "End session"

    Public Sub EndSession(oPropertyDetails As PropertyDetails) Implements IThirdParty.EndSession

    End Sub

#End Region

End Class