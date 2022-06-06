namespace ThirdParty.CSSuppliers.Juniper
{
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    
    public class JuniperFastPayHotelsSearch : JuniperBaseSearch
    {
        public JuniperFastPayHotelsSearch(IJuniperFastPayHotelsSettings settings, ISerializer serializer)
            : base(settings, serializer)
        {
        }

        public override string Source => ThirdParties.JUNIPERFASTPAYHOTELS;
    }
}