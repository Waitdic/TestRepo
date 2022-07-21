namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class GeneralErrorInfo
    {
        [XmlElement("messageErrorText")]
        public MessageErrorText MessageErrorText { get; set; } = new();

        [XmlElement("messageErrorInformation")]
        public MessageErrorInformation MessageErrorInformation { get; set; } = new();
    }
}
