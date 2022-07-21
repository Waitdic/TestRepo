namespace iVectorOne.CSSuppliers.RMI.Models
{
    using System.Xml.Serialization;

    [XmlRoot("BookRequest")]
    public class BookRequest
    {
        public LoginDetails LoginDetails { get; set; } = new();

        public BookDetails BookDetails { get; set; } = new();
    }
}
