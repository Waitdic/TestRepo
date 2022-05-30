namespace ThirdParty.CSSuppliers
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedAceroomsSettings : SettingsBase, IAceroomsSettings
    {
        protected override string Source => ThirdParties.ACEROOMS;

        public string APIKey(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("APIKey", tpAttributeSearch);
        }

        public string BaseURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BaseURL", tpAttributeSearch);
        }

        public string SecretKey(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SecretKey", tpAttributeSearch);
        }

        public string Signature(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Signature", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int BatchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BatchLimit", tpAttributeSearch).ToSafeInt();
        }
    }
}

