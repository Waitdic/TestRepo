﻿namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class MessageErrorInformation
    {
        [XmlElement("errorDetail")]
        public ErrorDetail ErrorDetail { get; set; } = new();
    }
}
