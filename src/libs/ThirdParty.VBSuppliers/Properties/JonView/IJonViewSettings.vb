Imports ThirdParty.Abstractions

Public Interface IJonViewSettings
    ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    ReadOnly Property OffsetCancellationDays(ByVal tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer
    ReadOnly Property URL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property UserID(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    ReadOnly Property ClientLoc(tpAttributeSearch As IThirdPartyAttributeSearch) As String
End Interface
