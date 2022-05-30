namespace ThirdParty.CSSuppliers.Restel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class HotRes
    {
        [XmlElement("pax")]
        public Pax[] Pax { get; set; } = Array.Empty<Pax>();
    }
}