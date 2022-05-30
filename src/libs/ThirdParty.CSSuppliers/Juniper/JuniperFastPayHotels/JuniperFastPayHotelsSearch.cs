namespace ThirdParty.CSSuppliers
{
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    
    public class JuniperFastPayHotelsSearch : JuniperBaseSearch
    {
        public JuniperFastPayHotelsSearch(IJuniperFastPayHotelsSettings settings, ISerializer serializer, ILogger<JuniperFastPayHotelsSearch> logger)
            : base(settings, serializer, logger)
        {
        }

        public override string Source => ThirdParties.JUNIPERFASTPAYHOTELS;
    }
}