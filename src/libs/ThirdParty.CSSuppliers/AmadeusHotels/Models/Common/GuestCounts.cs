namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class GuestCounts
    {
        [XmlAttribute("IsPerRoom")]
        public string IsPerRoom { get; set; } = string.Empty;
        public bool ShouldSerializeIsPerRoom() => !string.IsNullOrEmpty(IsPerRoom);

        public GuestCount GuestCount { get; set; } = new();
    }
}
