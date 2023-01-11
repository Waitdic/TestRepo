using System;
using System.Collections.Generic;
using System.Text;

namespace iVectorOne.Suppliers.TourPlanTransfers.Models
{
    public class LocationData
    {
        /// <summary>
        /// Gets or sets the location code.
        /// </summary>
        public string LocationCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the departure name.
        /// </summary>
        public string DepartureName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the arrival name.
        /// </summary>
        public string ArrivalName { get; set; } = string.Empty;

        /// <summary>
        /// Validate the fields 
        /// </summary>
        public bool Validation()
        {
            return !string.IsNullOrEmpty(LocationCode)
                && !string.IsNullOrEmpty(DepartureName)
                && !string.IsNullOrEmpty(ArrivalName);
        }

        /// <summary>
        /// Check the location data valid or invalid.
        /// </summary>
        /// <param name="locationData"></param>
        /// <returns></returns>
        public bool IsLocationDataValid(string[] locationData)
        {
            return locationData.Length == 2;
        }
    }
}
