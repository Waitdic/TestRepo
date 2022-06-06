namespace ThirdParty.CSSuppliers.Travelgate
{
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class TravelgateIxpiraSearch : TravelgateSearch
    {
        public TravelgateIxpiraSearch(
            ITravelgateIxpiraSettings settings,
            ITPSupport support,
            ISecretKeeper secretKeeper,
            ISerializer serializer)
            : base(settings, support, secretKeeper, serializer)
        {
        }

        public override string Source => ThirdParties.TRAVELGATEIXPIRA;
    }
}