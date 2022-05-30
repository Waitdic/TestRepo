Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Public Class InjectedJonViewSettings
    Implements IJonViewSettings

    Private ReadOnly configuration As ThirdPartyConfiguration

    Public Sub New(ByVal configuration As ThirdPartyConfiguration)
        Me.configuration = configuration
    End Sub

    Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IJonViewSettings.AllowCancellations
        Get
            Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer Implements IJonViewSettings.OffsetCancellationDays
        Get
            Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property URL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJonViewSettings.URL
        Get
            Return Get_Value("URL", configuration)
        End Get
    End Property

    Public ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJonViewSettings.Password
        Get
            Return Get_Value("Password", configuration)
        End Get
    End Property

    Public ReadOnly Property UserID(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJonViewSettings.UserID
        Get
            Return Get_Value("UserID", configuration)
        End Get
    End Property

    Public ReadOnly Property ClientLoc(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJonViewSettings.ClientLoc
        Get
            Return Get_Value("ClientLoc", configuration)
        End Get
    End Property
End Class
