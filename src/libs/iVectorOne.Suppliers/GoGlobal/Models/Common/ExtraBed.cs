using System.Xml.Serialization;

namespace iVectorOne.Suppliers.GoGlobal.Models
{
    public class ExtraBed
    {
        [XmlAttribute("PersonID")]
        public string PersonID { get; set; } = string.Empty;

        [XmlAttribute("FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [XmlAttribute("LastName")]
        public string LastName { get; set; } = string.Empty;

        [XmlAttribute("ChildAge")]
        public int ChildAge { get; set; }
    }
}
