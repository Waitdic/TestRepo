namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class StayDateRange
    {
        public StayDateRange() { }

        [XmlAttribute("Start")]
        public string Start { get; set; } = string.Empty;

        [XmlAttribute("End")]
        public string End { get; set; } = string.Empty;
    }
}
