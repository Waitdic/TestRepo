using System;
using System.Collections.Generic;
using System.Text;

 namespace iVectorOne.Suppliers.TourPlanTransfers
{
    public class LocationData
    {
        /// <summary>
        /// Gets or sets the LocationCode.
        /// </summary>
        public string LocationCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the DepartureName.
        /// </summary>
        public string DepartureName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ArrivalName.
        /// </summary>
        public string ArrivalName { get; set; } = string.Empty;
        
        /// <summary>
        /// Validate the fields 
        /// </summary>
        public bool Validation()
        {
            if (string.IsNullOrEmpty(LocationCode))
                return false;
            return true;
        }
    }
}
