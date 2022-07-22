namespace iVectorOne.CSSuppliers.BedsWithEase.Models.Common
{
    using System.Xml.Serialization;

    public class Location
    {
        [XmlElement("ResortCode")]
        public string ResortCode { get; set; } = string.Empty;
    }
}
