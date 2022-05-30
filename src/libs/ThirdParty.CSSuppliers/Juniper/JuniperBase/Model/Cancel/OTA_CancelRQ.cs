﻿namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class OTA_CancelRQ
    {
        [XmlAttribute("PrimaryLangID")]
        public string PrimaryLangId { get; set; } = string.Empty;

        [XmlElement("POS")]
        public Pos Pos { get; set; } = new();

        [XmlElement("UniqueID")]
        public UniqueId UniqueId { get; set; } = new();
    }
}
