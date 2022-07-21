namespace ThirdParty.CSSuppliers.HotelsProV2
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedHotelsProV2Settings : SettingsBase, IHotelsProV2Settings
    {
        protected override string Source => ThirdParties.HOTELSPROV2;

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string BookURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("BookURL", tpAttributeSearch);
        }

        public string CancelURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancelURL", tpAttributeSearch);
        }

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Currency", tpAttributeSearch);
        }

        public string HotelAvailabilityURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelAvailabilityURL", tpAttributeSearch);
        }

        public int MaxHotelCodesPerRequest(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaxHotelCodesPerRequest", tpAttributeSearch).ToSafeInt();
        }

        public string Nationality(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Nationality", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string ProvisionURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ProvisionURL", tpAttributeSearch);
        }

        public string SearchURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SearchURL", tpAttributeSearch);
        }

        public bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZIP", tpAttributeSearch).ToSafeBoolean();
        }

        public bool UseMultiHotelCodesSearch(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseMultiHotelCodesSearch", tpAttributeSearch).ToSafeBoolean();
        }

        public string UserName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UserName", tpAttributeSearch);
        }
    }
}
