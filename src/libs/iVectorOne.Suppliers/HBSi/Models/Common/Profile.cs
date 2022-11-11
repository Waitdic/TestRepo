namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Profile
    {
        [XmlAttribute("ProfileType")]
        public int ProfileType { get; set; }
        public bool ShouldSerializeProfileType() => ProfileType > 0;

        [XmlElement("Customer")]
        public Customer Customer { get; set; } = new();
    }
}
