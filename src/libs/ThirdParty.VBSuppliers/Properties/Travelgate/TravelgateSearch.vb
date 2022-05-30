Imports System.Xml
Imports System.Text
Imports Intuitive.Domain.Search
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Search.Models
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Search.Support
Imports Intuitive
Imports System.Web
Imports System.Xml.Serialization
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Results
Imports Intuitive.Net.WebRequests
Imports System.IO
Imports Intuitive.Helpers.Extensions

Public Class TravelgateSearch
    Inherits ThirdPartyPropertySearchBase

#Region "Constructor"

    Public Sub New(settings As ITravelgateSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

#End Region

#Region "Properties"

    Private ReadOnly _settings As ITravelgateSettings

    Private ReadOnly _support As ITPSupport

    Public Overrides Property Source As String = ThirdParties.TRAVELGATE

    Public Overrides ReadOnly Property SqlRequest As Boolean = False

#End Region

#Region "Search Restrictions"

    Public Overrides Function SearchRestrictions(oSearchDetails As SearchDetails) As Boolean
        Return False
    End Function

#End Region

#Region "SearchFunctions"

    Public Overrides Function BuildSearchRequests(oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit), bSaveLogs As Boolean) As List(Of Request)

        Dim oRequests As New List(Of Request)
        Dim iSalesChannelID As Integer = oSearchDetails.SalesChannelID
        Dim iBrandID As Integer = oSearchDetails.BrandID

        'Remove any suppliers in the dictionary that are attempting a restricted search
        'Needs to be done here since
        Dim oFilteredSuppliers As New List(Of String)

        Dim iMaximumRoomNumber As Integer = _settings.MaximumRoomNumber(oSearchDetails)
        Dim iMaximumRoomGuestNumber As Integer = _settings.MaximumRoomGuestNumber(oSearchDetails)
        Dim iMinimumStay As Integer = _settings.MinimumStay(oSearchDetails)

        Dim bSearchExceedsGuestCount As Boolean = False

        For Each oRoom As iVector.Search.Property.RoomDetail In oSearchDetails.RoomDetails
            If oRoom.Adults + oRoom.Children + oRoom.Infants > iMaximumRoomGuestNumber Then
                bSearchExceedsGuestCount = True
            End If
        Next

        If Not (oSearchDetails.Rooms > iMaximumRoomNumber OrElse oSearchDetails.Duration < iMinimumStay OrElse bSearchExceedsGuestCount) Then
            oFilteredSuppliers.Add(Me.Source)
        End If

        If oFilteredSuppliers.Count > 0 Then

            Dim sbSearchRequest As New StringBuilder

            With sbSearchRequest

                .Append("<soapenv:Envelope xmlns:soapenv = ""http://schemas.xmlsoap.org/soap/envelope/"" ")
                .Append("xmlns:ns = ""http://schemas.xmltravelgate.com/hub/2012/06"" ")
                .Append("xmlns:wsse = ""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">")
                .Append("<soapenv:Header>")
                .Append("<wsse:Security>")
                .Append("<wsse:UsernameToken>")
                .AppendFormat("<wsse:Username>{0}</wsse:Username>", _settings.Username(oSearchDetails))
                .AppendFormat("<wsse:Password>{0}</wsse:Password>", _settings.Password(oSearchDetails))
                .Append("</wsse:UsernameToken>")
                .Append("</wsse:Security>")
                .Append("</soapenv:Header>")
                .Append("<soapenv:Body>")
                .Append("<ns:Avail>")
                .Append("<ns:availRQ>")
                .Append("<ns:timeoutMilliseconds>25000</ns:timeoutMilliseconds>") ' max general search timeout is 25s
                .Append("<ns:version>1</ns:version>")
                .Append("<ns:providerRQs>")

                Dim iRequestCount As Integer = 1

                'Some third parties don't support searches by TPKey, so use these to check what kind of search we want to be doing
                Dim iMaximumHotelSearchNumber As Integer = _settings.MaximumHotelSearchNumber(oSearchDetails)
                Dim iMaximumCitySearchNumber As Integer = _settings.MaximumCitySearchNumber(oSearchDetails)
                'We generally prefer hotel based searches, but if a third party has a small maximum number of hotels per search
                'we leave it up to the discretion of the user to determine if they would rather perform city based searches (where allowed)
                Dim bAllowHotelSearch As Boolean = _settings.AllowHotelSearch(oSearchDetails)
                'Whether to try to search for a zone (region) instead of individual resorts (cities)
                Dim bUseZoneSearch As Boolean = oResortSplits.Count > 1 AndAlso _settings.UseZoneSearch(oSearchDetails)

                Dim oSearchBatchDetails As New SearchBatchDetails
                oSearchBatchDetails.Source = Me.Source
                oSearchBatchDetails.ResortSplits = oResortSplits

                'Check how many hotels we have - if only one we can ignore the allow hotel search boolean
                Dim iHotelCount As Integer = 0
                For Each oResortSplit As ResortSplit In oResortSplits
                    iHotelCount += oResortSplit.Hotels.Count
                Next

                'Get a count of our resorts as well
                Dim iResortCount As Integer = oResortSplits.Count

                'Set which search type we will be using - if hotel searches are allowed and either the allow flag is set to true,
                'or else city searches aren't allowed or we're only searching for one hotel, search by hotel
                'Otherwiwse search by city

                If iMaximumHotelSearchNumber > 0 AndAlso (bAllowHotelSearch OrElse iMaximumCitySearchNumber = 0 OrElse iHotelCount = 1) Then

                    oSearchBatchDetails.SearchByHotel = True

                    'Get the batch size and count for hotel searches
                    oSearchBatchDetails.BatchSize = iMaximumHotelSearchNumber
                    oSearchBatchDetails.SearchItemCount = iHotelCount

                ElseIf iMaximumCitySearchNumber > 0 Then

                    oSearchBatchDetails.SearchByHotel = False

                    If bUseZoneSearch Then
                        oSearchBatchDetails.SetZoneSearchID()
                        oSearchBatchDetails.UseZoneSearch = oSearchBatchDetails.SearchItemIDs.Count = 1
                    End If

                    If oSearchBatchDetails.UseZoneSearch Then
                        oSearchBatchDetails.BatchSize = 1
                        oSearchBatchDetails.SearchItemCount = 1
                    Else
                        'Get the batch size and count for city searches
                        oSearchBatchDetails.BatchSize = iMaximumCitySearchNumber
                        oSearchBatchDetails.SearchItemCount = iResortCount
                    End If
                End If

                oSearchBatchDetails.CalculateBatchCount()

                If Not oSearchBatchDetails.UseZoneSearch Then
                    oSearchBatchDetails.SetSearchIDs()
                End If

                BuildSearchBatch(oSearchDetails, oSearchBatchDetails, sbSearchRequest, iRequestCount)

                'Next

                .Append("</ns:providerRQs>")
                .Append("</ns:availRQ>")
                .Append("</ns:Avail>")
                .Append("</soapenv:Body>")
                .Append("</soapenv:Envelope>")

            End With

            'Build Request Object
            Dim oRequest As New Request
            With oRequest
                .EndPoint = _settings.URL(oSearchDetails)
                .SoapAction = _settings.SearchSOAPAction(oSearchDetails)
                .Method = eRequestMethod.POST
                .Source = Me.Source
                .LogFileName = "Search"
                .CreateLog = bSaveLogs
                .TimeoutInSeconds = RequestTimeOutSeconds(oSearchDetails)
                .ExtraInfo = oSearchDetails
                .SuppressExpectHeaders = True
                .SetRequest(sbSearchRequest.ToString)
                .UseGZip = _settings.UseGZip(oSearchDetails)
            End With

            oRequests.Add(oRequest)

        End If

        Return oRequests

    End Function

    Public Sub BuildSearchBatch(ByVal SearchDetails As SearchDetails, ByVal SearchBatchDetails As SearchBatchDetails,
     ByRef SearchRequest As StringBuilder, ByRef RequestCount As Integer)

        Dim iSalesChannelID As Integer = SearchDetails.SalesChannelID
        Dim iBrandID As Integer = SearchDetails.BrandID

        'Index to keep track of where we're at
        Dim iIndex As Integer = 0

        With SearchRequest

            'Loop through the batches
            For iBatchNumber As Integer = 1 To SearchBatchDetails.BatchCount

                .Append("<ns:ProviderRQ>")
                .AppendFormat("<ns:code>{0}</ns:code>", _settings.ProviderCode(SearchDetails))
                .AppendFormat("<ns:id>{0}</ns:id>", RequestCount)
                .Append("<ns:rqXML>")
                .Append("<AvailRQ>")
                .AppendFormat("<timeoutMilliseconds>{0}</timeoutMilliseconds>", _settings.SearchRequestTimeout(SearchDetails))
                .Append("<source>")
                .AppendFormat("<languageCode>{0}</languageCode>", _settings.LanguageCode(SearchDetails))
                .Append("</source>")
                .Append("<filterAuditData>")
                .Append("<registerTransactions>false</registerTransactions>")
                .Append("</filterAuditData>")
                .Append("<Configuration>")
                .AppendFormat("<User>{0}</User>", _settings.ProviderUsername(SearchDetails))
                .AppendFormat("<Password>{0}</Password>", _settings.ProviderPassword(SearchDetails))
                .Append(AppendURLs(SearchDetails))
                .Append(HttpUtility.HtmlDecode(_settings.Parameters(SearchDetails)))

                .Append("</Configuration>")

                .Append("<OnRequest>false</OnRequest>")

                .Append("<AvailDestinations>")

                'Get the last item for our current batch
                Dim iLastBatchItem As Integer = 0

                If iIndex + SearchBatchDetails.BatchSize > SearchBatchDetails.SearchItemCount Then
                    iLastBatchItem = SearchBatchDetails.SearchItemCount - 1
                Else
                    iLastBatchItem = iIndex + SearchBatchDetails.BatchSize - 1
                End If

                For iPosition As Integer = iIndex To iLastBatchItem

                    If SearchBatchDetails.SearchByHotel Then
                        .AppendFormat("<Destination type = ""HOT"" code = ""{0}""/>", SearchBatchDetails.SearchItemIDs.ElementAt(iIndex))
                    ElseIf SearchBatchDetails.UseZoneSearch Then
                        .AppendFormat("<Destination type = ""ZON"" code = ""{0}""/>", SearchBatchDetails.SearchItemIDs.ElementAt(iIndex))
                    Else
                        .AppendFormat("<Destination type = ""CTY"" code = ""{0}""/>", SearchBatchDetails.SearchItemIDs.ElementAt(iIndex))
                    End If

                    iIndex += 1

                Next

                .Append("</AvailDestinations>")

                .AppendFormat("<StartDate>{0}</StartDate>", SearchDetails.ArrivalDate.ToString("dd/MM/yyyy"))
                .AppendFormat("<EndDate>{0}</EndDate>", SearchDetails.DepartureDate.ToString("dd/MM/yyyy"))
                .AppendFormat("<Currency>{0}</Currency>", _settings.CurrencyCode(SearchDetails))

                Dim sNationality As String = _support.TPNationalityLookup(ThirdParties.TRAVELGATE, SearchDetails.NationalityID)
                If String.IsNullOrEmpty(sNationality) Then
                    sNationality = _settings.DefaultNationality(SearchDetails, False)
                End If

                If sNationality <> "" Then
                    .AppendFormat("<Nationality>{0}</Nationality>", sNationality)
                    .AppendFormat("<Markets><Market>{0}</Market></Markets>", sNationality)
                Else
                    Dim sDefaultNationality As String = _settings.DefaultNationality(SearchDetails, False)
                    If sDefaultNationality <> "" Then
                        .AppendFormat("<Markets><Market>{0}</Market></Markets>", sDefaultNationality)
                    End If
                End If

                Dim sMarkets As String = _settings.Markets(SearchDetails)
                If sMarkets.Length > 0 Then
                    .Append("<Markets>")
                    For Each sMarket As String In sMarkets.Split(","c)
                        .AppendFormat("<Market>{0}</Market>", sMarket)
                    Next
                    .Append("</Markets>")
                ElseIf sNationality <> "" Then
                    .AppendFormat("<Markets><Market>{0}</Market></Markets>", sNationality)
                End If

                .Append("<RoomCandidates>")

                Dim iRoomIndex As Integer = 1
                For Each oRoomDetails As iVector.Search.Property.RoomDetail In SearchDetails.RoomDetails

                    .AppendFormat("<RoomCandidate id = ""{0}"">", iRoomIndex)
                    .Append("<Paxes>")

                    Dim iPaxCount As Integer = 1

                    For i As Integer = 1 To oRoomDetails.Adults
                        .AppendFormat("<Pax age = ""30"" id = ""{0}""/>", iPaxCount)
                        iPaxCount += 1
                    Next

                    If oRoomDetails.Children > 0 Then
                        For Each sChildAge As String In oRoomDetails.ChildAgeCSV.Split(","c)
                            .AppendFormat("<Pax age = ""{0}"" id = ""{1}""/>", sChildAge, iPaxCount)
                            iPaxCount += 1
                        Next
                    End If

                    If oRoomDetails.Infants > 0 Then
                        For i As Integer = 1 To oRoomDetails.Infants
                            .AppendFormat("<Pax age = ""1"" id = ""{0}""/>", iPaxCount)
                            iPaxCount += 1
                        Next
                    End If

                    .Append("</Paxes>")
                    .Append("</RoomCandidate>")

                    iRoomIndex += 1
                Next

                .Append("</RoomCandidates>")

                .Append("</AvailRQ>")
                .Append("</ns:rqXML>")
                .Append("</ns:ProviderRQ>")

                RequestCount += 1

            Next

        End With

    End Sub

    Public Overrides Function TransformResponse(oRequests As List(Of Request), oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit)) As TransformedResultCollection

        Dim oTransformedResults As New TransformedResultCollection()
        Dim oAvailabilityResponses As New List(Of TravelgateSearchResponse)
        Dim oEnvelopeSerializer As New XmlSerializer(GetType(TravelgateResponseEnvelope))
        Dim oResponseSerializer As New XmlSerializer(GetType(TravelgateSearchResponse))

        For Each oRequest As Request In oRequests

            If oRequest.Success Then
                Dim oResponseXML As XmlDocument = CleanXMLNamespaces(oRequest.ResponseXML)

                'Retrieve response envelope
                Dim oResponseEnvelope As New TravelgateResponseEnvelope

                'Deserialize the response Envelope
                Using oReader As TextReader = New StringReader(oResponseXML.InnerXml)
                    oResponseEnvelope = CType(oEnvelopeSerializer.Deserialize(oReader), TravelgateResponseEnvelope)
                End Using

                Dim sResponses As String = oResponseEnvelope.Body.Response.Result.ProviderResults.FirstOrDefault.Results.Result

                'Decoded Xml if response encoded
                If sResponses.Contains("&gt;") Then
                    HttpUtility.HtmlDecode(sResponses)
                End If

                'Deserialize Response Body
                Dim oResponse As TravelgateSearchResponse = New TravelgateSearchResponse()
                Using oReader As TextReader = New StringReader(sResponses)
                    oResponse = CType(oResponseSerializer.Deserialize(oReader), TravelgateSearchResponse)
                End Using

                oAvailabilityResponses.Add(oResponse)
            End If
        Next

        'Extract search results from responses
        oTransformedResults.TransformedResults.AddRange(
                        oAvailabilityResponses.
                            Where(Function(r) r.Hotels.Count > 0).
                            SelectMany(Function(x) GetResultFromResponse(x)))

        Return oTransformedResults

    End Function


#End Region

#Region "ResponseHasExceptions"

    Public Overrides Function ResponseHasExceptions(oRequest As Request) As Boolean
        Return False
    End Function

#End Region

#Region "Helper methods"

    Public Class SearchBatchDetails

        Public Source As String
        Public ResortSplits As List(Of ResortSplit)
        Public SearchByHotel As Boolean
        Public UseZoneSearch As Boolean
        Public BatchSize As Integer
        Public BatchCount As Integer
        Public SearchItemCount As Integer
        Public SearchItemIDs As New List(Of String)

        Public Sub CalculateBatchCount()
            Me.BatchCount = CType(Math.Ceiling(Me.SearchItemCount / Me.BatchSize), Integer)
        End Sub

        Public Sub SetZoneSearchID()
            'Only try to extract the region/zone code if code has 3 parts separated by #
            Dim sCode As String() = Me.ResortSplits(0).ResortCode.Split("#"c)
            If sCode.Length = 3 Then
                Me.SearchItemIDs.Add(sCode(0) & "#" & sCode(1))
            End If
        End Sub

        Public Sub SetSearchIDs()
            'Create a list of all properties/cities that we will be searching (easier to keep track of this way)
            For Each oResortSplit As ResortSplit In Me.ResortSplits
                If Me.SearchByHotel Then
                    For Each oHotel As iVector.Search.Property.Hotel In oResortSplit.Hotels
                        Me.SearchItemIDs.Add(oHotel.TPKey)
                    Next
                Else
                    Me.SearchItemIDs.Add(oResortSplit.ResortCode)
                End If
            Next
        End Sub

    End Class

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

    Private Function GetResultFromResponse(oResponse As TravelgateSearchResponse) As List(Of TransformedResult)
        Dim oTransformedResults As New List(Of TransformedResult)

        For Each oHotel As TravelgateSearchResponse.Hotel In oResponse.Hotels
            For Each oMealPlans As TravelgateSearchResponse.Meal In oHotel.Meals
                For Each oOption As TravelgateSearchResponse.OptionDetails In oMealPlans.Options
                    Dim encryptedParamters As String = EncryptParamters(oOption.Parameters)

                    For Each oRoom As TravelgateSearchResponse.Room In oOption.Rooms
                        Dim oTransformedResult As New TransformedResult
                        With oTransformedResult
                            .TPKey = oHotel.TPKey
                            .CurrencyCode = oOption.Price.Currency
                            .RoomType = oRoom.RoomType
                            .RoomTypeCode = oRoom.RoomTypeCode
                            .MealBasisCode = oMealPlans.MealBaisCode
                            .Amount = oOption.Price.Amount.ToSafeDecimal()
                            .PropertyRoomBookingID = oRoom.PropertyRoomBookingID.ToSafeInt()
                            .CommissionPercentage = oOption.Price.Commission.ToSafeDecimal()
                            .NonRefundableRates = IsNonRefundable(oOption.RateRules, oRoom.NonRefunfable)
                            .FixPrice = IsFixedPrice(oOption.Price.Binding, oOption.Price.Commission)
                            .SellingPrice = GetSellingPrice(oOption.Price.Binding, oOption.Price.Amount)
                            .NetPrice = CalculateNetPrice(oOption.Price.Binding, oOption.Price.Amount, oOption.Price.Commission)
                            .TPRateCode = GetTPRateCode(oResponse.DailyRatePlans)
                            .TPReference = oRoom.ID & "~" & oRoom.RoomTypeCode & "~" &
                                           oRoom.RoomType & "~" & oOption.PaymentType & "~" &
                                           encryptedParamters & "~" & oOption.Price.Commission & "~" &
                                           oOption.Price.Binding & "~" & oMealPlans.MealBaisCode
                        End With

                        oTransformedResults.Add(oTransformedResult)
                    Next
                Next
            Next
        Next

        Return oTransformedResults
    End Function

    Private Function EncryptParamters(oParameters As List(Of TravelgateSearchResponse.Parameter)) As String
        Return Encrypt("<Parameters>" & GetParamters(oParameters) & "</Parameters>")
    End Function

    Private Function GetParamters(oParameters As List(Of TravelgateSearchResponse.Parameter)) As String
        Dim sbParamters As New StringBuilder
        For Each oParameter As TravelgateSearchResponse.Parameter In oParameters
            sbParamters.AppendFormat("<Parameter key=""{0}"" value=""{1}""></Parameter>", oParameter.Key, oParameter.Value)
        Next

        Return sbParamters.ToSafeString
    End Function

    Private Function IsNonRefundable(oRateRules As List(Of TravelgateSearchResponse.RateRule), sIsNonRefundable As String) As Boolean
        If oRateRules.Any() Then
            Return oRateRules.First.Rules.FirstOrDefault.RateType.Equals("NonRefundable") Or
                 (Not String.IsNullOrEmpty(sIsNonRefundable) And sIsNonRefundable.Equals("true"))
        End If

        Return False
    End Function

    Private Function IsFixedPrice(sBinding As String, sComissions As String) As Boolean
        Return sBinding.Equals("true") And (Not sComissions.Equals("-1"))
    End Function

    Private Function GetSellingPrice(sBinding As String, sAmount As String) As String
        If sBinding.Equals("true") Then
            Return sAmount
        End If

        Return "0"
    End Function

    Private Function CalculateNetPrice(sBinding As String, sAmount As String, sComission As String) As String
        If sBinding.Equals("true") Then
            Return (sAmount.ToSafeDecimal * ((100 - sComission.ToSafeDecimal) / 100)).ToSafeString
        End If

        Return "0"
    End Function

    Private Function GetTPRateCode(oDailyRatePlans As List(Of TravelgateSearchResponse.RatePlan)) As String
        If oDailyRatePlans.Any Then
            Return oDailyRatePlans.First.TPRateCode
        End If

        Return String.Empty
    End Function

#End Region

End Class