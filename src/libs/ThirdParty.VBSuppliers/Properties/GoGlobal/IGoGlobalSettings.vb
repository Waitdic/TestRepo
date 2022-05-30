Imports ThirdParty.Abstractions

Public Interface IGoGlobalSettings
        ReadOnly Property Agency(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property User(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property URL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property UseGZIP(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property SecondsBeforeSearchCutoff(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
        ReadOnly Property HotelSearchLimit(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
        ReadOnly Property BatchLimit(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
        ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer
End Interface
