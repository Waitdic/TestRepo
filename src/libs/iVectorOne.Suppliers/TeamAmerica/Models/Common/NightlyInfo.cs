namespace iVectorOne.Suppliers.TeamAmerica.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class NightlyInfo
    {
        [XmlElement("Status")]
        public string Status { get; set; } = string.Empty;

        [XmlElement("Prices")]
        public List<Price> Prices { get; set; } = new();
    }
}
