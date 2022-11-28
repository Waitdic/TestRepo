namespace iVectorOne.Suppliers
{
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public abstract class InjectedTourPlanTransfersSettings : SettingsBase, ITourPlanTransfersSettings
    {
        public string URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public string AgentId(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentId", tpAttributeSearch);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }
    }
}