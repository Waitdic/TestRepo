﻿namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomTypeExtension
    {
        public RoomTypeExtension() { }

        [XmlArray("Guests")]
        [XmlArrayItem("Guest")]
        public List<Guest> Guests { get; set; } = new();
    }
}