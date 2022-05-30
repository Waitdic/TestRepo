Imports Intuitive
Imports Intuitive.Net.WebRequests
Imports System.Xml
Imports System.Text
Imports iVector.Search.Property
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Search.Models
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Search.Support
Imports ThirdParty.VBSuppliers.My.Resources
Imports ThirdParty.Abstractions.Results

Public Class YouTravelSearch
    Inherits ThirdPartyPropertySearchBase

#Region "Constructor"

    Public Sub New(settings As IYouTravelSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

#End Region

#Region "Properties"

    Private ReadOnly _settings As IYouTravelSettings

    Private ReadOnly _support As ITPSupport

    Public Overrides Property Source As String = ThirdParties.YOUTRAVEL

    Public Overrides ReadOnly Property SqlRequest As Boolean = False

#End Region

#Region "SearchRestrictions"

    Public Overrides Function SearchRestrictions(oSearchDetails As SearchDetails) As Boolean

        Dim bRestrictions As Boolean = False

        'no more than 3 rooms allowed
        If oSearchDetails.RoomDetails.Count > 3 Then
            bRestrictions = True
        End If

        Return bRestrictions

    End Function

#End Region

#Region "SearchFunctions"

    Public Overrides Function BuildSearchRequests(oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit), bSaveLogs As Boolean) As List(Of Request)

        Dim oRequests As New List(Of Request)

        'if a single resort do that, else find distinct destination and loop through these
        Dim oURLs As New RequestURLs

        If oResortSplits.Count = 1 Then
            If oResortSplits(0).Hotels.Count = 1 Then
                Dim sHotelCode As String = oResortSplits(0).Hotels(0).TPKey
                Dim sHotelURL As String = _settings.SearchURL(oSearchDetails) & String.Format("?HID={0}", sHotelCode)
                oURLs.Add(sHotelURL, "HID", sHotelCode)
            End If

            Dim sResortCode As String = oResortSplits(0).ResortCode.Split("_"c)(1)
            Dim sResortURL As String = _settings.SearchURL(oSearchDetails) &
                String.Format("?RSRT={0}", sResortCode)
            oURLs.Add(sResortURL, "RSRT", sResortCode)

        Else
            Dim oDestinations As List(Of String) = YouTravelSupport.GetDistinctDestinations(oResortSplits)
            For Each sDestination As String In oDestinations
                Dim sDestinationURL As String = _settings.SearchURL(oSearchDetails) & String.Format("?DSTN={0}", sDestination)
                oURLs.Add(sDestinationURL, "DSTN", sDestination)
            Next
        End If

        'build string with addition critera (dates, passengers)
        Dim sbSuffix As New StringBuilder
        sbSuffix.AppendFormat("&LangID={0}", _settings.LangID(oSearchDetails))
        sbSuffix.AppendFormat("&Username={0}", _settings.Username(oSearchDetails))
        sbSuffix.AppendFormat("&Password={0}", _settings.Password(oSearchDetails))
        sbSuffix.AppendFormat("&Checkin_Date={0}", YouTravelSupport.FormatDate(oSearchDetails.PropertyArrivalDate))
        sbSuffix.AppendFormat("&Nights={0}", oSearchDetails.PropertyDuration)
        sbSuffix.AppendFormat("&Rooms={0}", oSearchDetails.Rooms)


        'adults and children
        Dim iRoomIndex As Integer = 0
        For Each oRoom As RoomDetail In oSearchDetails.RoomDetails
            iRoomIndex += 1
            sbSuffix.AppendFormat("&ADLTS_{0}={1}", iRoomIndex, oRoom.Adults)

            If oRoom.Children + oRoom.Infants > 0 Then
                sbSuffix.AppendFormat("&CHILD_{0}={1}", iRoomIndex, oRoom.Children + oRoom.Infants)
                Dim iChildCount As Integer = 1
                For Each iChildAge As Integer In oRoom.ChildAndInfantAges(1)
                    sbSuffix.AppendFormat("&ChildAgeR{0}C{1}={2}", iRoomIndex, iChildCount, iChildAge)
                    iChildCount += 1
                Next
            End If
        Next
        sbSuffix.Append("&CanxPol=1")

        For Each oURL As RequestURL In oURLs

            'set a unique code. if the is one request we only need the source name
            Dim sUniqueCode As String = Me.Source
            If oURLs.Count > 1 Then sUniqueCode = oURL.UniqueRequestID(Me.Source)

            Dim oRequest As New Request
            With oRequest
                .EndPoint = oURL.URL & sbSuffix.ToString
                .Method = eRequestMethod.GET
                .Source = Me.Source
                .LogFileName = "Search"
                .CreateLog = bSaveLogs
                .TimeoutInSeconds = Me.RequestTimeOutSeconds(oSearchDetails)
                .ExtraInfo = New SearchExtraHelper(oSearchDetails, sUniqueCode)
                .UseGZip = True
            End With

            oRequests.Add(oRequest)

        Next

        Return oRequests

    End Function


    Public Overrides Function TransformResponse(oRequests As List(Of Request), oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit)) As TransformedResultCollection

        Dim oXMLs As New List(Of XmlDocument)
        Dim oTransformedXML As New XmlDocument
        Dim iRoomIndex As Integer = 0

        SyncLock (oRequests)
            For Each oRequest As Request In oRequests
                oRequest.ResponseXML.InnerXml = oRequest.ResponseXML.InnerXml.Replace("<?xml version=""1.0"" encoding=""ISO-8859-1""?>", "")
                oXMLs.Add(oRequest.ResponseXML)
            Next
        End SyncLock

        'merge xmldocuments
        Dim oMergedResponses As New XmlDocument
        oMergedResponses = XMLFunctions.MergeXMLDocuments("Results", oXMLs)


        'build child ages xml and merge with response xml
        Dim oXMLBuilder As New Intuitive.Xml.XMLBuilder

        oXMLBuilder.StartNode("ChildAges")
        For Each oRoom As RoomDetail In oSearchDetails.RoomDetails
            iRoomIndex += 1
            oXMLBuilder.StartNode("Room_" & iRoomIndex)
            oXMLBuilder.AddElement("hlpChildAgeCSV", oRoom.ChildAgeCSV)
            oXMLBuilder.EndNode("Room_" & iRoomIndex)
        Next
        oXMLBuilder.EndNode("ChildAges")

        Dim oChildAgesXML As New XmlDocument
        oChildAgesXML.LoadXml(oXMLBuilder.ToString)
        oChildAgesXML.InnerXml = oChildAgesXML.InnerXml.Replace("<?xml version=""1.0""?>", "")
        oMergedResponses = XMLFunctions.MergeXMLDocuments("Results", oChildAgesXML, oMergedResponses)


        'transform response
        Dim oResult As New XmlDocument
        oResult = Intuitive.XMLFunctions.XMLStringTransform(oMergedResponses, YouTravelRes.YouTravel)


        'transform results again
        oTransformedXML = Intuitive.XMLFunctions.XMLStringTransform(oResult, TPRes.result)

        If Not oSearchDetails.StarRating Is Nothing Then
            oSearchDetails.StarRating = oSearchDetails.StarRating.Replace("+", "")
        End If


        Return oTransformedXML

    End Function

#End Region

#Region "ResponseHasExceptions"
    Public Overrides Function ResponseHasExceptions(oRequest As Request) As Boolean
        Return False
    End Function
#End Region

    Private Class RequestURLs
        Inherits List(Of RequestURL)

        Public Overloads Sub Add(ByVal URL As String, ByVal RequestType As String, ByVal RequestID As String)
            Dim oURL As New RequestURL(URL, RequestType, RequestID)
            Me.Add(oURL)
        End Sub

    End Class

    Private Class RequestURL
        Public URL As String = String.Empty
        Public RequestType As String = String.Empty
        Public RequestID As String = String.Empty

        Public Sub New(ByVal URL As String, ByVal RequestType As String, ByVal RequestID As String)
            Me.URL = URL
            Me.RequestType = RequestType
            Me.RequestID = RequestID
        End Sub

        Public Function UniqueRequestID(ByVal Source As String) As String
            Return String.Format("{0}_{1}_{2}", Source, Me.RequestType, Me.RequestID)
        End Function

    End Class

End Class
