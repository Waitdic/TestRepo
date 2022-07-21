namespace iVectorOne.Suppliers.BedsWithEase.Models.Common
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class ReservedGroup
    {
        [XmlArray("BookCodes")]
        [XmlArrayItem("BookCode")]
        public string[] BookCodes { get; set; } = Array.Empty<string>();

        [XmlArray("Passengers")]
        [XmlArrayItem("Passenger")]
        public List<Passenger> Passengers { get; set; } = new();

        public ErrataAccepted ErrataAccepted { get; set; } = new();

        public bool ShouldSerializeErrataAccepted()
            => ErrataAccepted.Accepted.Value == "true";
    }
}