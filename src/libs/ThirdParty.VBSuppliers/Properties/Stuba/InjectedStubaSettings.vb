Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Public Class InjectedStubaSettings
    Implements IStubaSettings

    Private ReadOnly configuration As ThirdPartyConfiguration

    Public Sub New(ByVal configuration As ThirdPartyConfiguration)
        Me.configuration = configuration
    End Sub

    Public ReadOnly Property URL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IStubaSettings.URL
        Get
            Return Get_Value("URL", configuration)
        End Get
    End Property

    Public ReadOnly Property MaxHotelsPerRequest(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IStubaSettings.MaxHotelsPerRequest
        Get
            Return Get_Value("MaxHotelsPerRequest", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property Organisation(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IStubaSettings.Organisation
        Get
            Return Get_Value("Organisation", configuration)
        End Get
    End Property

    Public ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IStubaSettings.Username
        Get
            Return Get_Value("Username", configuration)
        End Get
    End Property

    Public ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IStubaSettings.Password
        Get
            Return Get_Value("Password", configuration)
        End Get
    End Property

    Public ReadOnly Property Version(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IStubaSettings.Version
        Get
            Return Get_Value("Version", configuration)
        End Get
    End Property

    Public ReadOnly Property Currency(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IStubaSettings.Currency
        Get
            Return Get_Value("Currency", configuration)
        End Get
    End Property

    Public ReadOnly Property Nationality(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IStubaSettings.Nationality
        Get
            Return Get_Value("Nationality", configuration)
        End Get
    End Property

    Public ReadOnly Property ExcludeNonRefundableRates(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IStubaSettings.ExcludeNonRefundableRates
        Get
            Return Get_Value("ExcludeNonRefundableRates", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property ExcludeUnknownCancellationPolicys(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IStubaSettings.ExcludeUnknownCancellationPolicys
        Get
            Return Get_Value("ExcludeUnknownCancellationPolicys", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IStubaSettings.AllowCancellations
        Get
            Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer Implements IStubaSettings.OffsetCancellationDays
        Get
            Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
        End Get
    End Property

End Class
