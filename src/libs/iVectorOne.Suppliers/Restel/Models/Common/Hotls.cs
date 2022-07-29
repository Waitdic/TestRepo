namespace iVectorOne.Suppliers.Restel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Hotls
    {
        [XmlElement("hot")]
        public Hot[] Hot { get; set; } = Array.Empty<Hot>();

        [XmlAttribute("num")]
        public int Num { get; set; }
    }
}