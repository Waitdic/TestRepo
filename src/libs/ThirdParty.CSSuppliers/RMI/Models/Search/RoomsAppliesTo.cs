﻿namespace ThirdParty.CSSuppliers.RMI.Models
{
    using System.Xml.Serialization;

    public class RoomsAppliesTo
    {
        [XmlElement("RoomRequest")]
        public int RoomRequest { get; set; }
    }
}