namespace iVectorOne.Suppliers
{
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Support;

    public abstract class InjectedTourPlanTransfersSettings : SettingsBase, ITourPlanTransfersSettings
    {
        public int SearchTimeMilliseconds(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            throw new System.NotImplementedException();
        }
    }
}