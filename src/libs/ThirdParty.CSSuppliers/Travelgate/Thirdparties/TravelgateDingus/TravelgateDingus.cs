namespace ThirdParty.CSSuppliers.Travelgate
{
    using System.Net.Http;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class TravelgateDingus : Travelgate
    {
        public TravelgateDingus(
            ITravelgateDingusSettings settings,
            ITPSupport support,
            HttpClient httpClient,
            ISecretKeeper secretKeeper,
            ISerializer serializer,
            ILogger<TravelgateDingus> logger)
            : base(settings, support, httpClient, secretKeeper, serializer, logger)
        {
        }

        public override string Source => ThirdParties.TRAVELGATEDINGUS;
    }
}