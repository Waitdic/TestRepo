﻿namespace ThirdParty.CSSuppliers.Netstorming.Models
{
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.Netstorming.Models.Common;

    [XmlRoot("envelope")]
    public class NetstormingCancellationCostResponse : EnvelopeBase
    {
        [XmlElement("response")]
        public CostResponse Response { get; set; } = new();

        public class CostResponse
        {
            [XmlElement("policies")]
            public Policies Policies { get; set; } = new();

            [XmlElement("deadline")]
            public Deadline Deadline { get; set; } = new();
        }
    }
}