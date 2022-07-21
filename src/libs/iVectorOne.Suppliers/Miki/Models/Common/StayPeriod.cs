namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class StayPeriod
    {
        [XmlElement("checkinDate")]
        public string CheckinDate { get; set; } = string.Empty;

        [XmlElement("numberOfNights")]
        public int NumberOfNights { get; set; }
    }
}
