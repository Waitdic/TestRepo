namespace ThirdParty.CSSuppliers
{
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public class JuniperECTravelSearch : JuniperBaseSearch
    {
        public JuniperECTravelSearch(IJuniperECTravelSettings settings, ISerializer serializer, ILogger<JuniperECTravelSearch> logger)
            : base(settings, serializer, logger)
        {
        }

        public override string Source => ThirdParties.JUNIPERECTRAVEL;
    }
}