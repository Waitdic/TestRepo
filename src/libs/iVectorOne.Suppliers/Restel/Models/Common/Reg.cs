namespace iVectorOne.CSSuppliers.Restel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Reg
    {
        [XmlElement("lin")]
        public string[] Lin { get; set; } = Array.Empty<string>();

        [XmlAttribute("cod")]
        public string Cod { get; set; } = string.Empty;

        [XmlAttribute("prr")]
        public decimal Prr { get; set; }

        [XmlAttribute("div")]
        public string Div { get; set; } = string.Empty;

        [XmlAttribute("esr")]
        public string Esr { get; set; } = string.Empty;

        [XmlAttribute("nr")]
        public string Nr { get; set; } = string.Empty;
    }
}
