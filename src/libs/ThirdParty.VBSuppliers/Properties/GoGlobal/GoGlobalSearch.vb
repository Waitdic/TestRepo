Imports System.Xml
Imports iVector.Search.Property
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Search.Models
Imports ThirdParty.Abstractions.Lookups
Imports Intuitive
Imports ThirdParty.VBSuppliers.My.Resources
Imports Intuitive.Net.WebRequests
Imports ThirdParty.Abstractions.Results
Imports Intuitive.Helpers.Extensions

Public Class GoGlobalSearch
    Inherits ThirdPartyPropertySearchBase

#Region "Constructor"

    Public Sub New(settings As IGoGlobalSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

#End Region

#Region "Properties"

    Private ReadOnly _settings As IGoGlobalSettings

    Private ReadOnly _support As ITPSupport

    Public Overrides Property Source As String = ThirdParties.GOGLOBAL

    Public Overrides ReadOnly Property SqlRequest As Boolean = False

    Public Overrides ReadOnly Property SupportsNonRefundableTagging As Boolean = False

#End Region

#Region "SearchRestrictions"

    Public Overrides Function SearchRestrictions(oSearchDetails As SearchDetails) As Boolean

        Dim bRestrictions As Boolean = oSearchDetails.Rooms > 1

        For Each oRoom As RoomDetail In oSearchDetails.RoomDetails
            If oRoom.ChildAges.Where(Function(i) i <= 10).Count > 2 Then bRestrictions = True
            If oRoom.Infants > 1 Then bRestrictions = True
        Next

        Return bRestrictions

    End Function

#End Region

#Region "SearchFunctions"

    Public Overrides Function BuildSearchRequests(oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit), bSaveLogs As Boolean) As List(Of Request)

        Dim oRequests As New List(Of Request)

        'Create a request for each batch
        For Each oResortSplit As ResortSplit In oResortSplits

            'Build the request
            Dim xRequest As XDocument = BuildRequest(oSearchDetails, oResortSplit.ResortCode)

            'Make the web request
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.URL(oSearchDetails)
                .SetRequest(RequestWrapper("1", xRequest))
                .Method = eRequestMethod.POST
                .Source = Source
                .LogFileName = "Search"
                .TimeoutInSeconds = Me.RequestTimeOutSeconds(oSearchDetails)
                .CreateLog = bSaveLogs
                .ExtraInfo = oSearchDetails
                .UseGZip = True
            End With

            'Add the web request to the collection
            oRequests.Add(oWebRequest)
        Next

        Return oRequests

    End Function

    Public Overrides Function TransformResponse(oRequests As List(Of Request), oSearchDetails As SearchDetails, oResortSplits As List(Of ResortSplit)) As TransformedResultCollection

        Dim oTransformedXML As New XmlDocument
        Dim oXMLs As New List(Of XmlDocument)
        Dim soap As XNamespace = XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/")
        Dim ns As XNamespace = XNamespace.Get("http://www.goglobal.travel/")

        SyncLock (oRequests)
            For Each oRequest As Request In oRequests

                Dim XResponse As XDocument = XDocument.Parse(oRequest.ResponseString)

                Dim xInfo As XDocument = XDocument.Parse(XResponse.Document.Element(soap + "Envelope").Element(soap + "Body").Element(ns + "MakeRequestResponse").Element(ns + "MakeRequestResult").Value)

                Dim xmlHotel As New XmlDocument
                xmlHotel.LoadXml(xInfo.Document.Element("Root").Element("Main").ToString)

                oXMLs.Add(xmlHotel)
            Next
        End SyncLock

        'Merge the responses
        Dim oMergedResponses As XmlDocument = Intuitive.XMLFunctions.MergeXMLDocuments("Results", oXMLs)

        oTransformedXML = XMLFunctions.XMLStringTransform(oMergedResponses, GoGlobalRes.GoGlobalSearchXSL)

        Return oTransformedXML


    End Function

#End Region

#Region "Helper Functions"

    Private Function BuildRequest(ByVal oSearchDetails As SearchDetails, ByVal CityID As String) As XDocument

        Dim xRequest As XDocument = New XDocument(New XElement("Root",
                                                               New XElement("Header",
                                                                            New XElement("Agency", _settings.Agency(oSearchDetails)),
                                                                            New XElement("User", _settings.User(oSearchDetails)),
                                                                            New XElement("Password", _settings.Password(oSearchDetails)),
                                                                            New XElement("Operation", "HOTEL_SEARCH_REQUEST"),
                                                                            New XElement("OperationType", "Request")
                                                                            ),
                                                                New XElement("Main",
                                                                             New XElement("MaximumWaitTime", Me.RequestTimeOutSeconds(oSearchDetails)),
                                                                             New XElement("CityCode", CityID),
                                                                             New XElement("ArrivalDate", oSearchDetails.ArrivalDate.ToString("yyyy-MM-dd")),
                                                                             New XElement("Nights", oSearchDetails.Duration),
                                                                             New XElement("Rooms",
                                                                                          From oRoom In oSearchDetails.RoomDetails
                                                                                          Select New XElement("Room",
                                                                                                New XAttribute("Adults", oRoom.Adults + oRoom.ChildAges.Where(Function(i) i > 10).Count),
                                                                                                New XAttribute("RoomCount", oSearchDetails.Rooms),
                                                                                                New XAttribute("CotCount", oRoom.Infants),
                                                                                                From iChildAge In oRoom.ChildAges
                                                                                                Where iChildAge <= 10
                                                                                                Select New XElement("ChildAge", iChildAge))))))
        Return xRequest


    End Function

    Private Function RequestWrapper(ByVal RequestType As String, ByVal XML As XDocument) As String

        Dim xsi As XNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance")
        Dim xsd As XNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema")
        Dim soap As XNamespace = XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/")
        Dim ns As XNamespace = XNamespace.Get("http://www.goglobal.travel/")

        Dim xRequest As XDocument = New XDocument(
                                     New XDeclaration("1.0", "utf-8", "no"),
                                     New XElement(soap + "Envelope",
                                                  New XAttribute(XNamespace.Xmlns + "xsi", xsi),
                                                  New XAttribute(XNamespace.Xmlns + "xsd", xsd),
                                                  New XAttribute(XNamespace.Xmlns + "soap", soap),
                                                  New XElement(soap + "Body",
                                                               New XElement(ns + "MakeRequest",
                                                                            New XAttribute("xmlns", ns),
                                                                            New XElement(ns + "requestType", RequestType),
                                                                            New XElement(ns + "xmlRequest", New XCData(XML.ToString))))))

        Return xRequest.ToString

    End Function

    Private Function RoomCodes(ByVal Room As RoomDetail) As Generic.List(Of String)

        Dim iOccupancy As Integer = Room.Adults + Room.ChildAges.Where(Function(i) i > 10).Count

        Dim sCodes As New Generic.List(Of String)

        If iOccupancy = 1 Then sCodes = New Generic.List(Of String) From {"SGL", "DBLSGL"}
        If iOccupancy = 2 Then sCodes = New Generic.List(Of String) From {"DBL", "TWN"}
        If iOccupancy = 3 Then sCodes.Add("TPL")
        If iOccupancy = 4 Then sCodes.Add("QDR")

        Return sCodes

    End Function

#End Region

#Region "ResponseHasExceptions"

    Public Overrides Function ResponseHasExceptions(oRequest As Intuitive.Net.WebRequests.Request) As Boolean
        Return False
    End Function

#End Region

    Public Class GoGlobalSearchHelper
        Public Property SearchDetails As SearchDetails
        Public Property RoomTypes As New Generic.List(Of String)

        Public Sub New(ByVal Details As SearchDetails, ByVal Types As Generic.List(Of String))
            SearchDetails = Details
            RoomTypes = Types
        End Sub

        Public Sub New(ByVal Details As SearchDetails)
            SearchDetails = Details
        End Sub

    End Class

    Private Class Room
        Public Property Adults As Integer
        Public Property RoomCount As Integer
        Public Property ChildAges As New Generic.List(Of Integer)
        Public Property CotCount As Integer

        Public Sub New()

        End Sub

        Public Sub New(Adults As Integer, ChildAges As String, CotCount As Integer, RoomCount As Integer)
            Me.Adults = Adults
            Me.RoomCount = RoomCount
            Me.CotCount = CotCount

            For Each sAge As String In ChildAges.Split(","c)
                If sAge.ToSafeInt() <= 10 Then
                    Me.ChildAges.Add(sAge.ToSafeInt())
                Else
                    Me.Adults += 1
                End If
            Next
        End Sub
    End Class
End Class