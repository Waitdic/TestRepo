﻿namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class Rate
    {
        public Rate() { }

        [XmlElement("RateDescription")]
        public RateDescription RateDescription { get; set; } = new();

        [XmlElement("TPA_Extensions")]
        public RateExtension RateExtension { get; set; } = new();

        [XmlElement("Total")]
        public RateTotal Total { get; set; } = new();
    }
}