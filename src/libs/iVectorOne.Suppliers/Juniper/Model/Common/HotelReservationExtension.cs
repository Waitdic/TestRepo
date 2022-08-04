﻿namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class HotelReservationExtension
    {
        public HotelReservationExtension() { }

        [XmlElement("PaxCountry")]
        public string PaxCountry { get; set; } = string.Empty;

        [XmlElement("ForceCurrency")]
        public string ForceCurrency { get; set; } = string.Empty;
    }
}