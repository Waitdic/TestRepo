Imports Intuitive
Imports Intuitive.Functions
Imports System.Text
Imports System.Xml
Imports Intuitive.Net.WebRequests
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.Abstractions.Models
Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Support
Imports System.Net.Http

Public Class YouTravel
    Implements IThirdParty

#Region "Constructor"

    Public Sub New(settings As IYouTravelSettings, support As ITPSupport, httpClient As HttpClient)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
        _httpClient = Ensure.IsNotNull(httpClient, NameOf(httpClient))
    End Sub

#End Region

#Region "Properties"

    Private Property _settings As IYouTravelSettings

    Private Property _support As ITPSupport

    Private Property _httpClient As HttpClient

    Public ReadOnly Property SupportsRemarks As Boolean Implements IThirdParty.SupportsRemarks
        Get
            Return True
        End Get
    End Property

    Private Function IThirdParty_SupportsLiveCancellation(searchDetails As IThirdPartyAttributeSearch, source As String) As Boolean Implements IThirdParty.SupportsLiveCancellation
        Return _settings.AllowCancellations(searchDetails, False)
    End Function

    Public ReadOnly Property SupportsBookingSearch() As Boolean Implements IThirdParty.SupportsBookingSearch
        Get
            Return False
        End Get
    End Property

    Private Function TakeSavingFromCommissionMargin(searchDetails As IThirdPartyAttributeSearch) As Boolean Implements IThirdParty.TakeSavingFromCommissionMargin
        Return False
    End Function

    Private Function OffsetCancellationDays(searchDetails As IThirdPartyAttributeSearch) As Integer Implements IThirdParty.OffsetCancellationDays
        Return _settings.OffsetCancellationDays(searchDetails, False)
    End Function

    Private Function IThirdParty_RequiresVCard(info As VirtualCardInfo) As Boolean Implements IThirdParty.RequiresVCard
        Return False
    End Function


#End Region

#Region "PreBook"

    Public Function PreBook(ByVal PropertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook
        Dim oResponse As New XmlDocument
        Dim bSuccess As Boolean = True
        Dim oWebRequest As New Request

        Try
            'Get Errata details
            Dim sb As New StringBuilder
            sb.AppendFormat("{0}{1}", _settings.PrebookURL(PropertyDetails), "?")
            sb.AppendFormat("&LangID={0}", _settings.LangID(PropertyDetails))
            sb.AppendFormat("&HID={0}", PropertyDetails.TPKey)
            sb.AppendFormat("&UserName={0}", _settings.Username(PropertyDetails))
            sb.AppendFormat("&Password={0}", _settings.Password(PropertyDetails))

            With oWebRequest
                .Source = ThirdParties.YOUTRAVEL
                .CreateLog = True
                .LogFileName = "Prebook"
                .EndPoint = sb.ToString
                .Method = eRequestMethod.GET
                .Send(_httpClient)
            End With

            oResponse = XMLFunctions.CleanXMLNamespaces(oWebRequest.ResponseXML)
            Dim oErrataNode As XmlNode = oResponse.SelectSingleNode("/HtSearchRq/Hotel/Erratas")
            Dim sErrata As String = oErrataNode.InnerText
            PropertyDetails.Errata.AddNew("Important Information", sErrata)
            'Get cancellation policies
            For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms
                Dim oCancellationPolicyWebRequest As New Request
                Dim sbCancelPolicy As New StringBuilder
                Dim oCancelPolicyResponseXML As New XmlDocument
                sbCancelPolicy.AppendFormat("{0}{1}", _settings.CancellationPolicyURL(PropertyDetails), "?")
                sbCancelPolicy.AppendFormat("token={0}", oRoomDetails.ThirdPartyReference.Split("|"c)(2))
                With oCancellationPolicyWebRequest
                    .Source = ThirdParties.YOUTRAVEL
                    .CreateLog = True
                    .LogFileName = "Cancellation Cost"
                    .EndPoint = sbCancelPolicy.ToString
                    .Method = eRequestMethod.GET
                    .Send(_httpClient)
                End With

                oCancelPolicyResponseXML = XMLFunctions.CleanXMLNamespaces(oCancellationPolicyWebRequest.ResponseXML)
                For Each oCancellationNode As XmlNode In oCancelPolicyResponseXML.SelectNodes("/HtSearchRq/Policies/Policy")
                    Dim dFromDate As Date = oCancellationNode.SelectSingleNode("FromDate").InnerText.ToSafeDate
                    Dim dToDate As Date = New Date(2099, 1, 1)
                    Dim nCancellationCost As Decimal = SafeTypeExtensions.ToSafeDecimal(oCancellationNode.SelectSingleNode("Fees").InnerText)
                    PropertyDetails.Cancellations.AddNew(dFromDate, dToDate, nCancellationCost)
                Next
                PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel CancellationPolicy Request " & oRoomDetails.PropertyRoomBookingID, oCancellationPolicyWebRequest.RequestLog)
                PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel CancellationPolicy Response " & oRoomDetails.PropertyRoomBookingID, oCancellationPolicyWebRequest.ResponseLog)
            Next
            PropertyDetails.Cancellations.Solidify(SolidifyType.Sum)
        Catch ex As Exception
            bSuccess = False
            PropertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString)
        Finally
            'save the xml for the front end
            PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Prebook Request", oWebRequest.RequestLog)
            PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Prebook Response", oWebRequest.ResponseLog)

        End Try

        PropertyDetails.LocalCost = PropertyDetails.Rooms.Sum(Function(r) r.LocalCost)

        Return bSuccess
    End Function

#End Region

#Region "Book"

    Public Function Book(ByVal PropertyDetails As PropertyDetails) As String Implements IThirdParty.Book

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID
        Dim oResponse As New XmlDocument
        Dim sRequestURL As String = ""
        Dim sReference As String = ""

        Try

            'build url
            Dim sbURL As New StringBuilder
            sbURL.AppendFormat("{0}{1}", _settings.BookingURL(PropertyDetails), "?")
            sbURL.AppendFormat("&LangID={0}", _settings.LangID(PropertyDetails))
            sbURL.AppendFormat("&UserName={0}", _settings.Username(PropertyDetails))
            sbURL.AppendFormat("&Password={0}", _settings.Password(PropertyDetails))
            sbURL.AppendFormat("&session_ID={0}", PropertyDetails.Rooms(0).ThirdPartyReference.Split("|"c)(0))
            sbURL.AppendFormat("&Checkin_Date={0}", YouTravelSupport.FormatDate(PropertyDetails.ArrivalDate))
            sbURL.AppendFormat("&Nights={0}", PropertyDetails.Duration)
            sbURL.AppendFormat("&Rooms={0}", PropertyDetails.Rooms.Count)

            sbURL.AppendFormat("&HID={0}", PropertyDetails.TPKey)

            'adults and children
            Dim iRoomIndex As Integer = 0
            For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

                iRoomIndex += 1

                sbURL.AppendFormat("&ADLTS_{0}={1}", iRoomIndex, oRoomDetails.Adults)

                If oRoomDetails.Children + oRoomDetails.Infants > 0 Then

                    sbURL.AppendFormat("&CHILD_{0}={1}", iRoomIndex, oRoomDetails.Children + oRoomDetails.Infants)

                    For i As Integer = 1 To oRoomDetails.Children + oRoomDetails.Infants

                        If oRoomDetails.ChildAges.Count > i - 1 Then

                            sbURL.AppendFormat("&ChildAgeR{0}C{1}={2}", iRoomIndex, i, oRoomDetails.ChildAges(i - 1))

                        Else

                            sbURL.AppendFormat("&ChildAgeR{0}C{1}={2}", iRoomIndex, i, -1)

                        End If
                    Next

                Else

                    sbURL.AppendFormat("&CHILD_{0}=0", iRoomIndex)

                End If

                If iRoomIndex = 1 Then
                    sbURL.AppendFormat("&RID={0}", oRoomDetails.ThirdPartyReference.Split("|"c)(1))
                Else
                    sbURL.AppendFormat("&RID_{0}={1}", iRoomIndex, oRoomDetails.ThirdPartyReference.Split("|"c)(1))
                End If

                sbURL.AppendFormat("&Room{0}_Rate={1}", iRoomIndex, SafeMoney(oRoomDetails.GrossCost))

            Next

            sbURL.AppendFormat("&Customer_title={0}", PropertyDetails.Rooms(0).Passengers(0).Title)
            sbURL.AppendFormat("&Customer_firstname={0}", PropertyDetails.Rooms(0).Passengers(0).FirstName)
            sbURL.AppendFormat("&Customer_Lastname={0}", PropertyDetails.Rooms(0).Passengers(0).LastName)

            If PropertyDetails.BookingComments.Count > 0 Then
                'max 250 characters
                sbURL.AppendFormat("&Requests={0}", PropertyDetails.BookingComments.Item(0).Text.Substring _
                          (0, Min(PropertyDetails.BookingComments.Item(0).Text.Length, 250)))
            End If

            sRequestURL = sbURL.ToString

            Dim oWebRequest As New Request
            With oWebRequest
                .Source = ThirdParties.YOUTRAVEL
                .CreateLog = True
                .LogFileName = "Book"
                .EndPoint = sRequestURL
                .Method = eRequestMethod.GET
                .Send(_httpClient)
            End With

            oResponse = oWebRequest.ResponseXML

            'return booking reference if it exists
            Dim oNode As XmlNode = oResponse.SelectSingleNode("/HtSearchRq/Booking_ref")
            If oNode IsNot Nothing AndAlso Not oResponse.SelectSingleNode("/HtSearchRq/Booking_ref").InnerText = "" Then
                sReference = oResponse.SelectSingleNode("/HtSearchRq/Booking_ref").InnerText
            Else

                sReference = "failed"

            End If

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString)
            sReference = "failed"

        Finally

            'store the request and response xml on the property booking
            If sRequestURL <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Book Request", sRequestURL)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Book Response", oResponse)
            End If

        End Try

        Return sReference

    End Function

#End Region

#Region "Cancellation"

    Public Function CancelBooking(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking

        Dim oThirdPartyCancellationResponse As New ThirdPartyCancellationResponse
        Dim sbURL As New StringBuilder
        Dim oResponse As New XmlDocument

        Try

            'build url
            sbURL.AppendFormat("{0}{1}", _settings.CancellationURL(PropertyDetails), "?")
            sbURL.AppendFormat("Booking_ref={0}", PropertyDetails.SourceReference)
            sbURL.AppendFormat("&UserName={0}", _settings.Username(PropertyDetails))
            sbURL.AppendFormat("&Password={0}", _settings.Password(PropertyDetails))


            'Send the request
            Dim oWebRequest As New Request
            With oWebRequest
                .Source = ThirdParties.YOUTRAVEL
                .CreateLog = True
                .LogFileName = "Cancel"
                .EndPoint = sbURL.ToString
                .Method = eRequestMethod.GET
                .Send(_httpClient)
            End With

            oResponse = oWebRequest.ResponseXML


            'check response
            If oResponse.InnerXml <> "" Then

                If oResponse.SelectSingleNode("/HtSearchRq/Success").InnerText = "True" Then

                    oThirdPartyCancellationResponse.TPCancellationReference = (oResponse.SelectSingleNode("/HtSearchRq/Booking_ref").InnerText)
                    oThirdPartyCancellationResponse.Success = True

                Else

                    oThirdPartyCancellationResponse.Success = False

                End If

            End If

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString)
            oThirdPartyCancellationResponse.Success = False

        Finally

            'store the request and response xml on the property booking
            If sbURL.ToString <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Cancellation Request", sbURL.ToString)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Cancellation Response", oResponse)
            End If

        End Try

        Return oThirdPartyCancellationResponse

    End Function

    Public Function GetCancellationCost(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost

        Dim oResult As New ThirdPartyCancellationFeeResult
        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID
        Dim sbURL As New StringBuilder
        Dim oResponse As New XmlDocument

        Try

            sbURL.AppendFormat("{0}{1}", _settings.CancellationFeeURL(PropertyDetails), "?")
            sbURL.AppendFormat("Booking_ref={0}", PropertyDetails.SourceReference)
            sbURL.AppendFormat("&UserName={0}", _settings.Username(PropertyDetails))
            sbURL.AppendFormat("&Password={0}", _settings.Password(PropertyDetails))


            'Send the request
            Dim oWebRequest As New Request
            With oWebRequest
                .Source = ThirdParties.YOUTRAVEL
                .CreateLog = True
                .LogFileName = "Cancellation Cost"
                .EndPoint = sbURL.ToString
                .Method = eRequestMethod.GET
                .Send(_httpClient)
            End With

            oResponse = oWebRequest.ResponseXML

            'return a fees result if found
            Dim oNode As XmlNode = oResponse.SelectSingleNode("/HtSearchRq/Success")
            If oNode IsNot Nothing AndAlso oResponse.SelectSingleNode("/HtSearchRq/Success").InnerText = "True" Then

                oResult.Success = True
                oResult.Amount = SafeDecimal(oResponse.SelectSingleNode("/HtSearchRq/Fees").InnerText)
                oResult.CurrencyCode = SafeString(oResponse.SelectSingleNode("/HtSearchRq/Currency").InnerText)

            Else

                oResult.Success = False

            End If

        Catch ex As Exception

            oResult.Success = False
            PropertyDetails.Warnings.AddNew("Cancellation Cost Exception", ex.ToString)

        Finally

            If sbURL.ToString <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Cancellation Cost Request", sbURL.ToString)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Cancellation Cost Response", oResponse)
            End If

        End Try

        Return oResult

    End Function

#End Region

#Region "Booking Search"

    Public Function BookingSearch(ByVal oBookingSearchDetails As BookingSearchDetails) As ThirdPartyBookingSearchResults Implements IThirdParty.BookingSearch
        Return New ThirdPartyBookingSearchResults
    End Function

    Public Function CreateReconciliationReference(ByVal sInputReference As String) As String Implements IThirdParty.CreateReconciliationReference
        Return ""
    End Function

#End Region

#Region "Booking Status Update"

    Public Function BookingStatusUpdate(ByVal oPropertyDetails As PropertyDetails) As ThirdPartyBookingStatusUpdateResult Implements IThirdParty.BookingStatusUpdate
        Return New ThirdPartyBookingStatusUpdateResult
    End Function

#End Region
    Public Sub EndSession(oPropertyDetails As PropertyDetails) Implements IThirdParty.EndSession

    End Sub
End Class
