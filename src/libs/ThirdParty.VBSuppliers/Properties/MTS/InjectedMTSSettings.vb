Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Public Class InjectedMTSSettings
    Implements IMTSSettings

    Private ReadOnly configuration As ThirdPartyConfiguration

    Public Sub New(ByVal configuration As ThirdPartyConfiguration)
        Me.configuration = configuration
    End Sub

    Public ReadOnly Property ID(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IMTSSettings.ID
        Get
            Return Get_Value("ID", configuration)
        End Get
    End Property

    Public ReadOnly Property Type(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IMTSSettings.Type
        Get
            Return Get_Value("Type", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property BaseURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IMTSSettings.BaseURL
        Get
            Return Get_Value("BaseURL", configuration)
        End Get
    End Property

    Public ReadOnly Property Unique_ID_Type(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IMTSSettings.Unique_ID_Type
        Get
            Return Get_Value("Unique_ID_Type", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property OverRideID(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IMTSSettings.OverRideID
        Get
            Return Get_Value("OverRideID", configuration)
        End Get
    End Property

    Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IMTSSettings.AllowCancellations
        Get
            Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property LanguageCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IMTSSettings.LanguageCode
        Get
            Return Get_Value("LanguageCode", configuration)
        End Get
    End Property

    Public ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IMTSSettings.UseGZip
        Get
            Return Get_Value("UseGZIP", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer Implements IMTSSettings.OffsetCancellationDays
        Get
            Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property AuthenticationType(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IMTSSettings.AuthenticationType
        Get
            Return Get_Value("AuthenticationType", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property MessagePassword(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IMTSSettings.MessagePassword
        Get
            Return Get_Value("MessagePassword", configuration)
        End Get
    End Property

    Public ReadOnly Property ID_Context(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IMTSSettings.ID_Context
        Get
            Return Get_Value("ID_Context", configuration)
        End Get
    End Property

    Public ReadOnly Property OverrideCountries(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IMTSSettings.OverrideCountries
        Get
            Return Get_Value("OverrideCountries", configuration)
        End Get
    End Property

    Public ReadOnly Property AuthenticationID(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IMTSSettings.AuthenticationID
        Get
            Return Get_Value("AuthenticationID", configuration)
        End Get
    End Property

    Public ReadOnly Property Instance(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IMTSSettings.Instance
        Get
            Return Get_Value("Instance", configuration)
        End Get
    End Property

End Class