﻿namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class OriginDestinationDetails
    {
        [XmlElement("itineraryInfo")]
        public ItineraryInfo ItineraryInfo { get; set; } = new();
    }
}
