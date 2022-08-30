namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class Token
    {
        public Token() { }

        [XmlAttribute("TokenCode")]
        public string TokenCode { get; set; } = string.Empty;

        [XmlAttribute("TokenName")]
        public string TokenName { get; set; } = string.Empty;
    }
}
