﻿namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class CancelledTour
    {
        [XmlElement("status")]
        public string Status { get; set; } = string.Empty;

        [XmlElement("cancellationReference")]
        public string CancellationReference { get; set; } = string.Empty;
    }
}