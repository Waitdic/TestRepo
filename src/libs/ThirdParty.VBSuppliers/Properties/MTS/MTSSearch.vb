Imports System.Xml
Imports System.Text
Imports iVector.Search.Property
Imports Intuitive.Net.WebRequests
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Search.Models
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Search.Support
Imports Intuitive
Imports ThirdParty.Abstractions.Results

Public Class MTSSearch
    Inherits ThirdPartyPropertySearchBase

#Region "Constructor"

    Public Sub New(settings As IMTSSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

#End Region

#Region "Properties"

    Private ReadOnly _settings As IMTSSettings

    Private ReadOnly _support As ITPSupport

    Public Overrides Property Source As String = ThirdParties.MTS

    Public Overrides ReadOnly Property SqlRequest As Boolean = False

#End Region

#Region "SearchRestrictions"

    Public Overrides Function SearchRestrictions(oSearchDetails As SearchDetails) As Boolean

        Dim bRestrictions As Boolean = False

        'multiroom searches not supported until March 2012
        'If oSearchDetails.RoomDetails.Count > 1 Then
        '	bRestrictions = True
        'End If


        Return bRestrictions

    End Function

#End Region

#Region "SearchFunctions"

    Public Overrides Function BuildSearchRequests(
        oSearchDetails As SearchDetails,
        oResortSplits As List(Of ResortSplit),
        bSaveLogs As Boolean) As List(Of Request)

        Dim oRequests As New List(Of Request)
        Dim aRegions As New Dictionary(Of String, String)

        Dim overrideCountriesList As List(Of String) = New List(Of String)
        If overrideCountriesList.Count = 0 Then
            Dim sOverrideCountries As String = _settings.OverrideCountries(oSearchDetails)
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

        If oResortSplits.Count = 1 Then
            'how many hotels? if >1, search by resort, if =1 search by hotelcode
            If oResortSplits(0).Hotels.Count = 1 Then
                Dim sCountry As String = oResortSplits(0).ResortCode.Split("|"c)(0)
                Dim sHotelCode As String = oResortSplits(0).Hotels(0).TPKey
                Dim sCountryAndHotelCode As String = sCountry + "|" + sHotelCode

                aRegions.Add(sCountryAndHotelCode, "CountryAndHotelCode")

            End If

            If oResortSplits(0).Hotels.Count > 1 Then

                Dim sResortPath As String = oResortSplits(0).ResortCode

                'save resort if new
                If aRegions.ContainsKey(sResortPath) = False Then
                    aRegions.Add(sResortPath.ToString, "Resort")
                End If
            End If
        End If

        If oResortSplits.Count > 1 Then
            'select region and save; for each new different region, save that region as well
            For Each oResortSplit As ResortSplit In oResortSplits

                'select region
                Dim sCountry As String = oResortSplit.ResortCode.Split("|"c)(0)
                Dim sRegion As String = oResortSplit.ResortCode.Split("|"c)(1)
                Dim sRegionPath As String = sCountry + "|" + sRegion

                'save region if new
                If aRegions.ContainsKey(sRegionPath) = False Then
                    aRegions.Add(sRegionPath, "Region")
                End If

            Next
        End If

        'need to send off a request for each resort and store them in an array
        'build request
        For Each oSearch As KeyValuePair(Of String, String) In aRegions

            'get the third party resorts
            'once get IPs confirmed, ie not now
            Dim aSearchKey As String() = oSearch.Key.Split("|"c)

            Dim useOverrideId As Boolean = overrideCountriesList.Contains(oSearch.Key.Split("|"c)(0))

            'build the request string
            Dim sbRequest As New StringBuilder

            With sbRequest
                .Append("<OTA_HotelAvailRQ xmlns = ""http://www.opentravel.org/OTA/2003/05"" Version = ""0.1"">")
                .Append("<POS>")
                .Append("<Source>")

                If useOverrideId Then

                    'If country is Egypt, Turkey or UAE, search with second ID. This returns in contracted currency
                    .AppendFormat("<RequestorID Instance = ""{0}"" ID_Context = ""{1}"" ID = ""{2}"" Type = ""{3}""/>",
                       _settings.Instance(oSearchDetails), _settings.ID_Context(oSearchDetails),
                        _settings.OverRideID(oSearchDetails), _settings.Type(oSearchDetails))

                Else
                    'Anything else, search with main ID; returns in Euros
                    .AppendFormat("<RequestorID Instance = ""{0}"" ID_Context = ""{1}"" ID = ""{2}"" Type = ""{3}""/>",
                        _settings.Instance(oSearchDetails), _settings.ID_Context(oSearchDetails),
                        _settings.ID(oSearchDetails), _settings.Type(oSearchDetails))

                End If
                .Append("<BookingChannel Type = ""2""/>")
                .Append("</Source>")

                .Append("<Source>")
                .AppendFormat("<RequestorID Type=""{0}"" ID=""{1}"" MessagePassword=""{2}""/>",
                    _settings.AuthenticationType(oSearchDetails),
                    _settings.AuthenticationID(oSearchDetails),
                    _settings.MessagePassword(oSearchDetails))
                .Append("</Source>")

                .Append("</POS>")

                .Append("<AvailRequestSegments>")
                .Append("<AvailRequestSegment InfoSource='1*2*4*5*'>")
                .AppendFormat("<StayDateRange End=""{0}"" Start=""{1}""></StayDateRange>",
                              oSearchDetails.PropertyDepartureDate.ToString("yyyy-MM-dd"),
                              oSearchDetails.PropertyArrivalDate.ToString("yyyy-MM-dd"))
                .Append("<RoomStayCandidates>")
                'loop through the rooms
                Dim iRoomCount As Integer = 0
                For Each oRoomBooking As RoomDetail In oSearchDetails.RoomDetails
                    .AppendFormat("<RoomStayCandidate RPH=""{0}"">", iRoomCount + 1)
                    .Append("<GuestCounts>")

                    'Adults
                    .Append("<GuestCount AgeQualifyingCode=""10"" ")
                    .AppendFormat(" Count=""{0}"" ", oRoomBooking.Adults)
                    .Append("></GuestCount>")

                    'Children
                    For Each iChildAge As Integer In oRoomBooking.ChildAges
                        .Append("<GuestCount AgeQualifyingCode=""8"" ")
                        .AppendFormat("Age=""{0}"" ", iChildAge)
                        .AppendFormat("Count=""1""")
                        .Append("></GuestCount>")
                    Next

                    'Infants
                    If oRoomBooking.Infants > 0 Then
                        .Append("<GuestCount AgeQualifyingCode=""7"" ")
                        .AppendFormat("Age=""1"" ")
                        .AppendFormat("Count=""{0}""", oRoomBooking.Infants)
                        .Append("></GuestCount>")
                    End If

                    .Append("</GuestCounts>")
                    .Append("</RoomStayCandidate>")

                    iRoomCount += 1
                Next

                .Append("</RoomStayCandidates>")
                .Append("<HotelSearchCriteria>")

                If oSearch.Value = "CountryAndHotelCode" Then

                    .Append("<Criterion ExactMatch = ""true"">")
                    .AppendFormat("<HotelRef HotelCode=""{0}""></HotelRef>", oSearch.Key.Split("|"c)(1))

                Else
                    .Append("<Criterion>")
                    .AppendFormat("<RefPoint CodeContext = ""Country"">{0}</RefPoint>", oSearch.Key.Split("|"c)(0))
                    .AppendFormat("<RefPoint CodeContext = ""Region"">{0}</RefPoint>", oSearch.Key.Split("|"c)(1))

                    'Check if is a resort-level search

                    If oSearch.Value = "Resort" Then
                        .AppendFormat("<RefPoint CodeContext = ""Resort"">{0}</RefPoint>", oSearch.Key.Split("|"c)(2))
                    End If
                End If

                .Append("</Criterion>")
                .Append("</HotelSearchCriteria>")
                .Append("</AvailRequestSegment>")
                .Append("</AvailRequestSegments>")
                .Append("</OTA_HotelAvailRQ>")

            End With

            'if we have one regions then we can simply use the source here
            Dim sUniqueCode As String = Me.Source

            'unless there is more than one
            If aRegions.Count > 1 Then
                'get a "unique" key for this thread (if regions and resort names overlap then we are in trouble)
                Dim sUniqueCodeKey As String = "All"
                If aSearchKey.Length > 0 Then sUniqueCodeKey = aSearchKey(aSearchKey.Length - 1)
                sUniqueCode = String.Format("{0}_{1}", Me.Source, sUniqueCodeKey)
            End If

            Dim oRequest As New Request
            With oRequest
                .EndPoint = _settings.BaseURL(oSearchDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.MTS
                .LogFileName = "Search"
                .CreateLog = True
                .TimeoutInSeconds = Me.RequestTimeOutSeconds(oSearchDetails)
                .ExtraInfo = New SearchExtraHelper(oSearchDetails, sUniqueCode)
                .SetRequest(sbRequest.ToString)
                .UseGZip = True
                .ContentType = ContentTypes.Application_json
            End With

            oRequests.Add(oRequest)

        Next

        Return oRequests

    End Function

    Public Overrides Function TransformResponse(
        oRequests As List(Of Request),
        oSearchDetails As SearchDetails,
        oResortSplits As List(Of ResortSplit)) As TransformedResultCollection

        Dim oXMLs As New List(Of XmlDocument)
        Dim oTransformedXML As New XmlDocument
        Dim iSalesChannelId As Integer = oSearchDetails.SalesChannelID
        Dim iBrandId As Integer = oSearchDetails.BrandID


        For Each oRequest As Request In oRequests
            SyncLock (oRequest)
                oRequest.ResponseXML.InnerXml = oRequest.ResponseXML.InnerXml.Replace(" xmlns=""http://www.opentravel.org/OTA/2003/05""", "")
                oRequest.ResponseXML.InnerXml = Intuitive.XMLFunctions.CleanXMLNamespaces(oRequest.ResponseXML.InnerXml).InnerXml
            End SyncLock
            oXMLs.Add(oRequest.ResponseXML)
        Next

        'merge XML documents
        Dim oMergedResponses As New XmlDocument
        oMergedResponses = XMLFunctions.MergeXMLDocuments("Results", oXMLs)


        Dim iNumberOfRooms As Integer = oSearchDetails.Rooms
        Dim oXSLParams As New WebControls.XSL.XSLParams

        oXSLParams.AddParam("NumberOfRooms", iNumberOfRooms)


        'transform the results
        oTransformedXML = XMLFunctions.XMLStringTransform(oMergedResponses, MTSRes.MTSRes.MTSSearchXSL, oXSLParams)

        Return oTransformedXML

    End Function

#End Region

#Region "ResponseHasExceptions"

    Public Overrides Function ResponseHasExceptions(oRequest As Request) As Boolean
        Return False
    End Function

#End Region

End Class