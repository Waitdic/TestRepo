namespace ThirdParty.CSSuppliers
{
    using System.Net.Http;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public class JuniperECTravel : JuniperBase
    {
        public JuniperECTravel(
            IJuniperECTravelSettings settings,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<JuniperECTravel> logger)
            : base(settings, serializer, httpClient, logger)
        {
        }

        public override string Source => ThirdParties.JUNIPERECTRAVEL;
    }
}