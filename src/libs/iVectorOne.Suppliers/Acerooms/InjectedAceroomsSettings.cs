namespace iVectorOne.CSSuppliers.Acerooms
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedAceroomsSettings : SettingsBase, IAceroomsSettings
    {
        protected override string Source => ThirdParties.ACEROOMS;

        public string APIKey(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("APIKey", tpAttributeSearch);
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public string Secret(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Secret", tpAttributeSearch);
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

        public int HotelBatchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelBatchLimit", tpAttributeSearch).ToSafeInt();
        }
    }
}