namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class CancelInfoRS
    {
        [XmlElement("UniqueID")]
        public UniqueId UniqueId { get; set; } = new();
    }
}
