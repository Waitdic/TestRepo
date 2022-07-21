namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class CancelInfoRs
    {
        public CancelInfoRs() { }

        [XmlElement("UniqueID")]
        public UniqueId UniqueId { get; set; } = new();
    }
}
