namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class ProfileInfo
    {
        [XmlElement("Profile")]
        public Profile Profile { get; set; } = new();
    }
}
