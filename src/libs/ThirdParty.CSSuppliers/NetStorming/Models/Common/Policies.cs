﻿namespace ThirdParty.CSSuppliers.NetStorming.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Policies
    {
        [XmlElement("policy")]
        public Policy[] Policy { get; set; } = Array.Empty<Policy>();
    }
}