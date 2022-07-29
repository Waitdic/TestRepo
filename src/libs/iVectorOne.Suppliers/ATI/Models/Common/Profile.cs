namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class Profile
    {
        [XmlAttribute("RPH")]
        public int RPH { get; set; }

        public Customer Customer { get; set; } = new();
    }
}
