﻿namespace ThirdParty.CSSuppliers.NetStorming.Models.Common
{
    using System.Xml.Serialization;

    public class Nights
    {
        [XmlAttribute("number")]
        public string Number { get; set; } = string.Empty;
    }
}