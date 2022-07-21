namespace ThirdParty.CSSuppliers.RMI.Models
{
    using System.Xml.Serialization;

    public class Guest
    {
        [XmlElement("Type")]
        public string GuestType { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool ShouldSerializeTitle() => !string.IsNullOrEmpty(Title);
        public int Age { get; set; }
        public bool ShouldSerializeAge() => string.Equals(GuestType, "Child");
        public string Nationality { get; set; } = string.Empty;
    }
}
