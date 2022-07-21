namespace iVectorOne.Suppliers.Restel.Models.Common
{
    using System.Xml.Serialization;

    public class Hot
    {
        [XmlElement("cod")]
        public string Cod { get; set; } = string.Empty;

        [XmlElement("afi")]
        public string Afi { get; set; } = string.Empty;

        [XmlElement("nom")]
        public string Nom { get; set; } = string.Empty;

        [XmlElement("pro")]
        public string Pro { get; set; } = string.Empty;

        [XmlElement("prn")]
        public string Prn { get; set; } = string.Empty;

        [XmlElement("pob")]
        public string Pob { get; set; } = string.Empty;

        [XmlElement("cat")]
        public string Cat { get; set; } = string.Empty;

        [XmlElement("fen")]
        public string Fen { get; set; } = string.Empty;

        [XmlElement("fsa")]
        public string Fsa { get; set; } = string.Empty;

        [XmlElement("pdr")]
        public string Pdr { get; set; } = string.Empty;

        [XmlElement("cal")]
        public string Cal { get; set; } = string.Empty;

        [XmlElement("mar")]
        public string Mar { get; set; } = string.Empty;

        [XmlElement("c60")]
        public string C60 { get; set; } = string.Empty;

        [XmlElement("res")]
        public HotRes Res { get; set; } = new();

        [XmlElement("pns")]
        public string Pns { get; set; } = string.Empty;

        [XmlElement("end")]
        public string End { get; set; } = string.Empty;

        [XmlElement("enh")]
        public string Enh { get; set; } = string.Empty;

        [XmlElement("cat2")]
        public string Cat2 { get; set; } = string.Empty;

        [XmlElement("city_tax")]
        public string CityTax { get; set; } = string.Empty;

        [XmlElement("tipo_establecimiento")]
        public string TipoEstablecimiento { get; set; } = string.Empty;
    }
}