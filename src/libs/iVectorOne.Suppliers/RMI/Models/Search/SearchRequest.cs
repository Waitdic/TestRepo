namespace iVectorOne.Suppliers.RMI.Models
{
    using System.Xml.Serialization;

    public class SearchRequest
    {
        public LoginDetails LoginDetails { get; set; } = new();

        [XmlElement("SearchDetails")]
        public RmiSearchDetails SearchDetails { get; set; } = new();
    }
}
