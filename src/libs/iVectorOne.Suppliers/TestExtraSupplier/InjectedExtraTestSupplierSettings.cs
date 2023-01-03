namespace iVectorOne.Suppliers
{
    using iVectorOne.Constants;
    using iVectorOne.Support;
    
    public class InjectedExtraTestSupplierSettings : SettingsBase, ITestExtraSupplierSettings
    {
        protected override string Source => ThirdParties.TESTEXTRASUPPLIER;

        public int SearchTimeMilliseconds(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            var timeout = 500;
            //int.TryParse(Get_Value("SearchTimeMilliseconds", tpAttributeSearch), out int timeout);
            return timeout;
        }
    }
}
