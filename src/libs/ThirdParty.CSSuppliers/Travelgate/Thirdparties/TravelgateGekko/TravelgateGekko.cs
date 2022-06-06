namespace ThirdParty.CSSuppliers.Travelgate
{
    using System.Net.Http;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class TravelgateGekko : Travelgate
    {
        public TravelgateGekko(
            ITravelgateGekkoSettings settings,
            ITPSupport support,
            HttpClient httpClient,
            ISecretKeeper secretKeeper,
            ISerializer serializer,
            ILogger<TravelgateGekko> logger)
            : base(settings, support, httpClient, secretKeeper, serializer, logger)
        {
        }

        public override string Source => ThirdParties.TRAVELGATEGEKKO;
    }
}