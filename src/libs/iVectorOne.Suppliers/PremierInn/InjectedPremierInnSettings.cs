namespace iVectorOne.Suppliers.PremierInn
{
    using iVectorOne.Support;

    public class InjectedPremierInnSettings : SettingsBase, IPremierInnSettings
    {
        protected override string Source { get; }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public int HotelSearchLimit(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public string LanguageCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }

        public bool AllowOnRequest(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }
    }
}
