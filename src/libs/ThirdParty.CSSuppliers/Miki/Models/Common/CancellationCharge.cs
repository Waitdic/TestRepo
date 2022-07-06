namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class CancellationCharge
    {
        [XmlElement("percentage")]
        public decimal Percentage { get; set; }
    }
}
