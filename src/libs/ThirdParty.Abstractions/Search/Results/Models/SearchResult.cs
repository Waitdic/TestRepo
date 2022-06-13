namespace ThirdParty.Results.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Search Result
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Gets or sets The source
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The arrival date
        /// </summary>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets The duration
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets The property room booking identifier
        /// </summary>
        public int PropertyRoomBookingID { get; set; }

        /// <summary>
        /// Gets or sets The property identifier
        /// </summary>
        public int PropertyID { get; set; }

        /// <summary>
        /// Gets or sets The hit percentage
        /// </summary>
        public int HitPercentage { get; set; }

        /// <summary>
        /// Gets or sets The property room type identifier
        /// </summary>
        public int PropertyRoomTypeID { get; set; }

        /// <summary>
        /// Gets or sets The room type
        /// </summary>
        public string RoomType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The room type code
        /// </summary>
        public string RoomTypeCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The supplier identifier
        /// </summary>
        public int SupplierID { get; set; }

        /// <summary>
        /// Gets or sets The currency identifier
        /// </summary>
        public int CurrencyID { get; set; }

        /// <summary>
        /// Gets or sets The meal basis identifier
        /// </summary>
        public int MealBasisID { get; set; }

        /// <summary>
        /// Gets or sets The meal basis code
        /// </summary>
        public string MealBasisCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The adults
        /// </summary>
        public int Adults { get; set; }

        /// <summary>
        /// Gets or sets The children
        /// </summary>
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets The infants
        /// </summary>
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets The total cost
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Gets or sets The sub total
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Gets or sets The discount
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Gets or sets The total
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Gets or sets The special offer
        /// </summary>
        public string SpecialOffer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The third party reference
        /// </summary>
        public string TPReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The third party key
        /// </summary>
        public string TPKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The master identifier
        /// </summary>
        public int MasterID { get; set; }

        /// <summary>
        /// Gets or sets The contract identifier
        /// </summary>
        public int ContractID { get; set; }

        /// <summary>
        /// Gets or sets The alt contract identifier
        /// </summary>
        public int AltContractID { get; set; }

        /// <summary>
        /// Gets or sets The adjustments
        /// </summary>
        public List<Adjustment> Adjustments { get; set; } = new List<Adjustment>();
    }
}