namespace ThirdParty.CSSuppliers
{
    using System.Net.Http;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public class JuniperFastPayHotels : JuniperBase
    {
        public JuniperFastPayHotels(
            IJuniperFastPayHotelsSettings settings,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<JuniperFastPayHotels> logger)
            : base(settings, serializer, httpClient, logger)
        {
        }

        public override string Source => ThirdParties.JUNIPERFASTPAYHOTELS;
    }
}