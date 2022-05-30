namespace ThirdParty.CSSuppliers.NetStorming.WHL
{
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class WHLSearch : NetstormingBaseSearch
    {
        public override string Source => ThirdParties.WHL;

        public WHLSearch(
            INetstormingSettings settings,
            ITPSupport support,
            ISerializer serializer,
            ILogger<WHLSearch> logger)
            : base(settings, support, serializer, logger)
        {
        }
    }
}