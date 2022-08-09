namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System.Xml.Serialization;

    public class Leader
    {
        [XmlAttribute("LeaderPersonID")]
        public int LeaderPersonID { get; set; } = 1;
    }
}
