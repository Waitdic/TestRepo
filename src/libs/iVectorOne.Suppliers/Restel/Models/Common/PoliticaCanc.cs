namespace iVectorOne.Suppliers.Restel.Models.Common
{
    using System.Xml.Serialization;

    public class PoliticaCanc
    {
        [XmlElement("noches_gasto")]
        public string NochesGasto { get; set; } = string.Empty;

        [XmlElement("estCom_gasto")]
        public string EstComGasto { get; set; } = string.Empty;

        [XmlElement("dias_antelacion")]
        public string DiasAntelacion { get; set; } = string.Empty;
    }
}
