namespace iVectorOne.Suppliers
{
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedTestTransferSupplierSettings : SettingsBase, ITestTransferSupplierSettings
    {
        protected override string Source => ThirdParties.TESTTRANSFERSUPPLIER;

        public int SearchTimeMilliseconds(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            var timeout = 500;
            //int.TryParse(Get_Value("SearchTimeMilliseconds", tpAttributeSearch), out int timeout);
            return timeout;
        }
    }
}