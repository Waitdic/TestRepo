Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Public Class InjectedBonotelSettings
    Implements IBonotelSettings

    Private ReadOnly configuration As ThirdPartyConfiguration

    Public Sub New(ByVal configuration As ThirdPartyConfiguration)
        Me.configuration = configuration
    End Sub

    Public ReadOnly Property URL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IBonotelSettings.URL
        Get
            Return Get_Value("URL", configuration)
        End Get
    End Property

    Public ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IBonotelSettings.Username
        Get
            Return Get_Value("Username", configuration)
        End Get
    End Property

    Public ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IBonotelSettings.Password
        Get
            Return Get_Value("Password", configuration)
        End Get
    End Property

    Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Boolean Implements IBonotelSettings.AllowCancellations
        Get
            Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer Implements IBonotelSettings.OffsetCancellationDays
        Get
            Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property BookTimeout(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements IBonotelSettings.BookTimeout
        Get
            Return Get_Value("BookTimeout", configuration).ToSafeInt()
        End Get
    End Property

End Class
