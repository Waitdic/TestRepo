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
Imports System.Web.UI
Imports Intuitive
Imports ThirdParty.VBSuppliers.My.Resources
Imports System.Xml.Serialization
Imports System.IO
Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions.Results

Public Class DOTWSearch
    Inherits ThirdPartyPropertySearchBase

#Region "Constructor"

    Private ReadOnly _settings As IDOTWSettings

    Private ReadOnly _support As ITPSupport

    Private ReadOnly _dotwSupport As IDOTWSupport

    Public Sub New(settings As IDOTWSettings, support As ITPSupport, dotwSupport As IDOTWSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
        _dotwSupport = dotwSupport
    End Sub

#End Region

#Region "Properties"

    Public Overrides Property Source As String = ThirdParties.DOTW

    Public Overrides ReadOnly Property SqlRequest As Boolean = False

    Public Overrides ReadOnly Property SupportsNonRefundableTagging As Boolean = False

#End Region

#Region "SearchRestrictions"

    Public Overrides Function SearchRestrictions(oSearchDetails As SearchDetails) As Boolean

        Dim bRestrictions As Boolean = False

        Return bRestrictions

    End Function

#End Region

#Region "SearchFunctions"

    Public Overrides Function BuildSearchRequests(oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit), bSaveLogs As Boolean) As List(Of Request)

        Dim oRequests As New List(Of Request)

        'get cities and sub locations
        Dim oCities As New DOTWSupport.Cities
        For Each oResort As ResortSplit In oResortSplits
            If oResort.ResortCode.Contains("|"c) Then

                Dim iCityNumber As Integer = oResort.ResortCode.Split("|"c)(0).ToSafeInt()
                Dim iLocationID As Integer = oResort.ResortCode.Split("|"c)(1).ToSafeInt()

                If Not iCityNumber = 0 AndAlso oCities.ContainsKey(iCityNumber) Then
                    Dim oCity As DOTWSupport.City = oCities(iCityNumber)
                    If Not oCity.LocationIDs.Contains(iLocationID) Then
                        oCity.LocationIDs.Add(iLocationID)
                    End If
                Else
                    Dim oCity As New DOTWSupport.City(iCityNumber, iLocationID)
                    oCities.Add(iCityNumber, oCity)
                End If

            End If
        Next

        For Each oCityKeyValue As KeyValuePair(Of Integer, DOTWSupport.City) In oCities


            'create the search request for this city
            Dim oCity As DOTWSupport.City = oCityKeyValue.Value

            Dim sRequest As String = Me.BuildSearchRequestXML(oSearchDetails, oCity, _dotwSupport)


            Dim oRequest As New Request
            With oRequest
                .EndPoint = _settings.ServerURL(oSearchDetails)
                .Method = eRequestMethod.POST
                .Source = Me.Source
                .LogFileName = "Search"
                .CreateLog = bSaveLogs
                .TimeoutInSeconds = Me.RequestTimeOutSeconds(oSearchDetails) - 2
                .ExtraInfo = oSearchDetails
                .SetRequest(sRequest)
                .UseGZip = True
            End With

            oRequests.Add(oRequest)

        Next

        Return oRequests

    End Function

    Public Overrides Function TransformResponse(oRequests As List(Of Request), oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit)) As TransformedResultCollection

        Dim oXMLs As New List(Of XmlDocument)
        Dim oTransformedXML As New XmlDocument

        For Each oRequest As Request In oRequests
            oXMLs.Add(oRequest.ResponseXML)
        Next

        'merge responses
        Dim oMergedResponses As New XmlDocument
        oMergedResponses = XMLFunctions.MergeXMLDocuments("Results", oXMLs)


        'room xml
        Dim oRooms As New XmlDocument
        Dim serializer As New XmlSerializer(GetType(RoomDetails))
        Using stringWriter As StringWriter = New StringWriter()
            Using xmlWriter As XmlWriter = XmlWriter.Create(stringWriter)
                serializer.Serialize(xmlWriter, oSearchDetails.RoomDetails)
                oRooms.LoadXml(stringWriter.ToSafeString())
            End Using
        End Using

        'merge them in too
        oMergedResponses = XMLFunctions.MergeXMLDocuments(oMergedResponses, oRooms)


        'transform response
        Dim oTransformedResponse As New XmlDocument


        '7/11/2011 JW - added for version 2
        'DOTW now have a lot of their own third parties which we can exclude in the search if neccessary
        'they also have a minimum selling price for B2C if the element is present otherwise, we can use the normal total which is for B2B
        Dim oXSLParams As New WebControls.XSL.XSLParams
        oXSLParams.AddParam("ExcludeDOTWThirdParties", _settings.ExcludeDOTWThirdParties(oSearchDetails))
        oXSLParams.AddParam("UseMinimumSellingPrice", _settings.UseMinimumSellingPrice(oSearchDetails))
        oXSLParams.AddParam("Duration", oSearchDetails.PropertyDuration)

        'Do the initial Tranformation of the results
        oTransformedResponse = Intuitive.XMLFunctions.XMLStringTransform(oMergedResponses, DOTWRes.DOTWSearchXSL, oXSLParams)


        'Do the Final Transformation ready for TPHotelSearch store
        oTransformedXML = Intuitive.XMLFunctions.XMLStringTransform(oTransformedResponse, DOTWRes.DOTWFinalSearchXSL)


        Return oTransformedXML

    End Function

#End Region

#Region "ResponseHasExceptions"
    Public Overrides Function ResponseHasExceptions(oRequest As Intuitive.Net.WebRequests.Request) As Boolean
        Return False
    End Function
#End Region

#Region "Helpers"

    Public Function BuildSearchRequestXML(ByVal oSearchDetails As SearchDetails, ByVal oCity As DOTWSupport.City, ByVal oSupport As IDOTWSupport) As String

        Dim iSalesChannelID As Integer = oSearchDetails.SalesChannelID
        Dim iBrandID As Integer = oSearchDetails.BrandID

        Dim oSB As New StringBuilder

        With oSB

            .AppendLine("<customer>")
            .AppendFormatLine("<username>{0}</username>", _settings.Username(oSearchDetails))
            .AppendFormatLine("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(oSearchDetails)))
            .AppendFormat("<id>{0}</id>", _settings.CompanyCode(oSearchDetails))
            .AppendLine("<source>1</source>")
            .AppendLine("<product>hotel</product>")
            .AppendLine("<request command=""searchhotels"" debug=""0"">")
            .AppendLine("<bookingDetails>")
            .AppendFormatLine("<fromDate>{0}</fromDate>", oSearchDetails.PropertyArrivalDate.ToString("yyyy-MM-dd"))
            .AppendFormatLine("<toDate>{0}</toDate>", oSearchDetails.PropertyDepartureDate.ToString("yyyy-MM-dd"))
            .AppendFormat("<currency>{0}</currency>", oSupport.GetCachedCurrencyID(oSearchDetails, _support, oSearchDetails.CurrencyCode, _settings))

            .AppendFormat("<rooms no = ""{0}"">", oSearchDetails.Rooms)

            Dim iRoomRunNo As Integer = 0
            For Each oRoomDetail As RoomDetail In oSearchDetails.RoomDetails

                Dim oChildAndInfantAges As Generic.List(Of Integer) = oRoomDetail.ChildAndInfantAges(1).Where(Function(i) i <= 12).ToList

                Dim iAdults As Integer = oRoomDetail.Adults + oRoomDetail.Children + oRoomDetail.Infants - oChildAndInfantAges.Count
                Dim iChildren As Integer = oChildAndInfantAges.Count

                .AppendFormatLine("<room runno = ""{0}"">", iRoomRunNo)
                .AppendFormatLine("<adultsCode>{0}</adultsCode>", iAdults)
                .AppendFormat("<children no = ""{0}"">", iChildren)

                'append the children
                Dim iChildRunNo As Integer = 0
                For Each iChildAge As Integer In oChildAndInfantAges
                    .AppendFormat("<child runno=""{0}"">{1}</child>", iChildRunNo, iChildAge)
                    iChildRunNo += 1
                Next

                .AppendLine("</children>")
                .AppendLine("<extraBed>0</extraBed>")
                .AppendLine("<rateBasis>-1</rateBasis>")

                'Nationality and Country of residence				
                If _settings.Version(oSearchDetails) = "2" Then

                    Dim sNationality As String = DOTW.GetNationality(oSearchDetails.NationalityID, oSearchDetails, _support, _settings)
                    Dim sCountryCode As String = DOTW.GetCountryOfResidence(sNationality, oSearchDetails, _settings)

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
            .AppendLine("</bookingDetails>")
            .AppendLine("<return>")
            .AppendLine("<sorting order = ""asc"">sortByPrice</sorting>")
            .AppendLine("<getRooms>true</getRooms>")
            .AppendLine("<filters xmlns:a = ""http://us.dotwconnect.com/xsd/atomicCondition"" xmlns:c = ""http://us.dotwconnect.com/xsd/complexCondition"">")
            .AppendFormatLine("<city>{0}</city>", oCity.CityNumber)
            .AppendLine("<nearbyCities>false</nearbyCities>")


            'conditions
            .AppendLine("<c:condition>")


            'availability = not on request
            .AppendLine("<a:condition>")
            .AppendLine("<fieldName>onRequest</fieldName>")
            .AppendLine("<fieldTest>equals</fieldTest> ")
            .AppendLine("<fieldValues>")
            .AppendLine("<fieldValue>0</fieldValue> ")
            .AppendLine("</fieldValues>")
            .AppendLine("</a:condition>")


            'put in the city locations
            'only need to do this is there is more than one location
            If oCity.LocationIDs.Count > 1 Then
                .AppendLine("<operator>AND</operator>")
                .AppendLine("<a:condition>")
                .AppendLine("<fieldName>locationId</fieldName>")
                .AppendLine("<fieldTest>in</fieldTest>")
                .AppendLine("<fieldValues>")
                For Each iLocationID As Integer In oCity.LocationIDs
                    .AppendFormat("<fieldValue>{0}</fieldValue>", iLocationID)
                Next
                .AppendLine("</fieldValues>")
                .AppendLine("</a:condition>")
            End If

            .AppendLine("</c:condition>")
            .AppendLine("</filters>")
            .AppendLine("<fields>")
            .AppendLine("<field>hotelName</field>")
            .AppendLine("<field>noOfRooms</field>")
            .AppendLine("<roomField>name</roomField>")
            .AppendLine("<roomField>including</roomField>")
            .AppendLine("<roomField>minStay</roomField>")
            .AppendLine("</fields>")
            .AppendLine("</return>")
            .AppendLine("</request>")
            .AppendLine("</customer>")
        End With

        Return oSB.ToString

    End Function

#End Region

End Class