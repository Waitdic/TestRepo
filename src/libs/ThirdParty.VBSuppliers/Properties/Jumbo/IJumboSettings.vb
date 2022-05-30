Imports ThirdParty.Abstractions

Public Interface IJumboSettings
        ReadOnly Property AgencyCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property BrandCode(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property POS(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property CommonsURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property HotelBookingURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property BasketHandlerURL(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property AllowCancellations(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property Language(tpAttributeSearch As IThirdPartyAttributeSearch) As String
        ReadOnly Property UseGZip(tpAttributeSearch As IThirdPartyAttributeSearch) As Boolean
        ReadOnly Property OffsetCancellationDays(tpAttributeSearch As IThirdPartyAttributeSearch, isMandatory As Boolean) As Integer
        ReadOnly Property NationalityBasedCredentials(tpAttributeSearch As IThirdPartyAttributeSearch) As String
End Interface
