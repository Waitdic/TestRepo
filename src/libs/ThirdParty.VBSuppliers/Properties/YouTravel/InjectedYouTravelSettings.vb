Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Public Class InjectedYouTravelSettings
    Implements IYouTravelSettings

    Private ReadOnly configuration As ThirdPartyConfiguration

    Public Sub New(ByVal configuration As ThirdPartyConfiguration)
        Me.configuration = configuration
    End Sub

    Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Boolean Implements IYouTravelSettings.AllowCancellations
        Get
            Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IYouTravelSettings.Username
        Get
            Return Get_Value("Username", configuration)
        End Get
    End Property

    Public ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IYouTravelSettings.Password
        Get
            Return Get_Value("Password", configuration)
        End Get
    End Property

    Public ReadOnly Property SearchURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IYouTravelSettings.SearchURL
        Get
            Return Get_Value("SearchURL", configuration)
        End Get
    End Property

    Public ReadOnly Property BookingURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IYouTravelSettings.BookingURL
        Get
            Return Get_Value("BookingURL", configuration)
        End Get
    End Property

    Public ReadOnly Property CancellationFeeURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IYouTravelSettings.CancellationFeeURL
        Get
            Return Get_Value("CancellationFeeURL", configuration)
        End Get
    End Property

    Public ReadOnly Property CancellationURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IYouTravelSettings.CancellationURL
        Get
            Return Get_Value("CancellationURL", configuration)
        End Get
    End Property

    Public ReadOnly Property LangID(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IYouTravelSettings.LangID
        Get
            Return Get_Value("LangID", configuration)
        End Get
    End Property

    Public ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IYouTravelSettings.UseGZip
        Get
            Return Get_Value("UseGZIP", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property PrebookURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IYouTravelSettings.PrebookURL
        Get
            Return Get_Value("PrebookURL", configuration)
        End Get
    End Property

    Public ReadOnly Property CancellationPolicyURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IYouTravelSettings.CancellationPolicyURL
        Get
            Return Get_Value("CancellationPolicyURL", configuration)
        End Get
    End Property

    Private ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer Implements IYouTravelSettings.OffsetCancellationDays
        Get
            Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
        End Get
    End Property

End Class
