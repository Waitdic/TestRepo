﻿namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class OTA_HotelResV2ServiceResponse
    {

        [XmlElement("OTA_HotelResRS")]
        public OTA_HotelResRS OTA_HotelResRS { get; set; } = new();
    }
}