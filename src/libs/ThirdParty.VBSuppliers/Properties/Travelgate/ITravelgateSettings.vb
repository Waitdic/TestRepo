Imports ThirdParty.Abstractions

Public Interface ITravelgateSettings
    ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property URL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property SearchSOAPAction(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property PrebookSOAPAction(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property BookSOAPAction(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property CancelSOAPAction(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer
    ReadOnly Property RequiresVCard(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property ReferenceDelimiter(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As String
    ReadOnly Property DefaultNationality(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As String
    ReadOnly Property CardHolderName(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property EncryptedCardDetails(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property ProviderUsername(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property ProviderPassword(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property ProviderCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property UrlReservation(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property UrlGeneric(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property UrlValuation(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property UrlAvail(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property LanguageCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Parameters(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Markets(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property CurrencyCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property MaximumHotelSearchNumber(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
    ReadOnly Property MaximumCitySearchNumber(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
    ReadOnly Property MaximumRoomNumber(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
    ReadOnly Property MaximumRoomGuestNumber(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
    ReadOnly Property MinimumStay(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
    ReadOnly Property AllowHotelSearch(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property UseZoneSearch(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property SearchRequestTimeout(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
    ReadOnly Property RatePlanCodes(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property SendGUIDReference(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
End Interface
