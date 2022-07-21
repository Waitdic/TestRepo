namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class ItineraryInfo
    {
        [XmlElement("generalOption")] 
        public GeneralOption[] GeneralOption { get; set; } = Array.Empty<GeneralOption>();
    }
}
