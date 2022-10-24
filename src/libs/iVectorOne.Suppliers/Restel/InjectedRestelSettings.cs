namespace iVectorOne.Suppliers.Restel
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedRestelSettings : SettingsBase, IRestelSettings
    {
        protected override string Source => ThirdParties.RESTEL;

        public string Codusu(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Codusu", tpAttributeSearch);
        }

        public string Codigousu(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Codigousu", tpAttributeSearch);
        }

        public string Clausu(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Clausu", tpAttributeSearch);
        }

        public string Secacc(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Secacc", tpAttributeSearch);
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public bool GuestNamesAvailable(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GuestNamesAvailable", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }
    }
}
