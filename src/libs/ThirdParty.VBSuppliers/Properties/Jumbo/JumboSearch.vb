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
Imports ThirdParty.VBSuppliers.My.Resources
Imports ThirdParty.Abstractions.Results

Public Class JumboSearch
    Inherits ThirdPartyPropertySearchBase

#Region "Constructor"

    Public Sub New(settings As IJumboSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

#End Region

#Region "Properties"

    Public Overrides Property Source As String = ThirdParties.JUMBO

    Private Property _settings As IJumboSettings

    Private Property _support As ITPSupport

    Public Overrides ReadOnly Property SqlRequest As Boolean = False

    Public Overrides ReadOnly Property SupportsNonRefundableTagging As Boolean = False

#End Region

#Region "SearchRestrictions"

    Public Overrides Function SearchRestrictions(oSearchDetails As SearchDetails) As Boolean

        Dim bRestrictions As Boolean = False

        'Multi rooms was implemented however did not function correctly, Jumbo returns room combinations rather than available rooms
        If oSearchDetails.Rooms > 1 Then
            bRestrictions = True
        End If

        Return bRestrictions

    End Function

#End Region

#Region "SearchFunctions"

    Public Overrides Function BuildSearchRequests(oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit), bSaveLogs As Boolean) As List(Of Request)

        Dim oRequests As New List(Of Request)

        Dim sb As New StringBuilder
        Dim sHotelID As String = ""
        Dim sURL As String = _settings.HotelBookingURL(oSearchDetails)
        Dim sbSoapOpeningTags As New StringBuilder
        With sbSoapOpeningTags
            sbSoapOpeningTags.Append("<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:typ=""http://xtravelsystem.com/v1_0rc1/hotel/types"">")
            sbSoapOpeningTags.Append("<soapenv:Header/>")
            sbSoapOpeningTags.Append("<soapenv:Body>")
        End With
        Dim oTPPropertySearchRequest As New TPPropertySearchRequest
        oTPPropertySearchRequest.AppendOpenSOAPRequestString("typ:availableHotelsByMultiQueryV12", sbSoapOpeningTags.ToString, "soapenv")
        sb = oTPPropertySearchRequest.sbRequestBuilder

        sb.Append("<AvailableHotelsByMultiQueryRQ_1>")
        sb.AppendFormat("<agencyCode>{0}</agencyCode>", Jumbo.GetCredentials(oSearchDetails, oSearchDetails.LeadGuestNationalityID, "AgencyCode", _settings))
        sb.AppendFormat("<brandCode>{0}</brandCode>", Jumbo.GetCredentials(oSearchDetails, oSearchDetails.LeadGuestNationalityID, "BrandCode", _settings))
        sb.AppendFormat("<pointOfSaleId>{0}</pointOfSaleId>", Jumbo.GetCredentials(oSearchDetails, oSearchDetails.LeadGuestNationalityID, "POS", _settings))
        sb.AppendFormat("<checkin>{0}</checkin>", Format(oSearchDetails.PropertyArrivalDate, "yyyy-MM-ddThh:mm:ss"))
        sb.AppendFormat("<checkout>{0}</checkout>", Format(oSearchDetails.PropertyDepartureDate, "yyyy-MM-ddThh:mm:ss"))
        sb.AppendFormat("<fromPrice>{0}</fromPrice>", "0")
        sb.AppendFormat("<fromRow>{0}</fromRow>", "0")
        sb.AppendFormat("<includeEstablishmentData>{0}</includeEstablishmentData>", "false")
        sb.AppendFormat("<language>{0}</language>", _settings.Language(oSearchDetails))
        sb.AppendFormat("<maxRoomCombinationsPerEstablishment>{0}</maxRoomCombinationsPerEstablishment>", "10")
        sb.AppendFormat("<numRows>{0}</numRows>", "1000")

        For Each oRoom As RoomDetail In oSearchDetails.RoomDetails
            sb.Append("<occupancies>")
            sb.AppendFormat("<adults>{0}</adults>", oRoom.Adults)
            sb.AppendFormat("<children>{0}</children>", (oRoom.Children + oRoom.Infants))
            For Each iChildAge As Integer In oRoom.ChildAndInfantAges
                sb.AppendFormat("<childrenAges>{0}</childrenAges>", iChildAge)
            Next

            sb.AppendFormat("<numberOfRooms>{0}</numberOfRooms>", "1")
            sb.Append("</occupancies>")
        Next

        sb.AppendFormat("<onlyOnline>{0}</onlyOnline>", "true")
        sb.Append("<orderBy xsi:nil=""true"" />")
        sb.Append("<productCode xsi:nil=""true"" />")
        sb.AppendFormat("<toPrice>{0}</toPrice>", "9999")

        For Each oResortSplit As ResortSplit In oResortSplits

            Dim sResortCode As String = oResortSplit.ResortCode

            If oResortSplit.Hotels.Count = 1 And oResortSplits.Count = 1 Then
                sHotelID = oResortSplit.Hotels(0).TPKey.ToString
            Else
                sHotelID = "0"
            End If

            If sHotelID.ToString <> "0" AndAlso sHotelID.ToString <> "" Then
                sb.AppendFormat("<establishmentId>{0}</establishmentId>", sHotelID.ToString)
            Else
                sb.AppendFormat("<areaCode>{0}</areaCode>", sResortCode.ToString)
            End If

        Next

        sb.Append("</AvailableHotelsByMultiQueryRQ_1>")
        oTPPropertySearchRequest.AppendCloseRequestString()

        Dim oRequest As New Request
        oRequest = oTPPropertySearchRequest.Create(sURL, Me.Source, bSaveLogs, Me.RequestTimeOutSeconds(oSearchDetails) - 2, oSearchDetails, ResponseCallBack)

        oRequests.Add(oRequest)

        Return oRequests

    End Function

    Public Overrides Function TransformResponse(oRequests As List(Of Request), oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit)) As TransformedResultCollection

        Dim oTransformedXML As New XmlDocument
        Dim sResponse As String = XMLFunctions.CleanXMLNamespaces(oRequests(0).ResponseXML).InnerXml.ToString

        Dim oResponse As New XmlDocument

        If Not sResponse.Contains("<faultcode>") Then
            oResponse.LoadXml(sResponse.Replace("&", "&amp;"))

            'transform response
            Dim oResult As New XmlDocument
            oResult = Intuitive.XMLFunctions.XMLStringTransform(oResponse, JumboRes.JumboSearch)

            'transform results again
            oTransformedXML = Intuitive.XMLFunctions.XMLStringTransform(oResult, TPRes.result)

            'dedupe the results
            oResult = DeDupeJumboResults(oResult)

        End If

        Return oTransformedXML

    End Function

#End Region

#Region "ResponseHasExceptions"
    Public Overrides Function ResponseHasExceptions(oRequest As Request) As Boolean
        Return False
    End Function
#End Region

#Region "Helpers"

    Public Function GetPropertyID(ByVal MasterID As String) As String

        Dim sPropertyID As String = Intuitive.SQL.GetValue("select TPPropertyID from TPProperty where source='Jumbo' and MasterID={0}", MasterID)

        If sPropertyID <> "" Then
            Return sPropertyID
        Else
            Return "0"
        End If

    End Function

    Public Function DeDupeJumboResults(ByVal Result As XmlDocument) As XmlDocument

        Dim oAddedRooms As New Generic.List(Of String)
        Dim sbDeDupedXml As New StringBuilder
        Dim oDeDupedXml As New XmlDocument

        'add the rooms to the collection
        For Each oRoomResult As XmlNode In Result.SelectNodes("/Results/Result")

            Dim bNewRoom As Boolean = True
            Dim sRoomResult As String = oRoomResult.OuterXml

            If Not oAddedRooms.Contains(sRoomResult) Then
                oAddedRooms.Add(sRoomResult)
            End If

        Next

        'build up the new xml
        sbDeDupedXml.Append("<Results>")

        For Each sRoomXmlNode As String In oAddedRooms
            sbDeDupedXml.Append(sRoomXmlNode)
        Next

        sbDeDupedXml.Append("</Results>")

        'load
        oDeDupedXml.LoadXml(sbDeDupedXml.ToString)

        Return oDeDupedXml

    End Function

#End Region

End Class