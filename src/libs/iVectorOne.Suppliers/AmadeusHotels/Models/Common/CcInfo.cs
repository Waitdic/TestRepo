namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class CcInfo
    {
        [XmlElement("vendorCode")]
        public string VendorCode { get; set; } = string.Empty;

        [XmlElement("cardNumber")]
        public string CardNumber { get; set; } = string.Empty;

        [XmlElement("securityId")]
        public string SecurityId { get; set; } = string.Empty;

        [XmlElement("expiryDate")]
        public string ExpiryDate { get; set; } = string.Empty;

        [XmlElement("ccHolderName")]
        public string CcHolderName { get; set; } = string.Empty;

        [XmlElement("surname")]
        public string Surname { get; set; } = string.Empty;
        public bool ShouldSerializeSurname() => Surname != string.Empty;

        [XmlElement("firstName")]
        public string FirstName { get; set; } = string.Empty;
        public bool ShouldSerializeFirstName() => FirstName != string.Empty;
    } 
} 
