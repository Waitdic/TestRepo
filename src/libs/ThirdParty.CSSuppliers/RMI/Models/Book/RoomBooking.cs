namespace ThirdParty.CSSuppliers.RMI.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomBooking
    {
        public string RoomID { get; set; } = string.Empty;
        public string MealBasisID { get; set; } = string.Empty;
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }

        [XmlArray("Guests")]
        [XmlArrayItem("Guest")]
        public List<Guest> Guests { get; set; } = new();
    }
}
