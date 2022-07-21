namespace iVectorOne.Suppliers.AbreuV2.Models
{
    using System.Xml.Serialization;

    public class OTA_CancelRQ : SoapContent
    {
        [XmlAttribute("Transaction")]
        public string Transaction { get; set; } = string.Empty;

        public UniqueID UniqueID { get; set; } = new();
    }
}
