namespace iVectorOne.Suppliers.Restel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Hotel
    {
        [XmlArray("observaciones")]
        [XmlArrayItem("observacion")]
        public Observacion[] Observaciones { get; set; } = Array.Empty<Observacion>();
    }
}