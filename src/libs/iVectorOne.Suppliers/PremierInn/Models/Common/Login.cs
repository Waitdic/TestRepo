namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System.Xml.Serialization;

    public class Login
    {
        [XmlAttribute]
        public string UserName { get; set; }

        [XmlAttribute]
        public string Password { get; set; }
    }
}
