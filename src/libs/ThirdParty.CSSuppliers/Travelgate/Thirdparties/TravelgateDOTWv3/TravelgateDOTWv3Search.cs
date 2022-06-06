namespace ThirdParty.CSSuppliers.Travelgate
{
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class TravelgateDOTWv3Search : TravelgateSearch
    {
        public TravelgateDOTWv3Search(
            ITravelgateDOTWv3Settings settings,
            ITPSupport support,
            ISecretKeeper secretKeeper,
            ISerializer serializer)
            : base(settings, support, secretKeeper, serializer)
        {
        }

        public override string Source => ThirdParties.TRAVELGATEDOTWV3;
    }
}