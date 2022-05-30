namespace ThirdParty.CSSuppliers.iVectorConnect.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("BasketBookRequest")]
    public class BasketBookRequest
    {
        [XmlElement("LoginDetails")]
        public LoginDetails? LoginDetails { get; set; }

        [XmlElement("LeadCustomer")]
        public LeadCustomer LeadCustomer { get; set; } = new();

        [XmlArray("GuestDetails")]
        [XmlArrayItem("GuestDetail")]
        public GuestDetail[] GuestDetails { get; set; } = Array.Empty<GuestDetail>();

        [XmlElement("PropertyBookings")]
        public PropertyBookings PropertyBookings { get; set; } = new();
    }
}
