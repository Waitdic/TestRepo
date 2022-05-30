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
Imports Intuitive.Helpers.Extensions

Public Class SunHotelsSearch
    Inherits ThirdPartyPropertySearchBase

#Region "Properties"

    Public Overrides Property Source As String = ThirdParties.SUNHOTELS
    Private ReadOnly _support As ITPSupport
    Private ReadOnly _settings As ISunHotelsSettings
    Public Overrides ReadOnly Property SqlRequest As Boolean = False

#End Region

#Region "Constructor"

    Public Sub New(settings As ISunHotelsSettings, support As ITPSupport)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
    End Sub

#End Region

#Region "SearchRestrictions"

    Public Overrides Function SearchRestrictions(oSearchDetails As SearchDetails) As Boolean

        Dim bRestrictions As Boolean = False

        If oSearchDetails.Rooms > 1 Then
            bRestrictions = True
        End If

        Return bRestrictions

    End Function

#End Region

#Region "SearchFunctions"

    Public Overrides Function BuildSearchRequests(
        oSearchDetails As SearchDetails,
        oResortSplits As List(Of ResortSplit),
        bSaveLogs As Boolean) As List(Of Request)

        Dim salesChannelId As Integer = oSearchDetails.SalesChannelID
        Dim oRequests As New List(Of Request)
        Dim oSearchCodes As New Dictionary(Of List(Of String), String)
        Dim oHotelCodes As New List(Of String)

        For Each oResortSplit As ResortSplit In oResortSplits
            If oResortSplit.Hotels.Any Then
                oHotelCodes.AddRange(oResortSplit.Hotels.Select(Function(x) x.TPKey))
            End If
        Next

        oSearchCodes.Add(oHotelCodes, "HOTEL")

        'loop through each searchcode
        For Each oSearchCode As KeyValuePair(Of List(Of String), String) In oSearchCodes

            'set a unique code. if there is one request we only need the source name
            Dim sUniqueCode As String = Me.Source
            If oSearchCodes.Count > 1 Then sUniqueCode = String.Format("{0}_{1}_{2}", Me.Source, oSearchCode.Key, oSearchCode.Value)

            Dim iPropertyRoomBookingID As Integer = 1

            Dim iHotelRequestLimit As Integer = _settings.HotelRequestLimit(oSearchDetails)

            For Each oRoomDetail As RoomDetail In oSearchDetails.RoomDetails

                Dim oExtraInfo As SearchExtraHelper = New SearchExtraHelper(oSearchDetails, sUniqueCode)

                'record the room id for the transform
                oExtraInfo.ExtraInfo = iPropertyRoomBookingID.ToString

                Dim iTotal As Integer = oSearchCode.Key.Count()
                Dim iFrom As Integer
                Dim numberToTake As Integer = iTotal

                If iHotelRequestLimit > 0 Then
                    numberToTake = Math.Min(iHotelRequestLimit, iTotal)
                End If

                While iFrom < iTotal

                    Dim request As String = BuildSearchRequestString(_settings.SearchURL(oSearchDetails),
                                                                 _settings.Username(oSearchDetails),
                                                                 _settings.Password(oSearchDetails),
                                                                 _settings.Language(oSearchDetails),
                                                                 _settings.Currency(oSearchDetails),
                                                                 oSearchDetails, oRoomDetail,
                                                                 oSearchCode.Key.Skip(iFrom).Take(numberToTake).ToList(), oSearchCode.Value,
                                                                 _settings.Nationality(oSearchDetails),
                                                                 _settings.RequestPackageRates(oSearchDetails))

                    Dim oRequest As New Request
                    With oRequest
                        .EndPoint = request.ToString
                        .Method = eRequestMethod.GET
                        .Source = "SunHotels"
                        .LogFileName = "Search"
                        .CreateLog = bSaveLogs
                        .TimeoutInSeconds = Me.RequestTimeOutSeconds(oSearchDetails) - 2
                        .ExtraInfo = oExtraInfo
                    End With

                    oRequests.Add(oRequest)

                    iPropertyRoomBookingID += 1

                    iFrom += numberToTake
                End While
            Next

        Next

        Return oRequests

    End Function

    Public Function BuildSearchRequestString(
        ByVal url As String,
        ByVal username As String,
        ByVal password As String,
        ByVal language As String,
        ByVal currency As String,
        ByVal searchDetails As SearchDetails,
        ByVal roomDetail As RoomDetail,
        ByVal searchCode As List(Of String),
        ByVal searchCodeType As String,
        ByVal nationality As String,
        ByVal requestPackageRates As Boolean) As String

        'build the request url
        Dim searchRequestUrlBuilder As New StringBuilder

        Dim searchCodeCsv As String = String.Join(",", searchCode)

        With searchRequestUrlBuilder
            .Append(url)
            .AppendFormat("userName={0}", username)
            .AppendFormat("&password={0}", password)
            .AppendFormat("&language={0}", language)
            .AppendFormat("&currencies={0}", currency)
            .AppendFormat("&checkInDate={0}", SunHotels.GetSunHotelsDate(searchDetails.PropertyArrivalDate))
            .AppendFormat("&checkOutDate={0}", SunHotels.GetSunHotelsDate(searchDetails.PropertyDepartureDate))
            .AppendFormat("&numberOfRooms={0}", "1")
            .Append("&destination=")

            'check whether we have a hotel, resort or region code
            Select Case searchCodeType
                Case "HOTEL"
                    .Append("&destinationID=")
                    .AppendFormat("&hotelIDs={0}", searchCodeCsv)
                    .Append("&resortIDs=")

                Case "RESORT"
                    .Append("&destinationID=")
                    .Append("&hotelIDs=")
                    .AppendFormat("&resortIDs={0}", searchCodeCsv)

                Case "REGION"
                    .AppendFormat("&destinationID={0}", searchCodeCsv)
                    .Append("&hotelIDs=")
                    .Append("&resortIDs=")

            End Select

            .AppendFormat("&accommodationTypes=")
            .AppendFormat("&numberOfAdults={0}", GetAdultsFromRoomDetail(roomDetail))
            .AppendFormat("&numberOfChildren={0}", GetChildrenFromRoomDetail(roomDetail))
            .AppendFormat("&childrenAges={0}", roomDetail.ChildAgeCSV)
            .AppendFormat("&infant={0}", IsInfantIncluded(roomDetail))
            .Append("&sortBy=Price&sortOrder=Ascending")
            .Append("&exactDestinationMatch=")
            .Append("&blockSuperdeal=false")
            .Append("&showTransfer=")
            .Append("&mealIds=")
            .Append("&showCoordinates=")
            .Append("&showReviews=")
            .Append("&referencePointLatitude=")
            .Append("&referencePointLongitude=")
            .Append("&maxDistanceFromReferencePoint=")
            .Append("&minStarRating=")
            .Append("&maxStarRating=")
            .Append("&featureIds=")
            .Append("&minPrice=")
            .Append("&maxPrice=")
            .Append("&themeIds=")
            .Append("&excludeSharedRooms=")
            .Append("&excludeSharedFacilities=")
            .Append("&prioritizedHotelIds=")
            .Append("&totalRoomsInBatch=")
            .Append("&paymentMethodId=1")
            .AppendFormat("&customerCountry={0}", nationality)
            .AppendFormat("&B2C={0}", If(requestPackageRates, "0", ""))

        End With

        Return searchRequestUrlBuilder.ToString()
    End Function

    Public Overrides Function TransformResponse(
        oRequests As List(Of Request),
        oSearchDetails As SearchDetails,
        oResortSplits As List(Of ResortSplit)) As TransformedResultCollection

        Dim oXMLs As New List(Of XmlDocument)
        Dim oTransformedXML As New XmlDocument
        Dim sCurrency As String = _settings.Currency(oSearchDetails)

        ' Sunhotels have a separate lookup xml for room type name("http://xml.sunhotels.net/15/PostGet/NonStaticXMLAPI.asmx/GetRoomTypes?")
        Dim oRoomTypes As New Dictionary(Of String, String)
        oRoomTypes = GetRoomTypes()

        'order by property room booking id - stored in extra info
        For Each oRequest As Request In oRequests.OrderBy(Function(o) CType(o.ExtraInfo, SearchExtraHelper).ExtraInfo.ToSafeInt())

            SyncLock (oRequests)

                'strip out anything we don't need
                Dim oSearchResponseXML As XmlDocument = oRequest.ResponseXML
                oSearchResponseXML.InnerXml = oSearchResponseXML.InnerXml.Replace("<?xml version=""1.0"" encoding=""utf-8""?>", "")
                oSearchResponseXML.InnerXml = oSearchResponseXML.InnerXml.Replace(" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""", "")
                oSearchResponseXML.InnerXml = oSearchResponseXML.InnerXml.Replace(" xmlns=""http://xml.sunhotels.net/15/""", "")

                'add property room booking id 
                Dim oPropertyRoomBookingElem As XmlElement = oSearchResponseXML.CreateElement("PropertyRoomBookingID")
                oPropertyRoomBookingElem.InnerText = CType(oRequest.ExtraInfo, SearchExtraHelper).ExtraInfo
                oSearchResponseXML.DocumentElement.AppendChild(oPropertyRoomBookingElem)

                oXMLs.Add(Intuitive.XMLFunctions.CleanXMLNamespaces(oSearchResponseXML))

            End SyncLock

        Next

        'merge the responses
        Dim oMergedResponses As XmlDocument = Intuitive.XMLFunctions.MergeXMLDocuments("Results", oXMLs)



        'transform the response
        Dim oTransformedList As New TransformedResultCollection

        For Each oHotelNode As XmlNode In oMergedResponses.SelectNodes("/Results/searchresult/hotels/hotel")

            Dim iPropertyRoomBookingID As Integer = XMLFunctions.SafeNodeValue(oHotelNode, "../../PropertyRoomBookingID").ToSafeInt
            Dim iMasterID As Integer = XMLFunctions.SafeNodeValue(oHotelNode, "hotel.id").ToSafeInt
            Dim oRoomDetail As RoomDetail = oSearchDetails.RoomDetails.Single(Function(o) o.PropertyRoomBookingID = iPropertyRoomBookingID)

            For Each oRoomTypeNode As XmlNode In oHotelNode.SelectNodes("roomtypes/roomtype")

                'we don't update our sunhotels room types mapping xml file. 
                'if the room type isn't in our file then skip
                Dim sRoomTypeID As String = XMLFunctions.SafeNodeValue(oRoomTypeNode, "roomtype.ID").ToSafeString
                Dim sRoomTypeName As String = String.Empty
                oRoomTypes.TryGetValue(sRoomTypeID, sRoomTypeName)

                If String.IsNullOrEmpty(sRoomTypeName) Then Continue For

                For Each oRoomNode As XmlNode In oRoomTypeNode.SelectNodes("rooms/room")
                    Dim sRoomID As String = XMLFunctions.SafeNodeValue(oRoomNode, "id").ToSafeString
                    Dim sPaymentMethod As String = XMLFunctions.SafeNodeValue(oRoomNode, "paymentMethods/paymentMethod/@id").ToSafeString
                    Dim bIsSuperDeal As Boolean = XMLFunctions.SafeNodeValue(oRoomNode, "isSuperDeal").ToSafeBoolean

                    Dim aCancellations As New List(Of Cancellation)
                    Dim bNonRef As Boolean = False
                    Dim iCancellationPolicyCount As Integer = 0
                    For Each oCancellationPolicyNode As XmlNode In oRoomNode.SelectNodes("cancellation_policies/cancellation_policy")
                        iCancellationPolicyCount += 1

                        Dim oDeadlineNode As XmlNode = oCancellationPolicyNode.SelectSingleNode("deadline")

                        Dim bNullDeadline As Boolean = oDeadlineNode.Attributes.ItemOf("nil")?.Value = "true"
                        Dim nPercentage As Decimal = XMLFunctions.SafeNodeValue(oCancellationPolicyNode, "percentage").ToSafeDecimal

                        Dim oHours As New TimeSpan(oDeadlineNode.InnerText.ToSafeInt, 0, 0)
                        Dim dStartDate, dEndDate As Date

                        'for 100% cancellations we don't get an hours before
                        'so force the start date to be from now
                        If oHours.TotalHours = 0 Then
                            dStartDate = Date.Now.Date
                        Else
                            dStartDate = oSearchDetails.PropertyArrivalDate.Subtract(oHours)
                        End If

                        If (Not bNonRef AndAlso nPercentage = 100 AndAlso bNullDeadline) Then
                            bNonRef = True
                        End If

                        Dim sNode As String = String.Format("cancellation_policies/cancellation_policy[{0}]", iCancellationPolicyCount + 1)
                        If oRoomNode.SelectSingleNode(sNode) IsNot Nothing Then

                            'the hours of the end dates need to be rounded to the nearest 24 hours to stop them overlapping with the start of the next cancellation policy and making
                            'the charges add together
                            Dim iEndHours As Integer = oRoomNode.SelectSingleNode(String.Format("{0}/deadline", sNode)).InnerText.ToSafeInt
                            Dim oEndHours As New TimeSpan(SunHotels.RoundHoursUpToTheNearest24Hours(iEndHours), 0, 0)

                            dEndDate = oSearchDetails.PropertyArrivalDate.Subtract(oEndHours)
                            dEndDate = dEndDate.AddDays(-1)

                        Else
                            dEndDate = New Date(2099, 1, 1)
                        End If

                        aCancellations.Add(New Cancellation(dStartDate, dEndDate, nPercentage))
                    Next

                    For Each oMealNode As XmlNode In oRoomNode.SelectNodes("meals/meal")
                        Dim dAmount As Decimal = XMLFunctions.SafeNodeValue(oMealNode, "prices/price").ToSafeDecimal
                        Dim sLabelID As String = XMLFunctions.SafeNodeValue(oMealNode, "labelId").ToSafeString
                        Dim sID As String = XMLFunctions.SafeNodeValue(oMealNode, "id").ToSafeString
                        Dim sMealBasisID As String = ""
                        If Not String.IsNullOrEmpty(sLabelID) Then
                            sMealBasisID = $"{sID}|{sLabelID}"
                        Else
                            sMealBasisID = sID
                        End If

                        Dim aResultCancellations As New List(Of Models.Property.Booking.Cancellation)
                        For Each oCancellation As Cancellation In aCancellations
                            aResultCancellations.Add(New Models.Property.Booking.Cancellation With {
                                    .Amount = oCancellation.Percentage,
                                    .StartDate = oCancellation.StartDate,
                                    .EndDate = oCancellation.EndDate
                                })
                        Next

                        oTransformedList.TransformedResults.Add(New TransformedResult With {
                            .MasterID = iMasterID,
                            .TPKey = iMasterID.ToSafeString,
                            .CurrencyCode = sCurrency,
                            .PropertyRoomBookingID = iPropertyRoomBookingID,
                            .RoomType = sRoomTypeName,
                            .MealBasisCode = sMealBasisID,
                            .Adults = oRoomDetail.Adults,
                            .Children = oRoomDetail.Children,
                            .ChildAgeCSV = oRoomDetail.ChildAgeCSV,
                            .Infants = oRoomDetail.Infants,
                            .Amount = dAmount,
                            .TPReference = $"{sRoomID}_{sID}_{sRoomTypeID}_{sPaymentMethod}",
                            .NonRefundableRates = bNonRef OrElse bIsSuperDeal,
                            .Cancellations = aResultCancellations
                                })
                    Next
                Next
            Next
        Next

        Return oTransformedList

    End Function

#End Region

#Region "ResponseHasExceptions"
    Public Overrides Function ResponseHasExceptions(oRequest As Request) As Boolean
        Return False
    End Function
#End Region

#Region "Helpers"
    Public Function GetRoomTypes() As Dictionary(Of String, String)
        Dim oRoomTypes As Dictionary(Of String, String) = Functions.GetCache(Of Dictionary(Of String, String))("SunHotelsRoomTypes")

        If oRoomTypes Is Nothing Then
            If oRoomTypes Is Nothing Then
                oRoomTypes = New Dictionary(Of String, String)
                oRoomTypes = Intuitive.Serializer.DeSerialize(Of getRoomTypesResult)(SunHotelsRes.SunHotelsRoomType).roomTypes.ToDictionary(Function(o) o.id, Function(o) o.name)
            End If
            Functions.AddToCache("SunHotelsRoomTypes", oRoomTypes, 9999)
        End If

        Return oRoomTypes

    End Function

#End Region

    Public Class getRoomTypesResult
        Public Property roomTypes As New List(Of roomType)
    End Class
    Public Class roomType
        Public Property id As String
        Public Property name As String
    End Class

    Public Shared Function GetAdultsFromRoomDetail(ByVal roomDetail As RoomDetail) As String

        Dim iAdultCount As Integer = roomDetail.Adults

        For Each iAge As Integer In roomDetail.ChildAges
            If iAge > 17 Then iAdultCount += 1
        Next

        Return iAdultCount.ToString

    End Function

    Public Shared Function GetChildrenFromRoomDetail(ByVal roomDetail As RoomDetail) As String

        Dim iChildCount As Integer = 0

        For Each iAge As Integer In roomDetail.ChildAges
            If iAge <= 17 Then iChildCount += 1
        Next

        Return iChildCount.ToString

    End Function

    Public Shared Function IsInfantIncluded(roomDetail As RoomDetail) As Integer
        Dim iInfantIncluded As Integer = 0
        If roomDetail.Infants > 0 Then
            iInfantIncluded = 1
        End If
        Return iInfantIncluded
    End Function

    Public Class Cancellation
        Public Sub New(dStartDate As Date, dEndDate As Date, nPercentage As Decimal)
            StartDate = dStartDate
            EndDate = dEndDate
            Percentage = nPercentage
        End Sub
        Public Property StartDate As Date
        Public Property EndDate As Date
        Public Property Percentage As Decimal
    End Class

End Class