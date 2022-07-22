namespace iVectorOne.CSSuppliers.BedsWithEase.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RateWithBoard
    {
        public string BookCode { get; set; } = string.Empty;
        public string BookCodeExpireDateTime { get; set; } = string.Empty;
        public BoardType BoardType { get; set; } = new();
        public string Contract { get; set; } = string.Empty;
        public string ContractClassification { get; set; } = string.Empty;
        public ContractTolerance ContractTolerance { get; set; } = new();
        public string TotalPrice { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;

        [XmlArray("NightPrices")]
        [XmlArrayItem("NightPrice")]
        public List<NightPrice> NightPrices { get; set; } = new();

        [XmlArray("TaxesAndFees")]
        [XmlArrayItem("TaxAndFee")]
        public List<TaxAndFee> TaxesAndFees { get; set; } = new();

        [XmlArray("PaidLocallyFees")]
        [XmlArrayItem("Fee")]
        public List<Fee> PaidLocallyFees { get; set; } = new();

        [XmlArray("AdditionalServices")]
        [XmlArrayItem("AdditionalService")]
        public List<AdditionalService> AdditionalServices { get; set; } = new();

        [XmlArray("Offers")]
        [XmlArrayItem("Offer")]
        public List<Offer> Offers { get; set; } = new();
        public PriceWithFlight PriceWithFlight { get; set; } = new();

        [XmlArray("HlRvRates")]
        [XmlArrayItem("HlRvRate")]
        public List<HlRvRate> HlRvRates { get; set; } = new();

        public OnRequest OnRequest { get; set; } = new();

        public CancelInfoAvail CancelInfoAvail { get; set; } = new();

        public PackageOnly PackageOnly { get; set; } = new();

        public NonRefundable NonRefundable { get; set; } = new();

        public MarketCountries MarketCountries { get; set; } = new();

        public DataSource DataSource { get; set; }


    }

    public enum DataSource
    {
        GDS,
        VDWS,
        LCWS,
        XML,
        CONTRACT,
        DOWNLOAD,
        USER

    }
}
