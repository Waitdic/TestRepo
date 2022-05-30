namespace ThirdParty.CSSuppliers.Restel
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Support;

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

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public bool GuestNamesAvailable(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GuestNamesAvailable", tpAttributeSearch).ToSafeBoolean();
        }

        public bool UseGZIP(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZIP", tpAttributeSearch).ToSafeBoolean();
        }

        public bool CancelFirstPreBook(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("CancelFirstPreBook", tpAttributeSearch).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string Afil(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Afil", tpAttributeSearch);
        }
    }
}
