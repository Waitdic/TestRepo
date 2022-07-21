namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class Credentials
    {
        [XmlAttribute("UserName")]
        public string UserName { get; set; } = string.Empty;

        [XmlAttribute("Password")]
        public string Password { get; set; } = string.Empty;
    }
}
