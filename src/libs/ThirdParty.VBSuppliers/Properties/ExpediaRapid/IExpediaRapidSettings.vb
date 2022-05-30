Imports ThirdParty.Abstractions

Namespace ExpediaRapid

    Public Interface IExpediaRapidSettings
        ReadOnly Property ApiKey(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property Secret(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property Scheme(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property Host(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property SearchPath(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property SearchRequestBatchSize(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
        ReadOnly Property CountryCode(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property SalesChannel(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property SalesEnvironment(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property SortType(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property RatePlanCount(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
        ReadOnly Property PaymentTerms(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property PartnerPointOfSale(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property LanguageCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property UseGZIP(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property AllowCancellations(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property OffsetCancellationDays(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
        ReadOnly Property UserAgent(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property BillingTerms(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property PlatformName(ByVal tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property RateOption(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property ValidateAffiliateID(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
    End Interface

End Namespace