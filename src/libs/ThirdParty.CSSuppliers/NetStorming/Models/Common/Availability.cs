﻿namespace ThirdParty.CSSuppliers.NetStorming.Models.Common
{
    using System.Xml.Serialization;

    public class Availability
    {
        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;
    }
}