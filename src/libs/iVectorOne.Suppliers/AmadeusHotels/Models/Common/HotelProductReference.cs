﻿namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class HotelProductReference
    {
        [XmlElement("referenceDetails")]
        public ReferenceBase ReferenceDetails { get; set; } = new();
    }
}