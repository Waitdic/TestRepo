Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Public Class InjectedSunHotelsSettings
    Implements ISunHotelsSettings

    Private ReadOnly configuration As ThirdPartyConfiguration

    Public Sub New(ByVal configuration As ThirdPartyConfiguration)
        Me.configuration = configuration
    End Sub


    Public ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.Password
        Get
            Return Get_Value("Password", configuration)
        End Get
    End Property

    Public ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.Username
        Get
            Return Get_Value("Username", configuration)
        End Get
    End Property

    Public ReadOnly Property EmailAddress(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.EmailAddress
        Get
            Return Get_Value("EmailAddress", configuration)
        End Get
    End Property

    Public ReadOnly Property Language(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.Language
        Get
            Return Get_Value("Language", configuration)
        End Get
    End Property

    Public ReadOnly Property Currency(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.Currency
        Get
            Return Get_Value("Currency", configuration)
        End Get
    End Property

    Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements ISunHotelsSettings.AllowCancellations
        Get
            Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property SupplierReference(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.SupplierReference
        Get
            Return Get_Value("SupplierReference", configuration)
        End Get
    End Property

    Public ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements ISunHotelsSettings.UseGZip
        Get
            Return Get_Value("UseGZip", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property SearchURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.SearchURL
        Get
            Return Get_Value("SearchURL", configuration)
        End Get
    End Property

    Public ReadOnly Property BookURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.BookURL
        Get
            Return Get_Value("BookURL", configuration)
        End Get
    End Property

    Public ReadOnly Property CancelURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.CancelURL
        Get
            Return Get_Value("CancelURL", configuration)
        End Get
    End Property

    Public ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, isMandotory As Boolean) As Integer Implements ISunHotelsSettings.OffsetCancellationDays
        Get
            Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property Nationality(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.Nationality
        Get
            Return Get_Value("Nationality", configuration)
        End Get
    End Property

    Public ReadOnly Property PreBookURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.PreBookURL
        Get
            Return Get_Value("PreBookURL", configuration)
        End Get
    End Property

    Public ReadOnly Property AccommodationTypes(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ISunHotelsSettings.AccommodationTypes
        Get
            Return Get_Value("AccommodationTypes", configuration)
        End Get
    End Property

    Public ReadOnly Property RequestPackageRates(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements ISunHotelsSettings.RequestPackageRates
        Get
            Return Get_Value("RequestPackageRates", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property HotelRequestLimit(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements ISunHotelsSettings.HotelRequestLimit
        Get
            Return Get_Value("HotelRequestLimit", configuration).ToSafeInt()
        End Get
    End Property
End Class
