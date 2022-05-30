Imports ThirdParty.Abstractions

Public Interface IYouTravelSettings
    ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Boolean
    ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property SearchURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property BookingURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property CancellationFeeURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property CancellationURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property LangID(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer
    ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property PrebookURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property CancellationPolicyURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
End Interface
