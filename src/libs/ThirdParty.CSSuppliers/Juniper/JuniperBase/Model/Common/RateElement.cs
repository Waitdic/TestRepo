﻿using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    public class RateElement
    {
        public RateElement() { }

        [XmlAttribute("Class")]
        public string ClassAttr { get; set; } = string.Empty;

        [XmlAttribute("Type")]
        public string TypeAttr { get; set; } = string.Empty;

        [XmlAttribute("Price")]
        public string Price { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Description")]
        public string Description { get; set; } = string.Empty;
    }
}
