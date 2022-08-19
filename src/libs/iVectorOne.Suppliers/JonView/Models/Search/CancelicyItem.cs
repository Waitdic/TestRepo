namespace iVectorOne.Suppliers.JonView
{
    using System.Xml.Serialization;

    public class CancelicyItem 
    {
        [XmlElement("caneffectivedate")]
        public string EffectiveDate { get; set; } = string.Empty;

        [XmlElement("canexpirydate")]
        public string ExpiryDate { get; set; } = string.Empty;

        [XmlElement("fromdays")]
        public string FromDays { get; set; } = string.Empty;

        [XmlElement("todays")]
        public string ToDays { get; set; } = string.Empty;

        [XmlElement("chargetype")]
        public string ChargeType { get; set; } = string.Empty;

        [XmlElement("ratetype")]
        public string RateType { get; set; } = string.Empty;

        [XmlElement("canrate")]
        public string Canrate { get; set; } = string.Empty;

        [XmlElement("cannote")]
        public string CanNote { get; set; } = string.Empty;
    }

    public static class ChargeType 
    {
        public const string PerPerson = "P";
        public const string EntireItem = "EI";
        public const string Daily = "DAILY";
        public const string NotAvailable = "NA";
    }

    public static class RateType 
    {
        public const string Percentage = "P";
        public const string DollarAmount = "D";
    }
}