﻿using Intuitive.Helpers.Security;
using Intuitive.Helpers.Serialization;
using ThirdParty.Constants;
using ThirdParty.Lookups;

namespace ThirdParty.CSSuppliers
{

    public class TravelgateTraveltinoSearch : TravelgateSearch
    {

        public TravelgateTraveltinoSearch(ITravelgateSettings settings, ITPSupport support, ISecretKeeper secretKeeper, ISerializer serializer) : base(settings, support, secretKeeper, serializer)
        {
        }

        public override string Source
        {
            get
            {
                return ThirdParties.TRAVELGATETRAVELTINO;
            }
        }
    }
}