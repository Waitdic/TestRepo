namespace iVectorOne.Suppliers.ATI
{
    public interface IATISettings
    {
         bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
         int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
         string Password(IThirdPartyAttributeSearch tpAttributeSearch);
         string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch); 
         string Version(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
         string User(IThirdPartyAttributeSearch tpAttributeSearch); 
         bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch,bool isMandatory);
         bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
