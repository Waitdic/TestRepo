﻿namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Token
    {
        public Token() { }
        [XmlAttribute("TokenName")]
        public string TokenName { get; set; } = string.Empty;
        [XmlAttribute("TokenCode")]
        public string TokenCode { get; set; } = string.Empty;
    }
}
