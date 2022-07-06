namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class CancelPolicy : BasePolicy
    {
        [XmlAttribute("RoomIndex")]
        public string RoomIndex { get; set; } = string.Empty;

        [XmlAttribute("FromDate")]
        public string FromDate { get; set; } = string.Empty;

        [XmlAttribute("ToDate")]
        public string ToDate { get; set; } = string.Empty;

        [XmlAttribute("ChargeType")]
        public ChargeType ChargeType { get; set; }

        [XmlAttribute("CancellationCharge")]
        public decimal CancellationCharge { get; set; }
    }
}

