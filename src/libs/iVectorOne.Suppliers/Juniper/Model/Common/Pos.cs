﻿namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class Pos
    {
        [XmlElement("Source")]
        public PosSource Source { get; set; } = new();
    }
}