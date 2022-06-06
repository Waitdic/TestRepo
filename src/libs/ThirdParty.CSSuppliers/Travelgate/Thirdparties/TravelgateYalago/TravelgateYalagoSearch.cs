namespace ThirdParty.CSSuppliers.Travelgate
{
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class TravelgateYalagoSearch : TravelgateSearch
    {
        public TravelgateYalagoSearch(
            ITravelgateYalagoSettings settings,
            ITPSupport support,
            ISecretKeeper secretKeeper,
            ISerializer serializer)
            : base(settings, support, secretKeeper, serializer)
        {
        }

        public override string Source => ThirdParties.TRAVELGATEYALAGO;
    }
}