namespace iVectorOne.CSSuppliers.Hotelston.Models.Common
{
    using System.Xml.Serialization;

    public class BoardType
    {
        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;
    }
}