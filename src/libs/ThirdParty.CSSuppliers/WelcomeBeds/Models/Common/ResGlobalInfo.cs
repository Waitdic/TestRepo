﻿namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class ResGlobalInfo
    {
        public ResGlobalInfo() { }

        [XmlArray("HotelReservationIDs")]
        [XmlArrayItem("HotelReservationID")]
        public List<HotelReservationId> HotelReservationIds { get; set; } = new();

        [XmlArray("Comments")]
        [XmlArrayItem("Comment")]
        public List<Comment> Comments { get; set; } = new();
    }

    public class Comment
    {
        public Comment() { }

        [XmlElement("Text", Namespace = "http://www.opentravel.org/OTA/2003/05")]
        public string Text { get; set; } = string.Empty;
    }
}