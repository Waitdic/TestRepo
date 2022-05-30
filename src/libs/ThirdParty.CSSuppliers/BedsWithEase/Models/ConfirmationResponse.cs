namespace ThirdParty.CSSuppliers.BedsWithEase.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class ConfirmationResponse : SoapContent
    {
        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public Error[] Errors { get; set; } = Array.Empty<Error>();

        [XmlArray("Confirmations")]
        [XmlArrayItem("Confirmation")]
        public Confirmation[] Confirmations { get; set; } = Array.Empty<Confirmation>();
    }
}
