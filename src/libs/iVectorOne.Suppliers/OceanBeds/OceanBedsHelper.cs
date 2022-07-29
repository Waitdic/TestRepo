namespace iVectorOne.Suppliers.OceanBeds
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Intuitive.Helpers.Extensions;
    using Models.Common;
    using iVectorOne;
    using iVectorOne.Suppliers.OceanBeds.Models;

    public static class OceanBedsHelper
    {
        private const string DateFormat = "yyyy-MMM-dd";
        private static readonly CultureInfo Culture = new("en-GB");

        public static string ToDateString(this DateTime date)
        {
            return date.ToString(DateFormat, Culture);
        }

        public static Credential Credentials(IThirdPartyAttributeSearch thirdPartyAttributeSearch, IOceanBedsSettings settings)
        {
            return new Credential
            {
                User = settings.User(thirdPartyAttributeSearch),
                Password = settings.Password(thirdPartyAttributeSearch)
            };
        }

        public static PropertyAvailabilityRQ BuildPropertyAvailabilityRequest(OceanBedsPropertyDetails propertyDetails,
            IThirdPartyAttributeSearch searchDetails, IOceanBedsSettings settings)
        {
            const string lowCaseTrue = "true";
            const string lowCaseFalse = "false";

            int stateId = 0;
            int cityId = 0;
            string region = string.Empty;
            string regionSearch = lowCaseFalse;
            string hotelSearch = lowCaseFalse;
            string genericSearch = lowCaseFalse;
            string options = lowCaseTrue;

            switch (propertyDetails.PropertyType)
            {
                case "i":
                    // Individual Home searches need a region
                    region = propertyDetails.Region;
                    regionSearch = lowCaseTrue;
                    options = lowCaseFalse;
                    break;
                case "g":
                    // City and State needed for generic searches
                    stateId = propertyDetails.StateId.ToSafeInt();
                    cityId = propertyDetails.CityId.ToSafeInt();
                    genericSearch = lowCaseTrue;
                    break;
                default:
                    // City and State needed for hotel searches
                    stateId = propertyDetails.StateId.ToSafeInt();
                    cityId = propertyDetails.CityId.ToSafeInt();
                    hotelSearch = lowCaseTrue;
                    break;
            }

            var request = new PropertyAvailabilityRQ
            {
                Credential = Credentials(searchDetails, settings),
                CheckInDate = propertyDetails.ArrivalDate,
                CheckOutDate = propertyDetails.DepartureDate,
                PropertyCode = propertyDetails.PropertyCode,
                StateId = stateId,
                CityId = cityId,
                Region = region,
                CommunityId = 0,
                BedRoom = 0,
                Bathroom = 0,
                IsAvailable = lowCaseTrue,
                HotelSearch = hotelSearch,
                GenericSearch = genericSearch,
                VillaSearch = regionSearch,
                Filters =
                {
                    IsGamesRoom = options,
                    IsSpaPool = options,
                    IsPrivatePool = options,
                    IsOnGolfCourse = options,
                    IsGatedCommunity = options,
                    IsPoolSouthFacing = options,
                    IsInternetAccess = options,
                    IsConservationView = options,
                    IsTheatre = options,
                    AirportId = 0
                },
                RoomList = propertyDetails.RoomDetails
                    .Select(roomDetail => new RequestRoom
                    {
                        Adults = roomDetail.Adults, Children = roomDetail.Children, Infants = roomDetail.Infants
                    }).ToArray()
            };

            return request;
        }
    }
}
