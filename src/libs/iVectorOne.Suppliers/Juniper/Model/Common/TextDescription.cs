﻿namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class TextDescription
    {
        [XmlElement("Text")]
        public string Text { get; set; } = string.Empty;
    }
}