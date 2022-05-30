Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Net.WebRequests
Imports System.Web
Imports System.Xml
Imports System.Text
Imports System.IO
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Models.Property.Booking
Imports ThirdParty.Abstractions.Models
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Support
Imports ThirdParty.VBSuppliers.My.Resources
Imports Intuitive.Helpers.Extensions
Imports System.Net.Http

Public Class JonView
    Implements IThirdParty

#Region "Properties"

    Private ReadOnly _settings As IJonViewSettings

    Private ReadOnly _support As ITPSupport

    Private ReadOnly _httpClient As HttpClient

    Public ReadOnly Property SupportsRemarks As Boolean Implements IThirdParty.SupportsRemarks
        Get
            Return False
        End Get
    End Property

    Private Function IThirdParty_SupportsLiveCancellation(searchDetails As IThirdPartyAttributeSearch, source As String) As Boolean Implements IThirdParty.SupportsLiveCancellation
        Return _settings.AllowCancellations(searchDetails)
    End Function

    Public ReadOnly Property SupportsBookingSearch() As Boolean Implements IThirdParty.SupportsBookingSearch
        Get
            Return False
        End Get
    End Property

    Private Function IThirdParty_TakeSavingFromCommissionMargin(searchDetails As IThirdPartyAttributeSearch) As Boolean Implements IThirdParty.TakeSavingFromCommissionMargin
        Return False
    End Function

    Private Function IThirdParty_OffsetCancellationDays(searchDetails As IThirdPartyAttributeSearch) As Integer Implements IThirdParty.OffsetCancellationDays
        Return _settings.OffsetCancellationDays(searchDetails, False)
    End Function

    Public Function RequiresVCard(info As VirtualCardInfo) As Boolean Implements IThirdParty.RequiresVCard
        Return False
    End Function

#End Region

#Region "Constructor"

    Public Sub New(settings As IJonViewSettings, support As ITPSupport, httpClient As HttpClient)
        _settings = Ensure.IsNotNull(settings, NameOf(settings))
        _support = Ensure.IsNotNull(support, NameOf(support))
        _httpClient = Ensure.IsNotNull(httpClient, NameOf(httpClient))
    End Sub

#End Region

#Region "PreBook"

    Public Function PreBook(ByVal PropertyDetails As PropertyDetails) As Boolean Implements IThirdParty.PreBook

        Dim iSalesChannelID As Integer = PropertyDetails.SalesChannelID
        Dim iBrandID As Integer = PropertyDetails.BrandID

        Try
            Me.GetCancellationPolicy(PropertyDetails)

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString)
            Return False

        End Try

        Return True

    End Function

#End Region

#Region "Book"

    Public Function Book(ByVal PropertyDetails As PropertyDetails) As String Implements IThirdParty.Book

        Dim oResponse As New XmlDocument
        Dim oRequest As String = String.Empty
        Dim sBookingReference As String = ""

        Try

            'build request
            oRequest = BuildBookingURL(PropertyDetails)

            'send the request
            Dim oBookingRequest As New Request
            With oBookingRequest
                .EndPoint = _settings.URL(PropertyDetails) & GetRequestHeader(PropertyDetails) & oRequest
                .Source = ThirdParties.JONVIEW
                .ContentType = ContentTypes.Text_xml
                .Send(_httpClient)
            End With

            oResponse = oBookingRequest.ResponseXML


            'get booking reference
            Dim oBookingStatus As XmlNode = oResponse.SelectSingleNode("/message/actionseg/status")
            If Not oBookingStatus Is Nothing AndAlso oBookingStatus.InnerText = "C" Then
                sBookingReference = oResponse.SelectSingleNode("/message/actionseg/resnumber").InnerText
            End If


            'return reference or failed
            If sBookingReference = "" OrElse sBookingReference.ToLower = "error" Then
                sBookingReference = "failed"
            End If

        Catch ex As Exception
            PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString)
            sBookingReference = "failed"

        Finally

            'store the request and response xml on the property booking
            If oRequest <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Book Request", oRequest)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Book Response", oResponse)
            End If

        End Try

        Return sBookingReference

    End Function

#End Region

#Region "Get Cancellation Policy"

    Public Sub GetCancellationPolicy(ByVal PropertyDetails As PropertyDetails)

        'create an array variable to hold the policy for each room
        Dim aPolicies(PropertyDetails.Rooms.Count - 1) As Cancellations

        'we'll need this regular expression too (declared here for efficiency)
        Dim oDailyRegEx As New RegularExpressions.Regex("on (?<type>\S+) (?<numberofdays>[0-9]+) day(\(s\))?")

        'loop through the rooms
        Dim iLoop As Integer = 0
        For Each oRoomDetails As RoomDetails In PropertyDetails.Rooms

            'build request
            Dim cancellationURL As String = BuildCancellationURL(PropertyDetails, oRoomDetails)

            'get response
            Dim oResponse As New XmlDocument

            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.URL(PropertyDetails) & GetRequestHeader(PropertyDetails) & cancellationURL
                .ContentType = ContentTypes.Text_xml
                .Source = ThirdParties.JONVIEW
                .LogFileName = "CancellationPolicy"
                .CreateLog = True
                .Send(_httpClient)
            End With

            oResponse = oWebRequest.ResponseXML


            'make sure we initialize the final policy for this room
            aPolicies(iLoop) = New Cancellations


            'check the status
            If XMLFunctions.SafeNodeValue(oResponse, "/message/actionseg/status") = "C" Then

                'we need to get the end date and amount for each cancellation item, and add them to the final cancellation policy for this room
                For Each oCancellationNode As XmlNode In oResponse.SelectNodes("/message/productlistseg/listrecord/cancellation/canitem")

                    Dim iFromDaysBeforeArrival As Integer = oCancellationNode.SelectSingleNode("fromdays").InnerText.ToSafeInt()
                    Dim iToDaysBeforeArrival As Integer = oCancellationNode.SelectSingleNode("todays").InnerText.ToSafeInt()
                    Dim sChargeType As String = oCancellationNode.SelectSingleNode("chargetype").InnerText
                    Dim sCancellationRateType As String = oCancellationNode.SelectSingleNode("ratetype").InnerText
                    Dim nCancellationRate As Double = oCancellationNode.SelectSingleNode("canrate").InnerText.ToSafeDecimal()

                    Dim oNoteNode As XmlNode = oCancellationNode.SelectSingleNode("cannote")
                    Dim sNote As String = If(oNoteNode IsNot Nothing, oNoteNode.InnerText, "")


                    'get start date
                    Dim dStartDate As Date = PropertyDetails.ArrivalDate.AddDays(-iFromDaysBeforeArrival)
                    If iToDaysBeforeArrival < 0 Then
                        dStartDate = PropertyDetails.ArrivalDate
                    Else
                        dStartDate = PropertyDetails.ArrivalDate.AddDays(-iFromDaysBeforeArrival)
                    End If

                    'get end date
                    Dim dEndDate As Date
                    If iToDaysBeforeArrival < 0 Then
                        dEndDate = PropertyDetails.ArrivalDate
                    Else
                        dEndDate = PropertyDetails.ArrivalDate.AddDays(-iToDaysBeforeArrival)
                    End If


                    'calculate the base amounts (the amounts we're going to use to get the final amount from)
                    Dim aBaseAmounts As New Generic.List(Of Decimal)

                    Select Case sChargeType

                        Case "EI", "ENTIRE ITEM"
                            aBaseAmounts.Add(oRoomDetails.LocalCost)

                        Case "P", "PER PERSON" 'unfortunately we have to guess these as we are using the wrong search request (CT instead of CU)

                            For i As Integer = 1 To oRoomDetails.Adults + oRoomDetails.Children
                                aBaseAmounts.Add(oRoomDetails.LocalCost / oRoomDetails.Adults + oRoomDetails.Children)
                            Next

                        Case "DAILY"

                            Dim oDailyRegMatch As System.Text.RegularExpressions.Match = oDailyRegEx.Match(sNote)
                            Dim sType As String = oDailyRegMatch.Groups("type").Value.ToLower
                            Dim iNumberOfDays As Integer = oDailyRegMatch.Groups("numberofdays").Value.ToSafeInt()

                            'make sure we don't get zero rates (we have to be careful of this because if there is a 'stay X pay Y' special offer,
                            'often the first night will be zero - and the cancellation fee should be based on the second night instead)
                            Dim aRates() As String = Array.FindAll(oRoomDetails.ThirdPartyReference.Split("_"c)(1).Split("/"c),
                             Function(sRate As String) sRate.ToSafeDecimal() <> 0)

                            Dim aRatesWeWant(iNumberOfDays - 1) As String
                            Dim iSourceIndex As Integer

                            'I'm not sure the type is ever anything other than 'first' but I thought I'd check here just in case
                            If sType = "first" Then
                                iSourceIndex = 0
                            ElseIf sType = "last" Then
                                iSourceIndex = aRates.Length - iNumberOfDays
                            End If

                            If aRates.Length > iSourceIndex Then Array.ConstrainedCopy(aRates, iSourceIndex, aRatesWeWant, 0, iNumberOfDays)

                            For Each sRate As String In aRatesWeWant
                                aBaseAmounts.Add(sRate.ToSafeDecimal())
                            Next

                    End Select


                    'now, for each base amount, we're going to either add a value or a percentage to the final amount
                    Dim nFinalAmountForThisRule As Double = 0

                    For Each nAmount As Decimal In aBaseAmounts

                        Select Case sCancellationRateType

                            Case "D" 'fixed amount in dollars
                                nFinalAmountForThisRule += nAmount

                            Case "P" 'percentage of base amount
                                nFinalAmountForThisRule += nAmount * (nCancellationRate / 100.0)

                        End Select

                    Next

                    'we've got everything we need (finally) - now lets add it to the policy
                    aPolicies(iLoop).AddNew(dStartDate, dEndDate, nFinalAmountForThisRule.ToSafeDecimal())

                Next


                'solidify the policy (turns our random collection of rules into a proper (continuous) policy ready for merging)
                aPolicies(iLoop).Solidify(SolidifyType.Max, New Date(2099, 12, 31), oRoomDetails.LocalCost)

            End If


            'increment the loop counter 
            iLoop += 1

            'Add the Logs to the booking
            If cancellationURL <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Cancellation Costs Request", cancellationURL)
            End If

            If cancellationURL IsNot Nothing Then
                PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Cancellation Costs Response", cancellationURL)
            End If

        Next

        'merge the policies and add it to the booking
        PropertyDetails.Cancellations = Cancellations.MergeMultipleCancellationPolicies(aPolicies)

    End Sub

#End Region

#Region "Cancellation"

    Public Function CancelBooking(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationResponse Implements IThirdParty.CancelBooking

        Dim oThirdPartyCancellationResponse As New ThirdPartyCancellationResponse
        Dim sRequest As String = ""
        Dim oResponse As New XmlDocument

        Try

            'build request
            sRequest = BuildReservationCancellationURL(PropertyDetails.SourceReference)

            'Send the request
            Dim oWebRequest As New Request
            With oWebRequest
                .EndPoint = _settings.URL(PropertyDetails) & GetRequestHeader(PropertyDetails) & sRequest
                .ContentType = ContentTypes.Text_xml
                .Source = ThirdParties.JONVIEW
                .LogFileName = "Cancel"
                .CreateLog = True
                .Send(_httpClient)
            End With


            oResponse = oWebRequest.ResponseXML


            'get reference
            If oResponse.SelectSingleNode("message/actionseg/status").InnerText = "D" Then
                oThirdPartyCancellationResponse.TPCancellationReference = DateTime.Now.ToString("yyyyMMddhhmm")
                oThirdPartyCancellationResponse.Success = True
            End If

        Catch ex As Exception

            PropertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString)
            oThirdPartyCancellationResponse.Success = False

        Finally

            'store the request and response xml on the property booking
            If sRequest <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "Cancellation Request", sRequest)
            End If

            If oResponse.InnerXml <> "" Then
                PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "Cancellation Response", oResponse)
            End If

        End Try

        Return oThirdPartyCancellationResponse

    End Function

    Public Function GetCancellationCost(ByVal PropertyDetails As PropertyDetails) As ThirdPartyCancellationFeeResult Implements IThirdParty.GetCancellationCost
        Return New ThirdPartyCancellationFeeResult
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

    Public Function BookingStatusUpdate(propertyDetails As PropertyDetails) As ThirdPartyBookingStatusUpdateResult Implements IThirdParty.BookingStatusUpdate
        Return New ThirdPartyBookingStatusUpdateResult
    End Function

#End Region

#Region "Support"

    Private Function BuildBookingURL(propertyDetails As PropertyDetails) As String
        Dim message As New StringBuilder

        message.Append("<message>")
        message.Append("<actionseg>AR</actionseg>")
        message.Append("<commitlevelseg>1</commitlevelseg>")
        message.Append("<resinfoseg>")
        message.AppendFormat("<refitem>{0}</refitem>", DateTime.Now.ToString("yyyyMMddhhmm"))
        message.Append("<attitem>host</attitem>")
        message.Append("<resitem></resitem>")
        message.Append("</resinfoseg>")
        message.Append("<paxseg>")

        'for each guest in each room
        Dim iGuestIndex As Integer = 0
        For Each oRoomDetails As RoomDetails In propertyDetails.Rooms
            For Each oPassenger As Passenger In oRoomDetails.Passengers

                iGuestIndex += 1

                message.Append("<paxrecord>")
                message.AppendFormat("<paxnum>{0}</paxnum>", iGuestIndex)
                message.Append("<paxseq></paxseq>")
                message.AppendFormat("<titlecode>{0}</titlecode>", Me.GetTitle(oPassenger.Title.ToUpper))
                message.AppendFormat("<fname>{0}</fname>", oPassenger.FirstName)
                message.AppendFormat("<lname>{0}</lname>", oPassenger.LastName)

                Dim sAge As String = ""
                If oPassenger.PassengerType = PassengerType.Child Then
                    sAge = oPassenger.Age.ToString
                ElseIf oPassenger.PassengerType = PassengerType.Infant Then
                    sAge = "1"
                End If

                message.AppendFormat("<age>{0}</age>", sAge)
                message.Append("<language>EN</language>")
                message.Append("</paxrecord>")

            Next
        Next

        message.Append("</paxseg>")
        message.Append("<bookseg>")


        'get hotel request if it exists
        Dim sComment As String = ""
        For Each oComment As BookingComment In propertyDetails.BookingComments
            sComment += oComment.Text & " "
        Next


        'for each room
        Dim iRoomIndex As Integer = 0
        Dim incRoomGuestCount As Integer = 0
        For Each oRoomDetails As RoomDetails In propertyDetails.Rooms

            iRoomIndex += 1

            message.Append("<bookrecord>")
            message.AppendFormat("<booknum>{0}</booknum>", iRoomIndex)
            message.Append("<bookseq></bookseq>")
            message.AppendFormat("<prodcode>{0}</prodcode>", oRoomDetails.ThirdPartyReference.Split("_"c)(0))
            message.AppendFormat("<startdate>{0}</startdate>", propertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"))
            message.AppendFormat("<duration>{0}</duration>", propertyDetails.Duration)
            message.AppendFormat("<note>{0}</note>", sComment)

            Dim sbPaxArray As New StringBuilder

            Dim iRoomGuestCount As Integer = oRoomDetails.Passengers.Count
            For i As Integer = 1 To propertyDetails.Adults + propertyDetails.Children + propertyDetails.Infants

                If i >= incRoomGuestCount + 1 AndAlso i < incRoomGuestCount + 1 + iRoomGuestCount Then
                    sbPaxArray.Append("Y")
                Else
                    sbPaxArray.Append("N")
                End If

            Next

            incRoomGuestCount += iRoomGuestCount

            message.AppendFormat("<paxarray>{0}</paxarray>", sbPaxArray.ToString)
            message.Append("</bookrecord>")

        Next

        message.Append("</bookseg>")
        message.Append("</message>")

        Return message.ToString

    End Function

    Private Function BuildReservationCancellationURL(bookingReference As String) As String
        Dim message As New StringBuilder

        message.Append("<message>")
        message.Append("<actionseg>CR</actionseg>")
        message.Append("<resinfoseg>")
        message.AppendFormat("<resitem>{0}</resitem>", bookingReference)
        message.Append("</resinfoseg>")
        message.Append("</message>")

        Return message.ToString
    End Function


    Public Function GetTitle(ByVal sTitle As String) As String

        Dim oTitle As Generic.List(Of String) = CType(HttpRuntime.Cache("AlliedTitle"), Generic.List(Of String))

        If oTitle Is Nothing Then

            oTitle = New Generic.List(Of String)

            'read from res file
            Dim sTitles As New StringReader(JonViewRes.Title)
            Dim sTitlesLine As String = sTitles.ReadLine


            'loop through Titles, 
            Do While Not sTitlesLine Is Nothing

                If Not oTitle.Contains(sTitlesLine) Then
                    oTitle.Add(sTitlesLine)
                End If
                sTitlesLine = sTitles.ReadLine
            Loop

            Intuitive.Functions.AddToCache("JonViewTitle", oTitle, 9999)

        End If

        If oTitle.Contains(sTitle) Then
            Return sTitle
        Else
            Return "MR"
        End If

    End Function

    Private Function BuildCancellationURL(propertyDetails As PropertyDetails, roomDetails As RoomDetails) As String
        Dim message As New StringBuilder

        message.Append("<message>")
        message.Append("<actionseg>DP</actionseg>")
        message.Append("<searchseg>")
        message.Append("<changesince></changesince>")
        message.AppendFormat("<fromdate>{0}</fromdate>", propertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"))
        message.AppendFormat("<todate>{0}</todate>", propertyDetails.DepartureDate.ToString("dd-MMM-yyyy"))
        message.Append("<prodtypecode>ALL</prodtypecode>")
        message.Append("<searchtype>PROD</searchtype>")
        message.Append("<productlistseg>")
        message.Append("<codeitem>")
        message.AppendFormat("<productcode>{0}</productcode>", roomDetails.ThirdPartyReference.Split("_"c)(0))
        message.Append("</codeitem>")
        message.Append("</productlistseg>")
        message.Append("<displayrestriction>N</displayrestriction>")
        message.Append("<displaypolicy>Y</displaypolicy>")
        message.Append("<displayschdate>N</displayschdate>")
        message.Append("</searchseg>")
        message.Append("</message>")
        message.Append("<displaynamedetails>Y</displaynamedetails>")
        message.Append("<displayusage>Y</displayusage>")
        message.Append("<displaygeocode>Y</displaygeocode>")
        message.Append("<displaydynamicrates>Y</displaydynamicrates>")

        Return message.ToString
    End Function

    Private Function GetRequestHeader(searchDetails As IThirdPartyAttributeSearch) As String
        Dim header As New StringBuilder
        header.AppendFormat("?actioncode=HOSTXML&clientlocseq={0}&userid={1}&" &
                      "password={2}&message=<?xml version=""1.0"" encoding=""UTF-8""?>",
                      _settings.ClientLoc(searchDetails), _settings.UserID(searchDetails), _settings.Password(searchDetails))

        Return header.ToSafeString
    End Function

#End Region

#Region "End session"
    Public Sub EndSession(oPropertyDetails As PropertyDetails) Implements IThirdParty.EndSession

    End Sub

#End Region

End Class
