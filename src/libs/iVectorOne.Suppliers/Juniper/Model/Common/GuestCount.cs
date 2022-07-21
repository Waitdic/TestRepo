namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class GuestCount
    {
        public GuestCount() { }

        [XmlAttribute("Age")]
        public int Age { get; set; }

        [XmlAttribute("Count")]
        public int Count { get; set; }
    }
}