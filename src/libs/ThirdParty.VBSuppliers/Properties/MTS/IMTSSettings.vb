Imports ThirdParty.Abstractions

Public Interface IMTSSettings
        ReadOnly Property ID(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property Type(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
        ReadOnly Property Instance(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property BaseURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property Unique_ID_Type(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
        ReadOnly Property OverRideID(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property LanguageCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, IsMandatory As Boolean) As Integer
        ReadOnly Property AuthenticationType(tpAttributeSearch As IThirdPartyAttributeSearch) As Integer
        ReadOnly Property MessagePassword(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property ID_Context(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property OverrideCountries(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property AuthenticationID(tpAttributeSearch As IThirdPartyAttributeSearch) As String
    End Interface
