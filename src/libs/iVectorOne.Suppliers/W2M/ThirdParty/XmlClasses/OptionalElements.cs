﻿using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "OptionalElements")]
    public class OptionalElements
    {
        [XmlElement(ElementName = "Comments")]
        public Comments Comments { get; set; }
    }
}
