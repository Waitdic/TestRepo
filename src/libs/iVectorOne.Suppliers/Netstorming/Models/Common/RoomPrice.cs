namespace iVectorOne.CSSuppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class RoomPrice
    {
        [XmlAttribute("nett")]
        public string Nett { get; set; } = string.Empty;

        [XmlAttribute("gross")]
        public string Gross { get; set; } = string.Empty;
    }
}