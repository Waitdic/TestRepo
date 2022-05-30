Imports Intuitive.Helpers.Extensions
Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Search.Settings
Imports ThirdParty.Abstractions.Support.SettingsSupport

Public Class InjectedTravelgateSettings
    Implements ITravelgateSettings

    Private ReadOnly configuration As ThirdPartyConfiguration

    Public Sub New(ByVal configuration As ThirdPartyConfiguration)
        Me.configuration = configuration
    End Sub

    Public ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.Username
        Get
            Return Get_Value("Username", configuration)
        End Get
    End Property

    Public ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.Password
        Get
            Return Get_Value("Password", configuration)
        End Get
    End Property

    Public ReadOnly Property URL(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.URL
        Get
            Return Get_Value("URL", configuration)
        End Get
    End Property

    Public ReadOnly Property SearchSOAPAction(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.SearchSOAPAction
        Get
            Return Get_Value("SearchSOAPAction", configuration)
        End Get
    End Property

    Public ReadOnly Property PrebookSOAPAction(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.PrebookSOAPAction
        Get
            Return Get_Value("PrebookSOAPAction", configuration)
        End Get
    End Property

    Public ReadOnly Property BookSOAPAction(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.BookSOAPAction
        Get
            Return Get_Value("BookSOAPAction", configuration)
        End Get
    End Property

    Public ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements ITravelgateSettings.AllowCancellations
        Get
            Return Get_Value("AllowCancellations", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property CancelSOAPAction(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.CancelSOAPAction
        Get
            Return Get_Value("CancelSOAPAction", configuration)
        End Get
    End Property

    Public ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements ITravelgateSettings.UseGZip
        Get
            Return Get_Value("UseGZIP", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer Implements ITravelgateSettings.OffsetCancellationDays
        Get
            Return Get_Value("OffsetCancellationDays", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property RequiresVCard(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements ITravelgateSettings.RequiresVCard
        Get
            Return Get_Value("RequiresVCard", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property ReferenceDelimiter(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As String Implements ITravelgateSettings.ReferenceDelimiter
        Get
            Return Get_Value("ReferenceDelimiter", configuration)
        End Get
    End Property

    Public ReadOnly Property DefaultNationality(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As String Implements ITravelgateSettings.DefaultNationality
        Get
            Return Get_Value("DefaultNationality", configuration)
        End Get
    End Property

    Public ReadOnly Property CardHolderName(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.CardHolderName
        Get
            Return Get_Value("CardHolderName", configuration)
        End Get
    End Property

    Public ReadOnly Property EncryptedCardDetails(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.EncryptedCardDetails
        Get
            Return Get_Value("EncryptedCardDetails", configuration)
        End Get
    End Property

    Public ReadOnly Property Markets(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.Markets
        Get
            Return Get_Value("Markets", configuration)
        End Get
    End Property

    Public ReadOnly Property ProviderUsername(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.ProviderUsername
        Get
            Return Get_Value("ProviderUsername", configuration)
        End Get
    End Property

    Public ReadOnly Property ProviderPassword(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.ProviderPassword
        Get
            Return Get_Value("ProviderPassword", configuration)
        End Get
    End Property

    Public ReadOnly Property ProviderCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.ProviderCode
        Get
            Return Get_Value("ProviderCode", configuration)
        End Get
    End Property

    Public ReadOnly Property UrlReservation(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.UrlReservation
        Get
            Return Get_Value("UrlReservation", configuration)
        End Get
    End Property

    Public ReadOnly Property UrlGeneric(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.UrlGeneric
        Get
            Return Get_Value("UrlGeneric", configuration)
        End Get
    End Property

    Public ReadOnly Property UrlValuation(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.UrlValuation
        Get
            Return Get_Value("UrlValuation", configuration)
        End Get
    End Property

    Public ReadOnly Property UrlAvail(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.UrlAvail
        Get
            Return Get_Value("UrlAvail", configuration)
        End Get
    End Property

    Public ReadOnly Property LanguageCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.LanguageCode
        Get
            Return Get_Value("LanguageCode", configuration)
        End Get
    End Property

    Public ReadOnly Property Parameters(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.Parameters
        Get
            Return Get_Value("Parameters", configuration)
        End Get
    End Property

    Public ReadOnly Property CurrencyCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.CurrencyCode
        Get
            Return Get_Value("CurrencyCode", configuration)
        End Get
    End Property

    Public ReadOnly Property MaximumHotelSearchNumber(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements ITravelgateSettings.MaximumHotelSearchNumber
        Get
            Return Get_Value("MaximumHotelSearchNumber", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property MaximumCitySearchNumber(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements ITravelgateSettings.MaximumCitySearchNumber
        Get
            Return Get_Value("MaximumCitySearchNumber", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property MaximumRoomNumber(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements ITravelgateSettings.MaximumRoomNumber
        Get
            Return Get_Value("MaximumRoomNumber", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property MaximumRoomGuestNumber(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements ITravelgateSettings.MaximumRoomGuestNumber
        Get
            Return Get_Value("MaximumRoomGuestNumber", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property MinimumStay(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements ITravelgateSettings.MinimumStay
        Get
            Return Get_Value("MinimumStay", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property AllowHotelSearch(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements ITravelgateSettings.AllowHotelSearch
        Get
            Return Get_Value("AllowHotelSearch", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property UseZoneSearch(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements ITravelgateSettings.UseZoneSearch
        Get
            Return Get_Value("UseZoneSearch", configuration).ToSafeBoolean()
        End Get
    End Property

    Public ReadOnly Property SearchRequestTimeout(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer Implements ITravelgateSettings.SearchRequestTimeout
        Get
            Return Get_Value("SearchRequestTimeout", configuration).ToSafeInt()
        End Get
    End Property

    Public ReadOnly Property RatePlanCodes(tpAttributeSearch As IThirdPartyAttributeSearch) As String Implements ITravelgateSettings.RatePlanCodes
        Get
            Return Get_Value("RatePlanCodes", configuration)
        End Get
    End Property

    Public ReadOnly Property SendGUIDReference(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean Implements ITravelgateSettings.SendGUIDReference
        Get
            Return Get_Value("SendGUIDReference", configuration).ToSafeBoolean()
        End Get
    End Property
End Class
