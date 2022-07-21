namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class Passenger
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; } = string.Empty;
    }
}
