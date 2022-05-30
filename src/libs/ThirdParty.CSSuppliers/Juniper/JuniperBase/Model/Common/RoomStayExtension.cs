﻿namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class RoomStayExtension
    {
        public RoomStayExtension() { }

        [XmlElement("ExpectedPriceRange")]
        public ExpectedPriceRange ExpectedPriceRange { get; set; } = new();

        [XmlElement("PaxCountry")]
        public string PaxCountry { get; set; } = string.Empty;
    }
}
