﻿namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class GuestList
    {
        [XmlElement("occupantList")]
        public OccupantList OccupantList { get; set; } = new();
    }
}