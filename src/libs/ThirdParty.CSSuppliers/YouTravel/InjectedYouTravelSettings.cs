namespace ThirdParty.CSSuppliers.YouTravel
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Search.Settings;
    using ThirdParty.Support;

    public class InjectedYouTravelSettings : SettingsBase, IYouTravelSettings
    {

        private readonly ThirdPartyConfiguration configuration;

        protected override string Source => ThirdParties.YOUTRAVEL;

        public InjectedYouTravelSettings(ThirdPartyConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string get_Username(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Username", tpAttributeSearch);
        }

        public string get_Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string get_SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public string get_BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string get_CancellationFeeURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationFeeURL", tpAttributeSearch);
        }

        public string get_CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationURL", tpAttributeSearch);
        }

        public string get_LangID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LangID", tpAttributeSearch);
        }

        public bool get_UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZIP", tpAttributeSearch).ToSafeBoolean();
        }

        public string get_PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PrebookURL", tpAttributeSearch);
        }

        public string get_CancellationPolicyURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationPolicyURL", tpAttributeSearch);
        }

        private int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        int IYouTravelSettings.get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory) => get_OffsetCancellationDays(tpAttributeSearch, IsMandatory);

    }
}