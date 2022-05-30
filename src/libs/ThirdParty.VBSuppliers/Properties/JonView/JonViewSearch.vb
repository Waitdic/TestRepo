Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive
Imports Intuitive.Net.WebRequests
Imports iVector.Search.Property
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Results
Imports ThirdParty.Abstractions.Search.Models
Imports Intuitive.Helpers.Extensions

Public Class JonViewSearch
    Inherits ThirdPartyPropertySearchBase

#Region "Properties"

    Private Property _settings As IJonViewSettings

    Private Property _support As ITPSupport


    Public Overrides Property Source As String = ThirdParties.JONVIEW

    Public Overrides ReadOnly Property SqlRequest As Boolean = False

    Public Sub New(settings As IJonViewSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

    Public Overrides ReadOnly Property SupportsNonRefundableTagging As Boolean = False

#End Region

#Region "SearchRestrictions"

    Public Overrides Function SearchRestrictions(oSearchDetails As SearchDetails) As Boolean

        Dim bRestrictions As Boolean = False

        If oSearchDetails.RoomDetails.Count > 1 Then

            Dim iAdults As Integer = oSearchDetails.RoomDetails(0).Adults
            Dim iChildren As Integer = oSearchDetails.RoomDetails(0).Children
            Dim iInfants As Integer = oSearchDetails.RoomDetails(0).Infants
            Dim sChildAgesCSV As String = oSearchDetails.RoomDetails(0).ChildAgeCSV

            For Each oRoomDetails As RoomDetail In oSearchDetails.RoomDetails
                If Not (oRoomDetails.Adults = iAdults AndAlso oRoomDetails.Children = iChildren _
                  AndAlso oRoomDetails.Infants = iInfants AndAlso oRoomDetails.ChildAgeCSV = sChildAgesCSV) Then
                    bRestrictions = True
                End If
            Next


        End If

        Return bRestrictions

    End Function

#End Region

#Region "SearchFunctions"

    Public Overrides Function BuildSearchRequests(oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit), bSaveLogs As Boolean) As List(Of Intuitive.Net.WebRequests.Request)

        Dim oRequests As New Generic.List(Of Intuitive.Net.WebRequests.Request)

        'build request xml for each resort
        For Each oResort As ResortSplit In oResortSplits

            Dim sCityCode As String = oResort.ResortCode

            'Build request url
            Dim url As String = BuildSearchURL(oSearchDetails, sCityCode)

            Dim oRequest As New Request
            With oRequest
                .EndPoint = _settings.URL(oSearchDetails) & url
                .Method = eRequestMethod.POST
                .Source = Me.Source
                .LogFileName = "Search"
                .CreateLog = bSaveLogs
                .TimeoutInSeconds = Me.RequestTimeOutSeconds(oSearchDetails)
                .ExtraInfo = oSearchDetails
                .UseGZip = True
            End With

            oRequests.Add(oRequest)

        Next

        Return oRequests

    End Function

    Public Overrides Function TransformResponse(oRequests As List(Of Request), oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit)) As TransformedResultCollection

        Dim transformedResults As New TransformedResultCollection()
        Dim jonviewSearchResponses As New List(Of JonViewSearchResponse)
        Dim serializer As New XmlSerializer(GetType(JonViewSearchResponse))

        For Each request As Request In oRequests

            Dim searchResponse As New JonViewSearchResponse
            Dim success As Boolean = request.Success

            If success Then
                Using reader As TextReader = New StringReader(request.ResponseString)
                    searchResponse = CType(serializer.Deserialize(reader), JonViewSearchResponse)
                End Using
                If searchResponse.Response IsNot Nothing Then
                    jonviewSearchResponses.Add(searchResponse)
                End If
            End If

        Next

        transformedResults.TransformedResults.AddRange(
            jonviewSearchResponses.
                Where(Function(r) r.Response.Rooms.Count > 0).
                SelectMany(Function(x) GetResultFromResponse(x)))

        Return transformedResults

    End Function

#End Region

#Region "ResponseHasExceptions"
    Public Overrides Function ResponseHasExceptions(oRequest As Intuitive.Net.WebRequests.Request) As Boolean
        Return False
    End Function
#End Region

#Region "Helper classes"

    Private Function BuildSearchURL(searchDetails As SearchDetails, cityCode As String) As String

        Dim url As New StringBuilder

        url.AppendFormat("?actioncode=HOSTXML&clientlocseq={0}&userid={1}&" &
                        "password={2}&message=<?xml version=""1.0"" encoding=""UTF-8""?>",
                        _settings.ClientLoc(searchDetails), _settings.UserID(searchDetails), _settings.Password(searchDetails))

        url.AppendFormat("<message><actionseg>CT</actionseg><searchseg><prodtypecode>FIT</prodtypecode>" &
                         "<searchtype>CITY</searchtype>")
        url.AppendFormat("<citycode>{0}</citycode>", cityCode)
        url.AppendFormat("<startdate>{0}</startdate>", searchDetails.PropertyArrivalDate.ToString("dd-MMM-yyyy"))
        url.AppendFormat("<duration>{0}</duration>", searchDetails.PropertyDuration)
        url.AppendFormat("<status>AVAILABLE</status>")
        url.AppendFormat("<displayname>Y</displayname>")
        url.AppendFormat("<displaynamedetails>Y</displaynamedetails>")
        url.AppendFormat("<displayroomconf>Y</displayroomconf>")
        url.AppendFormat("<displayprice>Y</displayprice>")
        url.AppendFormat("<displaysuppliercd>Y</displaysuppliercd>")
        url.AppendFormat("<displayavail>Y</displayavail>")
        url.AppendFormat("<displaypolicy>Y</displaypolicy>")
        url.AppendFormat("<displayrestriction>Y</displayrestriction>")
        url.AppendFormat("<displaydynamicrates>Y</displaydynamicrates>")

        Dim room As RoomDetail = searchDetails.RoomDetails(0)
        Dim childAges As New StringBuilder
        childAges.Append(room.ChildAgeCSV.Replace(",", "/"))
        For i As Integer = 1 To room.Infants
            childAges.Append("/1")
        Next
        Dim sChildAges As String = childAges.ToString
        If Not sChildAges = "" AndAlso sChildAges.Substring(0, 1) = "/" Then
            sChildAges = sChildAges.Substring(1, sChildAges.Length - 1)
        End If

        url.AppendFormat("<adults>{0}</adults>", room.Adults)
        url.AppendFormat("<children>{0}</children>", room.Children + room.Infants)
        url.AppendFormat("<childrenage>{0}</childrenage>", childAges)
        url.AppendFormat("<displaypolicy>Y</displaypolicy>")
        url.AppendFormat("</searchseg>")
        url.AppendFormat("</message>")

        'Add the request body

        Return url.ToSafeString()

    End Function

    Private Function GetResultFromResponse(response As JonViewSearchResponse) As List(Of TransformedResult)
        Dim transformedResults As New List(Of TransformedResult)

        For Each room As JonViewSearchResponse.Room In response.Response.Rooms
            Dim transformedResult As New TransformedResult
            With transformedResult
                .TPKey = room.suppliercode
                .CurrencyCode = room.currencycode
                .RoomTypeCode = GetRoomType(room.productname)
                .MealBasisCode = "RO"
                .Amount = GetPrice(room.dayprice)
                .PropertyRoomBookingID = 1
                .TPReference = room.prodcode & "_" & room.dayprice
                .NonRefundableRates = room.cancellationPolicy.item.fromdays.Equals("999")
                .RoomType = room.roomDetails.roomtype
            End With

            transformedResults.Add(transformedResult)
        Next

        Return transformedResults
    End Function

    Private Function GetRoomType(productName As String) As String
        If productName.Split("-"c).Length > 1 Then
            Return productName.Split("-"c)(productName.Split("-"c).Length - 1).Trim
        Else
            Return "Standard Room"
        End If
    End Function

    Private Function GetPrice(dayPrice As String) As Decimal
        Dim price As Decimal = 0
        For Each sPrice As String In dayPrice.Split("/"c)
            price += sPrice.ToSafeDecimal()
        Next

        Return price
    End Function

#End Region

End Class
