﻿namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class BookingChannel
    {
        [XmlAttribute("Type")]
        public string ChannelType { get; set; } = string.Empty;
    }
}
