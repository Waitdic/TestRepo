Imports ThirdParty.Abstractions
Imports ThirdParty.Abstractions.Lookups

Public Interface IDOTWSupport
    Function GetCachedCurrencyID(ByVal SearchDetails As IThirdPartyAttributeSearch, ByVal Support As ITPSupport,
                                         ByVal CurrencyCode As String, ByVal Settings As IDOTWSettings) As Integer

End Interface
