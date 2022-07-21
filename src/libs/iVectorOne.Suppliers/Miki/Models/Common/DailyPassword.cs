namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class DailyPassword
    {
        [XmlElement("sendDate")]
        public string SendDate { get; set; } = string.Empty;

        [XmlElement("assword")]
        public string Password { get; set; } = string.Empty;
    }
}
