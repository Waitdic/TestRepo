using System.Net.Http;
using Intuitive.Helpers.Security;
using Intuitive.Helpers.Serialization;
using Microsoft.Extensions.Logging;
using ThirdParty.Constants;
using ThirdParty.Lookups;

namespace ThirdParty.CSSuppliers
{

    public class TravelgateYalago : Travelgate
    {
        public TravelgateYalago(ITravelgateSettings settings, ITPSupport support, HttpClient httpClient, ISecretKeeper secretKeeper, ISerializer serializer, ILogger<TravelgateYalago> logger) : base(settings, support, httpClient, secretKeeper, serializer, logger)
        {
        }


        public override string Source
        {
            get
            {
                return ThirdParties.TRAVELGATEYALAGO;
            }
            set
            {
            }
        }
    }
}