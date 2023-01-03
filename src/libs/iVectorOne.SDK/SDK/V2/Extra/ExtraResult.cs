namespace iVectorOne.SDK.V2.ExtraSearch
{
    /// <summary>
    /// A class representing a single extra result
    /// </summary>
    public class ExtraResult
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>
        /// The booking token.
        /// </value>
        public string BookingToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the supplier reference.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string SupplierReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the tp session id.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string TPSessionID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the extra vehicle.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string ExtraVehicle { get; set; } = string.Empty;

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
        public int OutboundExtraMinutes { get; set; }

        /// <summary>
        /// Gets or sets the return extra minutes.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public int ReturnExtraMinutes { get; set; }
    }
}