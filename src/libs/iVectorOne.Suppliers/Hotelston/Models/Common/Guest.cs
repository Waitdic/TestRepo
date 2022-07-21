namespace iVectorOne.Suppliers.Hotelston.Models.Common
{
    using System.Xml.Serialization;

    public class Guest
    {
        [XmlAttribute("title")]
        public string Title { get; set; } = string.Empty;

        public bool ShouldSerializeTitle() => !string.IsNullOrEmpty(Title);

        [XmlAttribute("firstname")]
        public string Firstname { get; set; } = string.Empty;

        [XmlAttribute("lastname")]
        public string Lastname { get; set; } = string.Empty;

        [XmlAttribute("age")]
        public int Age { get; set; }

        public bool ShouldSerializeAge() => Age != 0;
    }
}