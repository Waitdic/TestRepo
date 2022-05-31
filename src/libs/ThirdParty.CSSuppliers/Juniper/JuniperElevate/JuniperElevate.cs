namespace ThirdParty.CSSuppliers.Juniper
{
    using System.Net.Http;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public class JuniperElevate : JuniperBase
    {
        public JuniperElevate(
            IJuniperElevateSettings settings,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<JuniperElevate> logger)
            : base(settings, serializer, httpClient, logger)
        {
        }

        public override string Source => ThirdParties.JUNIPERELEVATE;
    }
}