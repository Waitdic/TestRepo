﻿namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class ProfileInfo
    {
        public ProfileInfo() { }

        [XmlElement("Profile")]
        public Profile Profile { get; set; } = new();
    }
}
