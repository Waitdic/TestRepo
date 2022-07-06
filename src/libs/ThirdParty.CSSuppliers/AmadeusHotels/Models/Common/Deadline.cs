namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Deadline
    {
        [XmlAttribute("AbsoluteDeadline")]
        public DateTime AbsoluteDeadline { get; set; }
    }
}
