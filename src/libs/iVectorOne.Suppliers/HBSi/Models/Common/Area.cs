﻿namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Area
    {
        public Area() { }
        [XmlAttribute("TypeCode")]
        public string TypeCode { get; set; } = string.Empty;
        [XmlAttribute("AreaCode")]
        public string AreaCode { get; set; } = string.Empty;
    }
}
