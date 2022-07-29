namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class CreditCardInfo
    {
        [XmlElement("ccInfo")]
        public CcInfo CcInfo { get; set; } = new();
    }
}
