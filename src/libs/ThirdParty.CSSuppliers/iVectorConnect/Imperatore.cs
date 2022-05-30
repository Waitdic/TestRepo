namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using System.Net.Http;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public sealed class Imperatore : IVCBase
    {
        public Imperatore(
            IImperatoreSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<Imperatore> logger)
            : base(settings, support, serializer, httpClient, logger)
        {
        }

        public override string Source => ThirdParties.IMPERATORE;
    }
}