namespace ThirdParty.Search.Support
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;

    /// <summary>
    /// A class containing support functions for third party searches
    /// </summary>
    public class ThirdPartySearchSupport
    {
        /// <summary>
        /// gets a search details object from the passed in extra information.
        /// </summary>
        /// <param name="extraInfo">The o extra information.</param>
        /// <returns>A search details object</returns>
        public static SearchDetails SearchDetailsFromExtraInfo(object extraInfo)
        {
            var type = extraInfo.GetType();
            while (!(type == null))
            {
                if (type == typeof(SearchExtraHelper))
                {
                    SearchExtraHelper searchHelper = (SearchExtraHelper)extraInfo;
                    return searchHelper.SearchDetails;
                }
                else if (type == typeof(SearchDetails))
                {
                    return (SearchDetails)extraInfo;
                }
                else
                {
                    type = type.BaseType;
                }
            }

            return null!;
        }

        /// <summary>
        /// returns a unique request identifier.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="extraInfo">The o extra information.</param>
        /// <returns>a unique request identifier</returns>
        public static string UniqueRequestID(string source, object extraInfo)
        {
            var type = extraInfo.GetType();

            // loop until we find a search extra helper or get to the base class
            while (!(type == null))
            {
                if (type == typeof(SearchExtraHelper))
                {
                    SearchExtraHelper searchHelper = (SearchExtraHelper)extraInfo;
                    string requestId = searchHelper.UniqueRequestID;

                    // if the source is empty string then return the parent source
                    if (requestId == string.Empty)
                    {
                        requestId = source;
                    }

                    return requestId;
                }
                else
                {
                    type = type.BaseType;
                }
            }

            return source;
        }

        /// <summary>Gets the extra information for request.</summary>
        /// <param name="extraInfo">The o extra information.</param>
        /// <param name="resortSplits">The o resort splits.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="ArgumentException">The search extra info type is not recognized</exception>
        public static SearchExtraHelper GetExtraInfoForRequest(object extraInfo, List<ResortSplit> resortSplits)
        {
            var type = extraInfo.GetType();
            if (type == typeof(SearchExtraHelper) || typeof(SearchExtraHelper).IsAssignableFrom(extraInfo.GetType()))
            {
                SearchExtraHelper searchHelper = (SearchExtraHelper)extraInfo;
                searchHelper.ResortSplits = resortSplits;
                return searchHelper;
            }
            else if (type == typeof(SearchDetails))
            {
                return new SearchExtraHelper()
                {
                    SearchDetails = (SearchDetails)extraInfo,
                    ResortSplits = resortSplits
                };
            }

            throw new ArgumentException("Unrecognised search extra info type");
        }
    }
}