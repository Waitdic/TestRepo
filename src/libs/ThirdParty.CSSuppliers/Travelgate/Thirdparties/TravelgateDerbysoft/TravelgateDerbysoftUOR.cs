namespace ThirdParty.CSSuppliers.Travelgate
{
    using System.Net.Http;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class TravelgateDerbysoftUOR : Travelgate
    {
        public TravelgateDerbysoftUOR(
            ITravelgateDerbysoftUORSettings settings,
            ITPSupport support,
            HttpClient httpClient,
            ISecretKeeper secretKeeper,
            ISerializer serializer,
            ILogger<TravelgateDerbysoftUOR> logger)
            : base(settings, support, httpClient, secretKeeper, serializer, logger)
        {
        }

        public override string Source => ThirdParties.TRAVELGATEDERBYSOFTUOR;
    }
}