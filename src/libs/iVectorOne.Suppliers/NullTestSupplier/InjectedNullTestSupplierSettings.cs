namespace iVectorOne.CSSuppliers
{
    using iVectorOne.Constants;
    using iVectorOne.Support;

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