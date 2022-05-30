namespace ThirdParty.Results
{
    using System.Collections.Generic;
    using System.Linq;
    using iVector.Search.Property;
    using ThirdParty.Search.Models;

    /// <summary>
    ///   <para>Filters property results</para>
    /// </summary>
    public class Filter : IFilter
    {
        /// <summary>Processes the results.</summary>
        /// <param name="results">The results.</param>
        /// <param name="resortSplits">The resort splits.</param>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public List<PropertySearchResult> ProcessResults(List<PropertySearchResult> results, IEnumerable<IResortSplit> resortSplits, SearchDetails searchDetails)
        {
            var filteredResults = new List<PropertySearchResult>();
            foreach (var propertyResult in results)
            {
                if (propertyResult.RoomResults.Select(x => x.RoomData.PropertyRoomBookingID).Distinct().Count() != searchDetails.RoomDetails.Count())
                {
                    continue;
                }

                var resorts = resortSplits.Where(rs => rs.ThirdPartySupplier == propertyResult.PropertyData.Source);
                Hotel hotel = null!;

                if (!resorts.Any())
                {
                    continue;
                }

                foreach (var resort in resorts)
                {
                    hotel = resort.Hotels.FirstOrDefault(h => h.TPKey == propertyResult.PropertyData.TPKey);
                    if (hotel != null)
                    {
                        break;
                    }
                }

                if (hotel != null)
                {
                    propertyResult.PropertyData.PropertyID = hotel.PropertyID;
                    propertyResult.PropertyData.PropertyName = hotel.PropertyName;

                    foreach (var room in propertyResult.RoomResults)
                    {
                        room.RoomData.PropertyID = propertyResult.PropertyData.PropertyID;
                    }

                    propertyResult.PropertyData.PropertyReferenceID = hotel.PropertyReferenceID;

                    filteredResults.Add(propertyResult);
                }
            }

            return filteredResults;
        }
    }
}