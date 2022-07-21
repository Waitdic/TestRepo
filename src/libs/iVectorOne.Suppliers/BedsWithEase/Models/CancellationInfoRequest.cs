namespace iVectorOne.Suppliers.BedsWithEase.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class CancellationInfoRequest : SoapContent
    {
        public string SessionId { get; set; } = string.Empty;

        public string OperatorCode { get; set; } = string.Empty;

        [XmlArray("BookCodes")]
        [XmlArrayItem("BookCode")]
        public string[] BookCodes { get; set; } = Array.Empty<string>();

        public string BookingReference { get; set; } = string.Empty;
    }
}
