﻿namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class PassengerData
    {
        [XmlElement("travellerInformation")]
        public TravellerInformation TravellerInformation { get; set; } = new();
    }
}
