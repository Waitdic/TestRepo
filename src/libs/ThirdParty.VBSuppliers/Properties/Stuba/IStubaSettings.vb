Imports ThirdParty.Abstractions

Public Interface IStubaSettings
    ReadOnly Property URL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property MaxHotelsPerRequest(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
    ReadOnly Property Organisation(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Version(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Currency(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Nationality(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property ExcludeNonRefundableRates(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property ExcludeUnknownCancellationPolicys(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer
End Interface
