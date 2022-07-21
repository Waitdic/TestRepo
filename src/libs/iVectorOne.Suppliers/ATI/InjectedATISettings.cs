namespace iVectorOne.Suppliers.ATI
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedATISettings : SettingsBase, IATISettings
    {
        protected override string Source => ThirdParties.ATI;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public string APIVersion(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("APIVersion", tpAttributeSearch);
        }

        public string UserID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UserID", tpAttributeSearch);
        }

        public bool ExcludeNonRefundable(IThirdPartyAttributeSearch tpAttributeSearch, bool isMandatory)
        {
            return Get_Value("ExcludeNonRefundable", tpAttributeSearch).ToSafeBoolean();
        }
    }
}
