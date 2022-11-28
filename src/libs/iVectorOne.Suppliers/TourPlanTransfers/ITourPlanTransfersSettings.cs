namespace iVectorOne.Suppliers
{
    public interface ITourPlanTransfersSettings
    {
        string URL(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentId(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
