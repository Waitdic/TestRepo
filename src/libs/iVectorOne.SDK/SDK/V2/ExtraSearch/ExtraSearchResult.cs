namespace iVectorOne.SDK.V2.ExtraSearch
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    public class ExtraSearchResult
    {
        [XmlIgnore]
        public List<string> Warnings { get; set; } = new List<string>();

        [XmlIgnore]
        public const string TPSESSIONIDERRORMESSAGE = "No TP Session Id Specified";
        [XmlIgnore]
        public const string CURRENCYCODEERRORMESSAGE = "No Currency Code Specified"; 
        [XmlIgnore]
        public const string COSTERRORMESSAGE = "No Valid Cost Specified";


        public string BookingToken { get; set; } = string.Empty;
        public string TPSessionID { get; set; } = string.Empty;
        public string SupplierReference { get; set; } = string.Empty;
        public int CurrencyID { get; set; }
        public decimal Cost { get; set; }
        public string ExtraName { get; set; } = string.Empty;
        public string ExtraCategory { get; set; } = string.Empty;
        public string UseDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string UseTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string AdditionalDetails { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public void Validate()
        {
            if (string.IsNullOrEmpty(this.TPSessionID))
            {
                this.Warnings.Add(TPSESSIONIDERRORMESSAGE);
            }

            if (this.Cost == 0)
            {
                this.Warnings.Add(COSTERRORMESSAGE);
            }
        }
    }
}
