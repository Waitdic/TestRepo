namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Telephone
    {
        [XmlAttribute("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;
    }


}
