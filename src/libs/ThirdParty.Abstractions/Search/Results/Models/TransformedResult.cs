namespace ThirdParty.Search.Results.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using ThirdParty.Models.Property.Booking;

    /// <summary>
    /// The transformed results
    /// </summary>
    [XmlType("Result")]
    public class TransformedResult
    {
        /// <summary>
        /// The master identifier error message
        /// </summary>
        [XmlIgnore]
        public const string MASTERIDERRORMESSAGE = "No Master Id Specified";

        /// <summary>
        /// The third party key error message
        /// </summary>
        [XmlIgnore]
        public const string TPKEYERRORMESSAGE = "No TPKey Specified";

        /// <summary>
        /// The currency code error message
        /// </summary>
        [XmlIgnore]
        public const string CURRENCYCODEERRORMESSAGE = "No Currency Code Specified";

        /// <summary>
        /// The room type error message
        /// </summary>
        [XmlIgnore]
        public const string ROOMTYPEERRORMESSAGE = "No Room Type Specified";

        /// <summary>
        /// The meal basis error message
        /// </summary>
        [XmlIgnore]
        public const string MEALBASISERRORMESSAGE = "No Meal Basis Specified";

        /// <summary>
        /// The amount error message
        /// </summary>
        [XmlIgnore]
        public const string AMOUNTERRORMESSAGE = "No Valid Amount Specified";

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        [XmlIgnore]
        public List<string> Warnings { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the child ages.
        /// </summary>
        [XmlIgnore]
        public List<int> ChildAges { get; set; } = new List<int>();

        /// <summary>
        /// Gets or sets the master identifier.
        /// </summary>
        [XmlAttribute("MID")]
        public int MasterID { get; set; }

        /// <summary>
        /// Gets or sets the third party key.
        /// </summary>
        [XmlAttribute("TPK")]
        public string TPKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [XmlAttribute("CC")]
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currency identifier.
        /// </summary>
        [DefaultValue(0)]
        [XmlAttribute("CID")]
        public int CurrencyID { get; set; }

        /// <summary>
        /// Gets or sets the property room booking identifier.
        /// </summary>
        [DefaultValue(0)]
        [XmlAttribute("PRBID")]
        public int PropertyRoomBookingID { get; set; }

        /// <summary>
        /// Gets or sets the room type code.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute("RTC")]
        public string RoomTypeCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the room.
        /// </summary>
        [XmlAttribute("RT")]
        public string RoomType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the meal basis code.
        /// </summary>
        [XmlAttribute("MBC")]
        public string MealBasisCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the meal basis identifier.
        /// </summary>
        [DefaultValue(0)]
        [XmlAttribute("MBID")]
        public int MealBasisID { get; set; }

        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        [DefaultValue(0)]
        [XmlAttribute("AD")]
        public int Adults { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        [DefaultValue(0)]
        [XmlAttribute("CH")]
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets the child age CSV.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute("hlpCHA")]
        public string ChildAgeCSV
        {
            get
            {
                return string.Join(",", this.ChildAges);
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the infants.
        /// </summary>
        [DefaultValue(0)]
        [XmlAttribute("INF")]
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [XmlAttribute("AMT")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the gross cost.
        /// </summary>
        /// <value>
        /// The gross cost.
        /// </value>
        [XmlAttribute("GS")]
        public decimal GrossCost { get; set; }

        /// <summary>
        /// Gets or sets the third party reference.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute("TPR")]
        public string TPReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        [XmlAttribute("DSC")]
        public decimal Discount { get; set; }

        /// <summary>
        /// Gets or sets the special offer.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute("SO")]
        public string SpecialOffer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the available rooms.
        /// </summary>
        [DefaultValue(0)]
        [XmlAttribute("AR")]
        public int AvailableRooms { get; set; }

        /// <summary>
        /// Gets or sets the commission percentage.
        /// </summary>
        [XmlAttribute("COM")]
        public decimal CommissionPercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [dynamic property].
        /// </summary>
        [XmlAttribute("DP")]
        public bool DynamicProperty { get; set; }

        /// <summary>
        /// Gets or sets the non package amount.
        /// </summary>
        [XmlAttribute("NPA")]
        public decimal NonPackageAmount { get; set; }

        /// <summary>
        /// Gets or sets the package rate basis.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute("PRB")]
        public string PackageRateBasis { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether [non refundable rates].
        /// </summary>
        [XmlIgnore]
        public bool? NonRefundableRates { get; set; }

        [XmlAttribute("NRF")]
        public bool NonRefundableRatesSerialize
        {
            get
            {
                return NonRefundableRates.GetValueOrDefault();
            }
            set
            {
            }
        }

        /// <summary>
        /// Should we serialize adjustments.
        /// </summary>
        /// <returns>a value to determine whether we are returning adjustments</returns>
        public bool ShouldSerializeNonRefundableRatesSerialize()
        {
            return NonRefundableRates.HasValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [fix price].
        /// </summary>
        [XmlAttribute("FIXPR")]
        public bool FixPrice { get; set; }

        /// <summary>
        /// Gets or sets the selling price.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute("SELLPR")]
        public string SellingPrice { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the net price.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute("NETPR")]
        public string NetPrice { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the regional tax.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute("TAXAMT")]
        public string RegionalTax { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether [free cancellation].
        /// </summary>
        [XmlAttribute("FREEC")]
        public bool FreeCanx { get; set; }

        /// <summary>
        /// Gets or sets the third party rate code.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute("TPRC")]
        public string TPRateCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the third party room details.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute("TPRD")]
        public string TPRoomDetails { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether [on request].
        /// </summary>
        [XmlAttribute("RQ")]
        public bool OnRequest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pay local available].
        /// </summary>
        [XmlAttribute("PLA")]
        public bool PayLocalAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pay local required].
        /// </summary>
        [XmlAttribute("PLR")]
        public bool PayLocalRequired { get; set; }

        /// <summary>
        /// Gets or sets the minimum price.
        /// </summary>
        [XmlAttribute("MSP")]
        public decimal MinimumPrice { get; set; }

        /// <summary>
        /// Gets or sets the adjustments.
        /// </summary>
        [XmlArray("Adjustments")]
        [XmlArrayItem("Adjustment")]
        public List<TransformedResultAdjustment> Adjustments { get; set; } = new List<TransformedResultAdjustment>();

        /// <summary>
        /// Gets or sets the cancellations.
        /// </summary>
        [XmlArray("Cs")]
        [XmlArrayItem("C")]
        public List<Cancellation> Cancellations { get; set; } = new List<Cancellation>();

        /// <summary>
        /// Should we serialize adjustments.
        /// </summary>
        /// <returns>a value to determine whether we are returning adjustments</returns>
        public bool ShouldSerializeAdjustments()
        {
            return this.Adjustments.Any();
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public void Validate()
        {
            if (this.MasterID == 0)
            {
                this.Warnings.Add(MASTERIDERRORMESSAGE);
            }

            if (string.IsNullOrEmpty(this.TPKey))
            {
                this.Warnings.Add(TPKEYERRORMESSAGE);
            }

            if (string.IsNullOrEmpty(this.CurrencyCode))
            {
                this.Warnings.Add(CURRENCYCODEERRORMESSAGE);
            }

            if (string.IsNullOrEmpty(this.RoomType))
            {
                this.Warnings.Add(ROOMTYPEERRORMESSAGE);
            }

            if (string.IsNullOrEmpty(this.MealBasisCode))
            {
                this.Warnings.Add(MEALBASISERRORMESSAGE);
            }

            if (this.Amount == 0)
            {
                this.Warnings.Add(AMOUNTERRORMESSAGE);
            }
        }
    }
}