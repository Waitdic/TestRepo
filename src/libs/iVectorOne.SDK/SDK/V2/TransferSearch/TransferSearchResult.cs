namespace iVectorOne.SDK.V2.TransferSearch
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    public class TransferSearchResult
    {
        [XmlIgnore]
        public List<string> Warnings { get; set; } = new List<string>();

        [XmlIgnore]
        public const string TPSESSIONIDERRORMESSAGE = "No TP Session Id Specified";
        [XmlIgnore]
        public const string CURRENCYCODEERRORMESSAGE = "No Currency Code Specified";
        [XmlIgnore]
        public const string TRANSFERVEHICLEERRORMESSAGE = "No Transfer Vehicle Specified";
        [XmlIgnore]
        public const string MEALBASISERRORMESSAGE = "No Meal Basis Specified";
        [XmlIgnore]
        public const string COSTERRORMESSAGE = "No Valid Cost Specified";


        public string BookingToken { get; set; } = string.Empty;
        public string TPSessionID { get; set; } = string.Empty;
        public string SupplierReference { get; set; } = string.Empty;
        public string TransferVehicle { get; set; } = string.Empty;
        public string ReturnTime { get; set; } = string.Empty;
        public decimal VehicleCost { get; set; }
        public decimal AdultCost { get; set; }
        public decimal ChildCost { get; set; }
        public int CurrencyID { get; set; }
        public int VehicleQuantity { get; set; }
        public decimal Cost { get; set; }
        public decimal BuyingChannelCost { get; set; }
        public string OutboundInformation { get; set; } = string.Empty;
        public string ReturnInformation { get; set; } = string.Empty;
        public decimal OutboundCost { get; set; }
        public decimal ReturnCost { get; set; }
        public string OutboundXML { get; set; } = string.Empty;
        public string ReturnXML { get; set; } = string.Empty;
        public int OutboundTransferMinutes { get; set; }
        public int ReturnTransferMinutes { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(this.TPSessionID))
            {
                this.Warnings.Add(TPSESSIONIDERRORMESSAGE);
            }

            //if (string.IsNullOrEmpty(this.CurrencyCode))
            //{
            //    this.Warnings.Add(CURRENCYCODEERRORMESSAGE);
            //}

            if (string.IsNullOrEmpty(this.TransferVehicle))
            {
                this.Warnings.Add(TRANSFERVEHICLEERRORMESSAGE);
            }

            if (this.Cost == 0)
            {
                this.Warnings.Add(COSTERRORMESSAGE);
            }
        }
    }
}
