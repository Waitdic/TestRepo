using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<string> DepartureName { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the arrival name.
        /// </summary>
        public List<string> ArrivalName { get; set; } = new List<string>();

        /// <summary>
        /// Validate the fields 
        /// </summary>
        public bool Validation()
        {
            return !string.IsNullOrEmpty(LocationCode)
                && DepartureName.All(x => !string.IsNullOrEmpty(x))
                && ArrivalName.All(x => !string.IsNullOrEmpty(x));
        }

        /// <summary>
        /// Check the location data valid or invalid.
        /// </summary>
        /// <param name="locationData"></param>
        /// <returns></returns>
        public static bool IsLocationDataValid(List<string[]> locationData)
        {
            return locationData.All(x => x.Length == 2);
        }

        public static bool IsLocationDataCodeValid(string firstLocationCode, string secondLocationCode)
        {
            return firstLocationCode.Equals(secondLocationCode);
        }
    }
}
