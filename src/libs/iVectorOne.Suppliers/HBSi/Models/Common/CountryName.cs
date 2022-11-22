namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class CountryName
    {
        [XmlAttribute("Code")]
        public string Code { get; set; } = string.Empty;
    }


}
