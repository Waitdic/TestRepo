namespace iVectorOne.CSSuppliers.iVectorConnect.Models
{
    using System.Collections.Generic;
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
        public List<GuestDetail> GuestDetails { get; set; } = new();

        [XmlElement("PropertyBookings")]
        public PropertyBookings PropertyBookings { get; set; } = new();

        public string ExternalReference { get; set; } = string.Empty;
    }
}