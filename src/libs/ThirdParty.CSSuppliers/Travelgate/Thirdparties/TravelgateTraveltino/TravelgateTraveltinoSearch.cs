namespace ThirdParty.CSSuppliers.Travelgate
{
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class TravelgateTraveltinoSearch : TravelgateSearch
    {
        public TravelgateTraveltinoSearch(
            ITravelgateTraveltinoSettings settings,
            ITPSupport support,
            ISecretKeeper secretKeeper,
            ISerializer serializer)
            : base(settings, support, secretKeeper, serializer)
        {
        }

        public override string Source => ThirdParties.TRAVELGATETRAVELTINO;
    }
}