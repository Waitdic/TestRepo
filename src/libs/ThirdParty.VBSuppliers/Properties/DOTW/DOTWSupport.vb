Imports System.Text
Imports System.Xml
Imports System.IO
Imports System.Security.Cryptography
Imports Intuitive.Net.WebRequests
Imports ThirdParty.VBSuppliers.My.Resources
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Constants
Imports ThirdParty.Abstractions.Lookups
Imports ThirdParty.Abstractions.Support
Imports Intuitive.Helpers.Extensions

Public Class DOTWSupport
    Implements IDOTWSupport

    Public Class Cities
        Inherits Generic.Dictionary(Of Integer, DOTWSupport.City)

    End Class

    Public Class City

        Public CityNumber As Integer
        Public LocationIDs As New Generic.List(Of Integer)

        Public Sub New(ByVal CityNumber As Integer, ByVal LocationID As Integer)
            Me.CityNumber = CityNumber
            Me.LocationIDs.Add(LocationID)
        End Sub

    End Class

    Public Shared Function StripNameSpaces(ByVal sXML As String) As String

        sXML = sXML.Replace("xmlns=""http://xmldev.dotwconnect.com/xsd/""", "")
        sXML = sXML.Replace("xmlns:a=""http://xmldev.dotwconnect.com/xsd/atomicCondition_rooms""", "")
        sXML = sXML.Replace("xmlns:c=""http://xmldev.dotwconnect.com/xsd/complexCondition_rooms""", "")

        Return sXML

    End Function


#Region "Password Hash"

    Public Shared ReadOnly Property MD5Password(ByVal sPassword As String) As String
        Get

            Dim encoder As New UTF8Encoding()
#Disable Warning SCS0006 ' Weak hashing function
            Dim md5Hasher As New MD5CryptoServiceProvider()
#Enable Warning SCS0006 ' Weak hashing function
            Dim hashedDataBytes As Byte() = md5Hasher.ComputeHash(encoder.GetBytes(sPassword))

            Dim sResult As String = ""
            For Each b As Byte In hashedDataBytes
                sResult += b.ToString("x2")
            Next

            Return sResult

        End Get
    End Property

#End Region

    Public Shared Function GetTitleID(ByVal sTitle As String, ByVal SalesChannelID As Integer) As Integer

        ' HACK CS Dim oTitle As Generic.Dictionary(Of String, Integer) = CType(HttpRuntime.Cache("DOTWTitle"), Generic.Dictionary(Of String, Integer))

        Dim oTitle As Generic.Dictionary(Of String, Integer) = Nothing

        If oTitle Is Nothing Then

            oTitle = New Generic.Dictionary(Of String, Integer)

            'read from res file
            Dim oTitles As New StringReader(DOTWRes.Titles)
            Dim sTitlesLine As String = oTitles.ReadLine


            'loop through Titles, 
            Do While Not sTitlesLine Is Nothing

                Dim sKey As String = sTitlesLine.Split("#"c)(0)
                Dim iValue As Integer = sTitlesLine.Split("#"c)(1).ToSafeInt()

                If Not oTitle.ContainsKey(sKey) Then
                    oTitle.Add(sKey, iValue)
                End If
                sTitlesLine = oTitles.ReadLine
            Loop
            oTitles.Close()

            ' HACK CS Intuitive.Functions.AddToCache("DOTWTitle", oTitle, 60)

        End If


        If oTitle.ContainsKey(sTitle) Then
            Return oTitle(sTitle)
        Else
            Return oTitle("Mr")
        End If


    End Function

    Public Shared Function GetCurrencyCache(ByVal SearchDetails As IThirdPartyAttributeSearch, ByVal Settings As IDOTWSettings) As DOTWCurrencyCache

        ' HACK CS Dim oCurrency As DOTWCurrencyCache = CType(HttpRuntime.Cache("DOTWCurrency"), DOTWCurrencyCache)

        Dim oCurrency As DOTWCurrencyCache = Nothing

        If oCurrency Is Nothing Then


            oCurrency = New DOTWCurrencyCache

            Dim oSB As New StringBuilder

            With oSB
                .AppendLine("<customer>")
                .AppendFormat("<username>{0}</username>", Settings.Username(SearchDetails)).AppendLine()
                .AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(Settings.Password(SearchDetails))).AppendLine()
                .AppendFormat("<id>{0}</id>", Settings.CompanyCode(SearchDetails)).AppendLine()
                .AppendLine("<source>1</source>")
                .AppendLine("<request command=""getcurrenciesids"" />")
                .AppendLine("</customer>")
            End With

            'get the xml response for all currencies
            Dim oResponseXML As New XmlDocument

            Dim oWebRequest As New Request

            Dim oHeaders As New RequestHeaders
            If Settings.UseGZip(SearchDetails) Then
                oHeaders.AddNew("Accept-Encoding", "gzip")
            End If

            With oWebRequest
                .EndPoint = Settings.ServerURL(SearchDetails)
                .Method = eRequestMethod.POST
                .Source = ThirdParties.DOTW
                .Headers = oHeaders
                .LogFileName = "GetCurrencyCache"
                .SetRequest(oSB.ToString)
                .ContentType = ContentTypes.Text_xml
                .CreateLog = True
                .Send()
            End With

            oResponseXML = oWebRequest.ResponseXML

            'check according to documentation that there is a success node with the value TRUE in it
            Dim oSuccessNode As XmlNode = oResponseXML.SelectSingleNode("result/successful")
            If oSuccessNode Is Nothing OrElse oSuccessNode.InnerText <> "TRUE" Then
                Throw New Exception("currencies do not return success")
            End If


            For Each oNode As XmlNode In oResponseXML.SelectNodes("result/currency/option")
                Dim sKey As String = oNode.SelectSingleNode("@shortcut").Value
                Dim iValue As Integer = oNode.SelectSingleNode("@value").Value.ToSafeInt()

                If Not oCurrency.ContainsKey(sKey) Then
                    oCurrency.Add(sKey, iValue)
                End If

            Next

            ' HACK CS Intuitive.Functions.AddToCache("DOTWCurrency", oCurrency, 60)

        End If

        Return oCurrency

    End Function

    Public Function GetCachedCurrencyID(ByVal SearchDetails As IThirdPartyAttributeSearch, ByVal Support As ITPSupport,
                                         ByVal CurrencyCode As String, ByVal Settings As IDOTWSettings) As Integer Implements IDOTWSupport.GetCachedCurrencyID

        Return GetCurrencyID(SearchDetails, Support, CurrencyCode, Settings)

    End Function

    Public Shared Function GetCurrencyID(ByVal SearchDetails As IThirdPartyAttributeSearch, ByVal Support As ITPSupport,
                                         ByVal CurrencyCode As String, ByVal Settings As IDOTWSettings) As Integer

        Dim sCurrencyCode As String = Support.TPCurrencyLookup("DOTW", CurrencyCode)

        Dim oCurrencyCache As DOTWCurrencyCache = DOTWSupport.GetCurrencyCache(SearchDetails, Settings)
        If oCurrencyCache.ContainsKey(sCurrencyCode) Then
            Return oCurrencyCache(sCurrencyCode)
        Else
            Return Settings.DefaultCurrencyID(SearchDetails)
        End If

        Return sCurrencyCode.ToSafeInt()

    End Function

    Public Shared Function GetCurrencyCode(ByVal CurrencyID As Integer, ByVal SearchDetails As IThirdPartyAttributeSearch, ByVal Settings As IDOTWSettings) As Integer

        Dim sCurrencyCode As String =
            SQL.GetValue("select CurrencyCode from TPCurrencyLookup where CurrencyID <> 0 and CurrencyID = {0} and source = {1}",
                CurrencyID, SQL.GetSqlValue(ThirdParties.DOTW, SQL.SqlValueType.String))

        Dim oCurrencyCache As DOTWCurrencyCache = DOTWSupport.GetCurrencyCache(SearchDetails, Settings)
        If oCurrencyCache.ContainsKey(sCurrencyCode) Then
            Return oCurrencyCache(sCurrencyCode)
        Else
            Return Settings.DefaultCurrencyID(SearchDetails)
        End If

    End Function

    Friend Shared Function CleanName(ByVal Name As String, ByVal Support As ITPSupport) As String

        Dim sCleanName As String = Name

        'Remove spaces
        sCleanName = sCleanName.Replace(" "c, String.Empty)

        'Try and convert to latin characters. This also replaces special charaters and numbers
        sCleanName = SettingsSupport.AngliciseString(sCleanName)

        'If it's under 2 characters long then pad with X
        If sCleanName.Length < 2 Then
            sCleanName = sCleanName.PadRight(2, "X"c)
        End If

        'Don't allow it to be longer than 25 characters
        If sCleanName.Length > 25 Then
            sCleanName = sCleanName.Substring(0, 25)
        End If

        Return sCleanName

    End Function
End Class

Public Class DOTWCurrencyCache
    Inherits Generic.Dictionary(Of String, Integer)
End Class

