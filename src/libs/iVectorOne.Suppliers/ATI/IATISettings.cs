namespace iVectorOne.Suppliers.ATI
{
    public interface IATISettings
    {
         bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
         int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
         string Password(IThirdPartyAttributeSearch tpAttributeSearch);
         string URL(IThirdPartyAttributeSearch tpAttributeSearch); 
         string APIVersion(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory);
         string UserID(IThirdPartyAttributeSearch tpAttributeSearch); 
         bool ExcludeNonRefundable(IThirdPartyAttributeSearch tpAttributeSearch,bool isMandatory);
    }
}
