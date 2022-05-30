﻿namespace ThirdParty.CSSuppliers.NetStorming.Models
{
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.NetStorming.Models.Common;

    [XmlRoot("envelope")]
    public class NetstormingCancellationCostRequest : EnvelopeBase
    {
        [XmlElement("query")]
        public RequestQuery Query { get; set; } = new();

        public class RequestQuery
        {
            [XmlAttribute("type")]
            public string Type { get; set; } = string.Empty;

            [XmlAttribute("product")]
            public string Product { get; set; } = string.Empty;

            [XmlElement("availability")]
            public Availability Availability { get; set; } = new();

            [XmlElement("agreement")]
            public RequestAgreement Agreement { get; set; } = new();

            [XmlElement("hotel")]
            public RequestHotel Hotel { get; set; } = new();
        }
    }
}