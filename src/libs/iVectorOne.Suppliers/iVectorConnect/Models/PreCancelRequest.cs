namespace iVectorOne.Suppliers.iVectorConnect.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("PreCancelRequest")]
    public class PreCancelRequest
    {
        public LoginDetails? LoginDetails { get; set; }

        public string BookingReference { get; set; } = string.Empty;
    }
}