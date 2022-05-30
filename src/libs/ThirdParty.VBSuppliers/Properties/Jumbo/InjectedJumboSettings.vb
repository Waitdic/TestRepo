Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Public Class InjectedJumboSettings
    Implements IJumboSettings

    Private ReadOnly configuration As ThirdPartyConfiguration

    Public Sub New(ByVal configuration As ThirdPartyConfiguration)
        Me.configuration = configuration
    End Sub

    Public ReadOnly Property AgencyCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJumboSettings.AgencyCode
        Get
            Return Get_Value("AgencyCode", configuration)
        End Get
    End Property

    Public ReadOnly Property BrandCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJumboSettings.BrandCode
        Get
            Return Get_Value("BrandCode", configuration)
        End Get
    End Property

    Public ReadOnly Property POS(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJumboSettings.POS
        Get
            Return Get_Value("POS", configuration)
        End Get
    End Property

    Public ReadOnly Property CommonsURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJumboSettings.CommonsURL
        Get
            Return Get_Value("CommonsURL", configuration)
        End Get
    End Property

    Public ReadOnly Property HotelBookingURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJumboSettings.HotelBookingURL
        Get
            Return Get_Value("HotelBookingURL", configuration)
        End Get
    End Property

    Public ReadOnly Property BasketHandlerURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJumboSettings.BasketHandlerURL
        Get
            Return Get_Value("BasketHandlerURL", configuration)
        End Get
    End Property

    Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IJumboSettings.AllowCancellations
        Get
            Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property Language(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJumboSettings.Language
        Get
            Return Get_Value("Language", configuration)
        End Get
    End Property

    Public ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements IJumboSettings.UseGZip
        Get
            Return Get_Value("UseGZIP", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, isMandatory As Boolean) As Integer Implements IJumboSettings.OffsetCancellationDays
        Get
            Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property NationalityBasedCredentials(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements IJumboSettings.NationalityBasedCredentials
        Get
            Return Get_Value("NationalityBasedCredentials", configuration)
        End Get
    End Property
End Class
