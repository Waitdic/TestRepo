namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class GuestCount
    {
        public GuestCount() { }
        [XmlAttribute("Count")]
        public int Count { get; set; }
        [XmlAttribute("Age")]
        public int Age { get; set; }
    }
}
