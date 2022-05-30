Imports ThirdParty.Abstractions


Public Interface IBonotelSettings
    ReadOnly Property URL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Boolean
    ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer
    ReadOnly Property BookTimeout(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
End Interface
