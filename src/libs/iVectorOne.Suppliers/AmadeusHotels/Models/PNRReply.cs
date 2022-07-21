namespace ThirdParty.CSSuppliers.AmadeusHotels.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;
    using Soap;

    public class PNRReply : SoapContent
    {
        [XmlElement("generalErrorInfo")]
        public GeneralErrorInfo[] GeneralErrorInfo { get; set; } = Array.Empty<GeneralErrorInfo>();

        [XmlElement("travellerInfo")]
        public TravellerInfo TravellerInfo { get; set; } = new();

        [XmlElement("pnrHeader")]
        public PnrHeader PnrHeader { get; set; } = new();

        [XmlElement("originDestinationDetails")]
        public OriginDestinationDetails OriginDestinationDetails { get; set; } = new();
    }
}
