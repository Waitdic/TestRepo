﻿using System.Net.Http;
using Intuitive.Helpers.Security;
using Intuitive.Helpers.Serialization;
using ThirdParty.Constants;
using ThirdParty.Lookups;

namespace ThirdParty.CSSuppliers
{

    public class TravelgateDerbysoftUOR : Travelgate
    {

        public TravelgateDerbysoftUOR(ITravelgateSettings settings, ITPSupport support, HttpClient httpClient, ISecretKeeper secretKeeper, ISerializer serializer) : base(settings, support, httpClient, secretKeeper, serializer)
        {
        }

        public override string Source
        {
            get
            {
                return ThirdParties.TRAVELGATEDERBYSOFTUOR;
            }
            set
            {
            }
        }

    }
}