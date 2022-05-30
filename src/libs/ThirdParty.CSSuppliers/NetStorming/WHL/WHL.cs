namespace ThirdParty.CSSuppliers.NetStorming.WHL
{
    using System.Net.Http;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class WHL : NetstormingBase
    {
        public override string Source { get; set; } = ThirdParties.WHL;

        public WHL(
            INetstormingSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<WHL> logger)
            : base(settings, support, serializer, httpClient, logger)
        {
        }
    }
}