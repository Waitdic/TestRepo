namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Soap
{
    using System.Xml.Serialization;
    using Header;

    [XmlRoot(ElementName = "Envelope")]
    public class EnvelopeResponse<T> where T : SoapContent, new()
    {
        public SoapHeaderResponse Header { get; set; } = new();

        public SoapBody Body { get; set; } = new();

        public class SoapBody
        {
            [XmlElement(typeof(OTAHotelAvailRS), ElementName = "OTA_HotelAvailRS")]
            [XmlElement(typeof(HotelSellReply), ElementName = "Hotel_SellReply")]
            [XmlElement(typeof(Fault), ElementName = "Fault")]
            [XmlElement(typeof(PNRReply), ElementName = "PNR_Reply")]
            public SoapContent SoapContent { get; set; } = new T();

            [XmlIgnore]
            public T Content
            {
                get => (T)SoapContent;
                set => SoapContent = value;
            }
        }
    }
}
