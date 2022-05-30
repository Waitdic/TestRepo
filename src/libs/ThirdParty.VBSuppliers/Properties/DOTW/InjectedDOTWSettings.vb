Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Public Class InjectedDOTWSettings
    Implements IDOTWSettings

    Private ReadOnly configuration As ThirdPartyConfiguration

    Public Sub New(ByVal configuration As ThirdPartyConfiguration)
        Me.configuration = configuration
    End Sub


    Public ReadOnly Property CompanyCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IDOTWSettings.CompanyCode
        Get
            Return Get_Value("CompanyCode", configuration)
        End Get
    End Property

    Public ReadOnly Property CustomerCountryCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IDOTWSettings.CustomerCountryCode
        Get
            Return Get_Value("CustomerCountryCode", configuration)
        End Get
    End Property

    Public ReadOnly Property CustomerNationality(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IDOTWSettings.CustomerNationality
        Get
            Return Get_Value("CustomerNationality", configuration)
        End Get
    End Property

    Public ReadOnly Property DefaultCurrencyID(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IDOTWSettings.DefaultCurrencyID
        Get
            Return Get_Value("DefaultCurrencyID", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property ExcludeDOTWThirdParties(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IDOTWSettings.ExcludeDOTWThirdParties
        Get
            Return Get_Value("ExcludeDOTWThirdParties", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IDOTWSettings.Password
        Get
            Return Get_Value("Password", configuration)
        End Get
    End Property

    Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IDOTWSettings.AllowCancellations
        Get
            Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property RequestCurrencyID(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IDOTWSettings.RequestCurrencyID
        Get
            Return Get_Value("RequestCurrencyID", configuration)
        End Get
    End Property

    Public ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IDOTWSettings.UseGZip
        Get
            Return Get_Value("UseGZIP", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer Implements IDOTWSettings.OffsetCancellationDays
        Get
            Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property SendTradeReference(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IDOTWSettings.SendTradeReference
        Get
            Return Get_Value("SendTradeReference", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property ServerURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IDOTWSettings.ServerURL
        Get
            Return Get_Value("ServerURL", configuration)
        End Get
    End Property

    Public ReadOnly Property ThreadedSearch(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IDOTWSettings.ThreadedSearch
        Get
            Return Get_Value("ThreadedSearch", configuration)
        End Get
    End Property

    Public ReadOnly Property UseMinimumSellingPrice(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IDOTWSettings.UseMinimumSellingPrice
        Get
            Return Get_Value("UseMinimumSellingPrice", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IDOTWSettings.Username
        Get
            Return Get_Value("Username", configuration)
        End Get
    End Property

    Public ReadOnly Property Version(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IDOTWSettings.Version
        Get
            Return Get_Value("Version", configuration)
        End Get
    End Property

End Class
