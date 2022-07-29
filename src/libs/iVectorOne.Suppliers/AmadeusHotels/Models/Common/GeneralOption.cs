namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class GeneralOption
    {
        [XmlElement("optionDetail")]
        public OptionDetail OptionDetail { get; set; } = new();
    }
}
