Imports ThirdParty.Abstractions

Public Interface IDOTWSettings
        ReadOnly Property CompanyCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property CustomerCountryCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property CustomerNationality(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property DefaultCurrencyID(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
        ReadOnly Property ExcludeDOTWThirdParties(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property Password(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property RequestCurrencyID(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer
        ReadOnly Property SendTradeReference(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property ServerURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property ThreadedSearch(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property UseMinimumSellingPrice(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property Username(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property Version(tpAttributeSearch As IThirdPartyAttributeSearch) As String
End Interface
