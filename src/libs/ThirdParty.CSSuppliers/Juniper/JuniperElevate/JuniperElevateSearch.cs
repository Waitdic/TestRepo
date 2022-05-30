namespace ThirdParty.CSSuppliers
{
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public class JuniperElevateSearch : JuniperBaseSearch
    {
        public JuniperElevateSearch(IJuniperElevateSettings settings, ISerializer serializer, ILogger<JuniperElevateSearch> logger)
            : base(settings, serializer, logger)
        {
        }

        public override string Source => ThirdParties.JUNIPERELEVATE;
    }
}