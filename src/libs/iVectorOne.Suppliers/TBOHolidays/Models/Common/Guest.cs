namespace iVectorOne.CSSuppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class Guest
    {
        [XmlAttribute("LeadGuest")]
        public bool LeadGuest { get; set; }

        [XmlAttribute("GuestType")]
        public GuestType GuestType { get; set; }

        [XmlAttribute("GuestInRoom")]
        public int GuestInRoom { get; set; }

        public string Title { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int? Age { get; set; }
        public bool ShouldSerializeAge() => Age != null;
    }
}
