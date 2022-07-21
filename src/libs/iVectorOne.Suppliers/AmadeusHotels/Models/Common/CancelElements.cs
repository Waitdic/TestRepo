namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class CancelElements
    {
        [XmlElement("entryType")]
        public string EntryType { get; set; } = string.Empty;
    }
}
