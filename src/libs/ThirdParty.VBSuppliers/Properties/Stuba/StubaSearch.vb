Imports System.Xml
Imports Intuitive
Imports Intuitive.Helpers.Extensions
Imports Intuitive.Net.WebRequests
Imports iVector.Search.Property
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Results
Imports ThirdParty.Abstractions.Search.Models
Imports ThirdParty.Abstractions.Search.Support
Imports ThirdParty.VBSuppliers.My.Resources

Public Class StubaSearch
    Inherits ThirdPartyPropertySearchBase

    Private ReadOnly _settings As IStubaSettings

    Private ReadOnly _support As ITPSupport


    Public Overrides Property Source As String = ThirdParties.STUBA

    Public Overrides ReadOnly Property SqlRequest As Boolean = False

    Public Sub New(settings As IStubaSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

    Public Overrides Function BuildSearchRequests(oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit), bSaveLogs As Boolean) As List(Of Request)

        Dim oRequests As New List(Of Request)

        Dim requestBodies As New List(Of String)
        For Each resortSplit As ResortSplit In oResortSplits.Where(Function(rs) Not rs.ResortCode.Contains("|"))
            requestBodies.AddRange(BuildRequests(oSearchDetails, resortSplit.ResortCode, resortSplit.Hotels.Select(Function(h) h.TPKey).ToList))
        Next

        Dim resortCodesGroupedByCityCode As ILookup(Of String, String) =
            oResortSplits.Where(Function(rs) rs.ResortCode.Contains("|")) _
                         .ToLookup(Function(rs) rs.ResortCode.Split("|"c)(0),
                                   Function(rs) rs.ResortCode.Split("|"c)(1))

        For Each resortCodeGroup As IGrouping(Of String, String) In resortCodesGroupedByCityCode
            Dim cityCode As String = resortCodeGroup.Key
            Dim regionIdToUse As String
            Dim hotelIDs As IEnumerable(Of String)
            If resortCodeGroup.Count() > 1 Then
                regionIdToUse = cityCode
                hotelIDs = From rs As ResortSplit In oResortSplits.Where(Function(rs) rs.ResortCode.Split("|"c)(0) = regionIdToUse)
                           From hotel As Hotel In rs.Hotels
                           Select hotel.TPKey
            Else
                regionIdToUse = resortCodeGroup.Single()
                hotelIDs = oResortSplits.Single(Function(rs) rs.ResortCode = $"{cityCode}|{regionIdToUse}").Hotels.Select(Function(h) h.TPKey)
            End If
            requestBodies.AddRange(BuildRequests(oSearchDetails, regionIdToUse, hotelIDs.ToList))
        Next

        For Each request As String In requestBodies
            Dim oRequest As New Request
            With oRequest
                .EndPoint = _settings.URL(oSearchDetails)
                .SetRequest(request)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.STUBA
                .TimeoutInSeconds = Me.RequestTimeOutSeconds(oSearchDetails)
                .LogFileName = "Search"
                .CreateLog = bSaveLogs
                .ExtraInfo = New SearchExtraHelper() With {.SearchDetails = oSearchDetails}
                .UseGZip = True
            End With
            oRequests.Add(oRequest)
        Next
        Return oRequests
    End Function

    Private Iterator Function BuildRequests(oSearchDetails As SearchDetails, sResortCode As String, oHotelIDs As List(Of String)) As IEnumerable(Of String)
        Dim iMaxHotels As Integer = _settings.MaxHotelsPerRequest(oSearchDetails)
        If iMaxHotels <= 0 Then
            Yield BuildRequest(oSearchDetails, sResortCode, oHotelIDs)
        Else
            Dim iBatchCount As Integer = Math.Ceiling(oHotelIDs.Count / iMaxHotels).ToSafeInt()
            For i As Integer = 0 To iBatchCount - 1
                Yield BuildRequest(oSearchDetails, sResortCode, oHotelIDs.Skip(i * iMaxHotels).Take(iMaxHotels))
            Next
        End If
    End Function

    Private Function BuildRequest(oSearchDetails As SearchDetails, sResortCode As String, oHotelIDs As IEnumerable(Of String)) As String
        Dim sOrg As String = _settings.Organisation(oSearchDetails)
        Dim sUser As String = _settings.Username(oSearchDetails)
        Dim sPassword As String = _settings.Password(oSearchDetails)
        Dim sVersion As String = _settings.Version(oSearchDetails)
        Dim sCurrencyCode As String = _settings.Currency(oSearchDetails)
        Dim sNationality As String = _settings.Nationality(oSearchDetails)

        Dim request As XElement =
            <AvailabilitySearch>
                <Authority>
                    <Org><%= sOrg %></Org>
                    <User><%= sUser %></User>
                    <Password><%= sPassword %></Password>
                    <Currency><%= sCurrencyCode %></Currency>
                    <Version><%= sVersion %></Version>
                </Authority>
                <RegionId><%= sResortCode %></RegionId>
                <Hotels>
                    <%= From id As String In oHotelIDs
                        Select <Id><%= id %></Id>
                    %>
                </Hotels>
                <HotelStayDetails>
                    <ArrivalDate><%= oSearchDetails.PropertyArrivalDate.ToString("yyyy-MM-dd") %></ArrivalDate>
                    <Nights><%= oSearchDetails.PropertyDuration %></Nights>
                    <Nationality><%= sNationality %></Nationality>
                    <%= From oRoom As RoomDetail In oSearchDetails.RoomDetails
                        Select <Room>
                                   <Guests>
                                       <%= From adult As Integer In Enumerable.Range(0, oRoom.Adults)
                                           Select <Adult/>
                                       %>
                                       <%= From childAge As Integer In oRoom.ChildAges
                                           Select <Child age=<%= childAge %>/>
                                       %>
                                       <%= From infant As Integer In Enumerable.Range(0, oRoom.Infants)
                                           Select <Child age="0"/>
                                       %>
                                   </Guests>
                               </Room>
                    %>
                </HotelStayDetails>
            </AvailabilitySearch>
        Return request.ToString()
    End Function

    Public Overrides Function TransformResponse(oRequests As List(Of Request), searchDetails As SearchDetails, oResortSplits As List(Of ResortSplit)) As TransformedResultCollection

        Dim filteredResponses As New List(Of XmlDocument)
        For Each webRq As Request In oRequests
            Dim responseXml As XDocument = XDocument.Parse(webRq.ResponseString)
            Dim removeNonRefundable As Boolean =
                _settings.ExcludeNonRefundableRates(searchDetails)
            Dim removeUnknownCancellations As Boolean =
                _settings.ExcludeUnknownCancellationPolicys(searchDetails)

            If removeNonRefundable Then
                RemoveNrfResults(responseXml)
            End If
            If removeUnknownCancellations Then
                RemoveUnknownCancResults(responseXml)
            End If
            If searchDetails.Rooms > 1 Then
                RemoveNonCheapestRoomCombinations(responseXml)
            End If

            filteredResponses.Add(responseXml.ToXmlDocument())
        Next
        Dim merged As XmlDocument = XMLFunctions.MergeXMLDocuments("Results", filteredResponses)

        Dim params As New XSLParams
        params.AddParam("RoomCount", searchDetails.Rooms.ToString())
        Return XMLFunctions.XMLStringTransform(merged, StubaSearchRes.StubaSearchXSL, params)
    End Function

    Private Sub RemoveUnknownCancResults(response As XDocument)
        RemoveResultsByCancellationPolicy(response, "Unknown")
    End Sub

    Private Sub RemoveNrfResults(response As XDocument)
        RemoveResultsByCancellationPolicy(response, "NonRefundable")
    End Sub

    Private Sub RemoveResultsByCancellationPolicy(ByVal response As XDocument, ByVal policyStatus As String)
        response.Elements("AvailabilitySearchResult") _
                .Elements("HotelAvailability") _
                .Elements("Result") _
                .Where(Function(r) r.Elements("Room") _
                                    .Elements("CancellationPolicyStatus") _
                                    .Any(Function(cps) cps.Value.ToLower() = policyStatus.ToLower())) _
                .Remove()
    End Sub

    Private Sub RemoveNonCheapestRoomCombinations(response As XDocument)
        For Each hotel As XElement In response.Elements("AvailabilitySearchResult").Elements("HotelAvailability")
            hotel.Elements("Result") _
                 .OrderBy(Function(r) r.Elements("Price") _
                                       .Sum(Function(p) p.Attribute("amt").Value.ToSafeDecimal())) _
                 .Skip(1) _
                 .Remove()
        Next
    End Sub

    Public Overrides Function SearchRestrictions(oSearchDetails As SearchDetails) As Boolean
        Return False
    End Function

    Public Overrides Function ResponseHasExceptions(oRequest As Request) As Boolean
        Return False
    End Function
End Class
