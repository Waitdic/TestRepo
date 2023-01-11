namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System.Xml.Serialization;
    using System;

    public class StatusWarningFlags
    {
        [XmlElement("Text")]
        public string[] Text { get; set; } = Array.Empty<string>();
    }
}
