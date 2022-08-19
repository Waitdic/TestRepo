namespace iVectorOne.Suppliers.JonView
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomRecord
    {
        [XmlElement("prodcode")]
        public string ProdCode { get; set; } = string.Empty;

        [XmlElement("status")]
        public string Status { get; set; } = string.Empty;

        [XmlElement("productname")]
        public string ProductName { get; set; } = string.Empty;

        [XmlElement("currencycode")]
        public string CurrencyCode { get; set; } = string.Empty;

        [XmlElement("dayprice")]
        public string DayPrice { get; set; } = string.Empty;

        [XmlElement("suppliercode")]
        public string SupplierCode { get; set; } = string.Empty;

        [XmlElement("productnamedetails")]
        public ProductDetails ProductDetails { get; set; } = new ProductDetails();

        [XmlArray("cancellationpolicy")]
        [XmlArrayItem("canpolicyitem")]
        public List<CancelicyItem> CancellationPolicy { get; set; } = new();
    }


    public class ProductDetails
    {
        [XmlElement("board")]
        public string Board { get; set; } = string.Empty;

        [XmlElement("roomtype")]
        public string RoomType { get; set; } = string.Empty;
    }
}