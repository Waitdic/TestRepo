Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Public Class InjectedGoGlobalSettings
    Implements IGoGlobalSettings

    Private ReadOnly configuration As ThirdPartyConfiguration

    Public Sub New(ByVal configuration As ThirdPartyConfiguration)
        Me.configuration = configuration
    End Sub


    Public ReadOnly Property Agency(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IGoGlobalSettings.Agency
        Get
            Return Get_Value("Agency", configuration)
        End Get
    End Property

    Public ReadOnly Property User(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IGoGlobalSettings.User
        Get
            Return Get_Value("User", configuration)
        End Get
    End Property

    Public ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IGoGlobalSettings.Password
        Get
            Return Get_Value("Password", configuration)
        End Get
    End Property

    Public ReadOnly Property URL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IGoGlobalSettings.URL
        Get
            Return Get_Value("URL", configuration)
        End Get
    End Property

    Public ReadOnly Property UseGZIP(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IGoGlobalSettings.UseGZIP
        Get
            Return Get_Value("UseGZIP", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IGoGlobalSettings.AllowCancellations
        Get
            Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property SecondsBeforeSearchCutoff(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IGoGlobalSettings.SecondsBeforeSearchCutoff
        Get
            Return Get_Value("SecondsBeforeSearchCutoff", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property HotelSearchLimit(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IGoGlobalSettings.HotelSearchLimit
        Get
            Return Get_Value("HotelSearchLimit", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property BatchLimit(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IGoGlobalSettings.BatchLimit
        Get
            Return Get_Value("BatchLimit", configuration).ToSafeInt()
        End Get
    End Property
    Public ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer Implements IGoGlobalSettings.OffsetCancellationDays
        Get
            Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
        End Get
    End Property
End Class
