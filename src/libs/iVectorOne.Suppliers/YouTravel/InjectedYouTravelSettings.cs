namespace iVectorOne.Suppliers.YouTravel
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedYouTravelSettings : SettingsBase, IYouTravelSettings
    {
        protected override string Source => ThirdParties.YOUTRAVEL;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public string BookingURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookingURL", tpAttributeSearch);
        }

        public string CancellationFeeURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationFeeURL", tpAttributeSearch);
        }

        public string CancellationURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationURL", tpAttributeSearch);
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageCode", tpAttributeSearch);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }

        public string PrebookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PrebookURL", tpAttributeSearch);
        }

        public string CancellationPolicyURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancellationPolicyURL", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }
    }
}