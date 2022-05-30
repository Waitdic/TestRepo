Imports System.Xml
Imports System.Text
Imports iVector.Search.Property
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Search.Models
Imports ThirdParty.Abstractions.Search.Support
Imports ThirdParty.VBSuppliers.My.Resources
Imports Intuitive
Imports Intuitive.Net.WebRequests
Imports ThirdParty.Abstractions.Results

Public Class BonotelSearch
    Inherits ThirdPartyPropertySearchBase

#Region "Properties"

    Private ReadOnly _settings As IBonotelSettings

    Private ReadOnly _support As ITPSupport

    Public Overrides Property Source As String = ThirdParties.BONOTEL

    Public Overrides ReadOnly Property SqlRequest As Boolean = False

    Public Overrides ReadOnly Property SupportsNonRefundableTagging As Boolean = False

#End Region

#Region "Constructors"

    Public Sub New(settings As IBonotelSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

#End Region

#Region "SearchRestrictions"

    Public Overrides Function SearchRestrictions(oSearchDetails As SearchDetails) As Boolean

        Dim bRestrictions As Boolean = False

        If oSearchDetails.Duration > 30 Then
            bRestrictions = True
        End If

        Return bRestrictions

    End Function

#End Region

#Region "SearchFunctions"

    Public Overrides Function BuildSearchRequests(oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit), bSaveLogs As Boolean) As List(Of Request)

        Dim oRequests As New Generic.List(Of Intuitive.Net.WebRequests.Request)

        For Each oResortSplit As ResortSplit In oResortSplits


            'Dim oFinalResults As New Results
            Dim sb As New StringBuilder
            sb.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
            sb.Append("<availabilityRequest>")
            sb.Append("<control>")
            sb.AppendFormat("<userName>{0}</userName>", _settings.Username(oSearchDetails))
            sb.AppendFormat("<passWord>{0}</passWord>", _settings.Password(oSearchDetails))
            sb.Append("</control>")
            sb.AppendFormat("<checkIn>{0}</checkIn>", oSearchDetails.PropertyArrivalDate.ToString("dd-MMM-yyyy"))
            sb.AppendFormat("<checkOut>{0}</checkOut>", oSearchDetails.PropertyDepartureDate.ToString("dd-MMM-yyyy"))
            sb.AppendFormat("<noOfRooms>{0}</noOfRooms>", oSearchDetails.Rooms)
            sb.AppendFormat("<noOfNights>{0}</noOfNights>", oSearchDetails.PropertyDuration)
            sb.AppendFormat("<city>{0}</city>", oResortSplit.ResortCode)
            sb.AppendFormat("<hotelCodes>")
            If oResortSplit.Hotels.Count = 1 Then
                sb.AppendFormat("<hotelCode>{0}</hotelCode>", oResortSplit.Hotels(0).TPKey)
            Else
                sb.AppendFormat("<hotelCode>0</hotelCode>")
            End If
            sb.AppendFormat("</hotelCodes>")
            sb.AppendFormat("<roomsInformation>")
            For Each oRoom As RoomDetail In oSearchDetails.RoomDetails
                sb.AppendFormat("<roomInfo>")
                sb.AppendFormat("<roomTypeId>0</roomTypeId>")
                sb.AppendFormat("<bedTypeId>0</bedTypeId> ")
                sb.AppendFormat("<adultsNum>{0}</adultsNum>", oRoom.Adults)
                sb.AppendFormat("<childNum>{0}</childNum>", oRoom.Children + oRoom.Infants)
                If oRoom.Children + oRoom.Infants > 0 Then
                    sb.AppendFormat("<childAges>")
                    For Each iChildAge As Integer In oRoom.ChildAndInfantAges
                        sb.AppendFormat("<childAge>{0}</childAge>", iChildAge)
                    Next
                    sb.AppendFormat("</childAges>")
                End If
                sb.AppendFormat("</roomInfo>")
            Next
            sb.AppendFormat("</roomsInformation>")
            sb.Append("</availabilityRequest>")



            Dim oRequest As New Request
            With oRequest
                .EndPoint = (_settings.URL(oSearchDetails) & "GetAvailability.do")
                .Method = eRequestMethod.POST
                .Source = ThirdParties.BONOTEL
                .LogFileName = "Search"
                .CreateLog = bSaveLogs
                .ExtraInfo = oSearchDetails
                .SetRequest(sb.ToString)
                .ContentType = ContentTypes.Text_xml
            End With

            oRequests.Add(oRequest)

        Next

        Return oRequests

    End Function

    Public Overrides Function TransformResponse(oRequests As List(Of Request), oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit)) As TransformedResultCollection

        Dim oXMLs As New List(Of XmlDocument)
        Dim oTransformedXML As New XmlDocument

        For Each oRequest As Request In oRequests
            oRequest.ResponseXML.InnerXml = oRequest.ResponseXML.InnerXml.Replace("<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>", "")
            oXMLs.Add(oRequest.ResponseXML)
        Next

        'merge XML documents
        Dim oMergedResponses As New XmlDocument
        oMergedResponses = Intuitive.XMLFunctions.MergeXMLDocuments("Results", oXMLs)

        'transform response
        Dim oResult As New XmlDocument
        oResult = Intuitive.XMLFunctions.XMLStringTransform(oMergedResponses, BonotelRes.BonotelRes.BonotelSearch)

        'transform results again
        oTransformedXML = Intuitive.XMLFunctions.XMLStringTransform(oResult, TPRes.result)

        Return oTransformedXML

    End Function

#End Region

#Region "ResponseHasExceptions"
    Public Overrides Function ResponseHasExceptions(oRequest As Request) As Boolean
        Return False
    End Function
#End Region

#Region "Helpers"



#End Region

End Class