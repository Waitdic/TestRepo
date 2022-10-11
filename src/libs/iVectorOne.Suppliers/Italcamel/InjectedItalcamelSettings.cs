namespace iVectorOne.Suppliers.Italcamel
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedItalcamelSettings : SettingsBase, IItalcamelSettings
    {
        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public string LanguageID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("LanguageID", tpAttributeSearch);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public int MaximumHotelSearchNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaximumHotelSearchNumber", tpAttributeSearch).ToSafeInt();
        }

        public int MaximumNumberOfNights(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaximumNumberOfNights", tpAttributeSearch).ToSafeInt();
        }

        public int MaximumRoomGuestNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaximumRoomGuestNumber", tpAttributeSearch).ToSafeInt();
        }

        public int MaximumRoomNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaximumRoomNumber", tpAttributeSearch).ToSafeInt();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public string Login(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Login", tpAttributeSearch);
        }

        protected override string Source => ThirdParties.ITALCAMEL;
    }
}

