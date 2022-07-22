namespace iVectorOne.CSSuppliers.BedsWithEase.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class CancellationInfoResponse : SoapContent
    {
        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public Error[] Errors { get; set; } = Array.Empty<Error>();

        public CancellationInfo CancellationInfo { get; set; } = new();
    }
}
