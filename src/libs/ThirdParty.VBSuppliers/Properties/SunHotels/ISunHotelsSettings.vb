Imports ThirdParty.Abstractions

Public Interface ISunHotelsSettings
    ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property EmailAddress(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Language(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Currency(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property SupplierReference(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property SearchURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property BookURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property CancelURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, isMandotory As Boolean) As Integer
    ReadOnly Property Nationality(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property PreBookURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property AccommodationTypes(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property RequestPackageRates(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property HotelRequestLimit(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
End Interface
