﻿namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class TicketElement
    {
        [XmlElement("ticket")]
        public Ticket Ticket { get; set; } = new();
    }
}