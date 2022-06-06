namespace ThirdParty.CSSuppliers.Travelgate
{
    using System.Net.Http;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class TravelgateDerbysoftIHG : Travelgate
    {
        public TravelgateDerbysoftIHG(
            ITravelgateDerbysoftIHGSettings settings,
            ITPSupport support,
            HttpClient httpClient,
            ISecretKeeper secretKeeper,
            ISerializer serializer,
            ILogger<TravelgateDerbysoftIHG> logger)
            : base(settings, support, httpClient, secretKeeper, serializer, logger)
        {
        }

        public override string Source => ThirdParties.TRAVELGATEDERBYSOFTIHG;
    }
}