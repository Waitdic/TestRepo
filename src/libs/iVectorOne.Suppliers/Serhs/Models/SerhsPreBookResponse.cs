namespace iVectorOne.CSSuppliers.Serhs.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("response")]
    public class SerhsPreBookResponse
    {
        [XmlElement("preBooking")]
        public Booking PreBooking { get; set; } = new();

        [XmlElement("amount_details")]
        public AmountDetails AmountDetails { get; set; } = new();

        [XmlArray("extraCharges")]
        [XmlArrayItem("extraCharge")]
        public List<ExtraCharge> ExtraCharges { get; set; } = new();

        [XmlElement("remarks")]
        public List<Remark> Remarks { get; set; } = new();

        [XmlArray("cancelPolicies")]
        [XmlArrayItem("cancelPolicy")]
        public List<CancelPolicy> CancelPolicies { get; set; } = new();
    }
}
