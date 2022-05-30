using Intuitive.Helpers.Security;
using Intuitive.Helpers.Serialization;
using ThirdParty.Constants;
using ThirdParty.Lookups;

namespace ThirdParty.CSSuppliers
{

    public class TravelgateIxpiraSearch : TravelgateSearch
    {

        public TravelgateIxpiraSearch(ITravelgateSettings settings, ITPSupport support, ISecretKeeper secretKeeper, ISerializer serializer) : base(settings, support, secretKeeper, serializer)
        {
        }

        public override string Source
        {
            get
            {
                return ThirdParties.TRAVELGATEIXPIRA;
            }
        }

    }
}