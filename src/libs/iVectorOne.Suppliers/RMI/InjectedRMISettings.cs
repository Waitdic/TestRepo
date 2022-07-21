namespace ThirdParty.CSSuppliers.RMI
{
    using Intuitive.Helpers.Extensions;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedRMISettings : SettingsBase, IRMISettings
    {
        protected override string Source => ThirdParties.RMI;

        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }
        public string Login(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Login", tpAttributeSearch);
        }
        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }
        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }
        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch).ToSafeBoolean();
        }
        public string Version(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Version", tpAttributeSearch);
        }
        public string DefaultCancellationReason(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DefaultCancellationReason", tpAttributeSearch);
        }
        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }
        public string RequestedMealBases(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RequestedMealBases", tpAttributeSearch);
        }
    }
}