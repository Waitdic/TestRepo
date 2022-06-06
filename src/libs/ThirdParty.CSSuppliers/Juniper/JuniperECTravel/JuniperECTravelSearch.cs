﻿namespace ThirdParty.CSSuppliers.Juniper
{
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public class JuniperECTravelSearch : JuniperBaseSearch
    {
        public JuniperECTravelSearch(IJuniperECTravelSettings settings, ISerializer serializer)
            : base(settings, serializer)
        {
        }

        public override string Source => ThirdParties.JUNIPERECTRAVEL;
    }
}