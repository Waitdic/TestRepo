namespace iVectorOne.Suppliers.iVectorConnect.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class ReturnStatus
    {
        public bool Success { get; set; }

        [XmlArray("Exceptions")]
        [XmlArrayItem("Exception")]
        public string[] Exceptions { get; set; } = Array.Empty<string>();
    }
}
