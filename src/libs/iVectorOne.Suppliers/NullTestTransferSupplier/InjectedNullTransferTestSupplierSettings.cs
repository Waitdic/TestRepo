namespace iVectorOne.Suppliers
{
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public class InjectedNullTestTransferSupplierSettings : SettingsBase, INullTestTransferSupplierSettings
    {
        protected override string Source => ThirdParties.NULLTESTTRANSFERSUPPLIER;

        public int SearchTimeMilliseconds(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            int.TryParse(Get_Value("SearchTimeMilliseconds", tpAttributeSearch), out int timeout);
            return timeout;
        }
    }
}