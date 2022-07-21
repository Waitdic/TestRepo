namespace ThirdParty.CSSuppliers
{
    using ThirdParty.Constants;
    using ThirdParty.Support;

    public class InjectedNullTestSupplierSettings : SettingsBase, INullTestSupplierSettings
    {
        protected override string Source => ThirdParties.NULLTESTSUPPLIER;

        public int SearchTimeMilliseconds(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            int.TryParse(Get_Value("SearchTimeMilliseconds", tpAttributeSearch), out int timeout);
            return timeout;
        }
    }
}