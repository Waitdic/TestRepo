namespace iVectorOne.Suppliers
{
    using iVectorOne.Models;
    public class InjectedNullTestTransferSupplierSettings : INullTestTransferSupplierSettings
    {
        private readonly ThirdPartyConfiguration configuration;

        public InjectedNullTestTransferSupplierSettings(ThirdPartyConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public int SearchTimeMilliseconds(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            //int.TryParse(Get_Value("SearchTimeMilliseconds", configuration), out int timeout);
            return 500;
        }
    }
}