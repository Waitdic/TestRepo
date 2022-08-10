namespace iVectorOne.Suppliers.GoGlobal
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedGoGlobalSettings : SettingsBase, IGoGlobalSettings
    {
        protected override string Source => ThirdParties.GOGLOBAL;

        public string Agency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Agency", tpAttributeSearch);
        }
        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch);
        }
        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }
        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }
        public bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZIP", tpAttributeSearch).ToSafeBoolean();
        }
        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }
        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelSearchLimit", tpAttributeSearch).ToSafeInt();
        }
        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }
        public string LeadGuestNationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultNationality", tpAttributeSearch);
        }
    }
}
