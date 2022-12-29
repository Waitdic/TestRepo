using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace iVectorOne.Search.Results.Models.Extra
{
    /// <summary>
    /// The transformed extra results
    /// </summary>
    [XmlType("Result")]
    public class TransformedExtraResult
    {
        /// <summary>
        /// The currency code error message
        /// </summary>
        [XmlIgnore]
        public const string CURRENCYCODEERRORMESSAGE = "No Currency Code Specified";

        /// <summary>
        /// The cost error message
        /// </summary>
        [XmlIgnore]
        public const string COSTERRORMESSAGE = "No Valid Cost Specified";

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        [XmlIgnore]
        public List<string> Warnings { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the tp session id.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string TPSessionID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the supplier reference.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string SupplierReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the extra vehicle.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string ExtraVehicle { get; set; } = string.Empty; //TODO confirm: Is this ExtraVehicle correct ? Should it be TransferVehicle Only ?

        /// <summary>
        /// Gets or sets the return time.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string ReturnTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the vehicle cost.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public decimal VehicleCost { get; set; }

        /// <summary>
        /// Gets or sets the adult cost.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public decimal AdultCost { get; set; }

        /// <summary>
        /// Gets or sets the child cost.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public decimal ChildCost { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the vehicle quantity.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public int VehicleQuantity { get; set; }

        /// <summary>
        /// Gets or sets the cost.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the buying channel cost.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public decimal BuyingChannelCost { get; set; }

        /// <summary>
        /// Gets or sets the outbound information.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string OutboundInformation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the return information.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string ReturnInformation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the outbound cost.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public decimal OutboundCost { get; set; }

        /// <summary>
        /// Gets or sets the return cost.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public decimal ReturnCost { get; set; }

        /// <summary>
        /// Gets or sets the outbound xml.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string OutboundXML { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the return xml.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string ReturnXML { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the outbound extra minutes
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public int OutboundExtraMinutes { get; set; } //TODO confirm: Is this OutboundExtraMinutes correct ? Should it be OutboundTransferMinutes Only ?

        /// <summary>
        /// Gets or sets the return extra minutes.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public int ReturnExtraMinutes { get; set; } //TODO confirm: Is this ReturnExtraMinutes correct ? Should it be ReturnTransferMinutes Only ?

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrEmpty(this.CurrencyCode))
            {
                this.Warnings.Add(CURRENCYCODEERRORMESSAGE);
            }

            if (this.Cost == 0)
            {
                this.Warnings.Add(COSTERRORMESSAGE);
            }
        }
    }
}
