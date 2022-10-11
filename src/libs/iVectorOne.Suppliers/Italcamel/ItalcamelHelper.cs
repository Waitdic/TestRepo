namespace iVectorOne.Suppliers.Italcamel
{
    using System;
    using iVectorOne.Models.Property.Booking;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Models.Common;
    using Models.Envelope;
    using iVectorOne.Suppliers.Italcamel.Models.Search;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Search.Models;
    using Intuitive.Helpers.Net;
    using iVectorOne.Constants;

    public class ItalcamelHelper
    {
        public string Source => ThirdParties.ITALCAMEL;

        public enum SearchType
        {
            MacroRegion,
            City,
            Hotel
        }

        public string BuildSearchRequest(
            IItalcamelSettings settings,
            ISerializer serializer,
            IThirdPartyAttributeSearch searchDetails,
            string searchCode,
            SearchType searchType,
            DateTime arrivalDate,
            DateTime departureDate,
            iVector.Search.Property.RoomDetails roomDetails)
        {
            var request = new Envelope<GetAvailabilitySplitted>
            {
                Body =
                {
                    Content =
                    {
                        Request =
                        {
                            Username = settings.Login(searchDetails),
                            Password = settings.Password(searchDetails),
                            LanguageuId = settings.LanguageID(searchDetails),
                            AccomodationuId = searchType == SearchType.Hotel ? searchCode : string.Empty,
                            MacroregionuId = searchType == SearchType.MacroRegion ? searchCode : string.Empty,
                            CityuId =  searchType != SearchType.Hotel && searchType != SearchType.MacroRegion ? searchCode : string.Empty,
                            CheckIn = arrivalDate.ToString("yyyy-MM-dd"),
                            CheckOut = departureDate.ToString("yyyy-MM-dd"),
                            Rooms = roomDetails.Select(room => new SearchRoom
                            {
                                Adults = room.Adults,
                                Children = room.Children,
                                ChildAge1 = room.Children > 0 ? room.ChildAges[0] : 0,
                                ChildAge2 = room.Children > 1 ? room.ChildAges[1] : 0,
                            }).ToArray(),
                        }
                    }
                }
            };

            return CleanRequest(serializer.Serialize(request));
        }

        public string BuildSearchRequest(
            IItalcamelSettings settings,
            ISerializer serializer,
            PropertyDetails propertyDetails,
            string searchCode,
            SearchType searchType = SearchType.City)
        {
            return BuildSearchRequest(
                settings,
                serializer,
                propertyDetails,
                searchCode,
                searchType,
                propertyDetails.ArrivalDate,
                propertyDetails.DepartureDate,
                GetRoomDetailsFromThirdPartyRoomDetails(propertyDetails.Rooms));
        }

        public string BuildSearchRequest(
            IItalcamelSettings settings,
            ISerializer serializer,
            SearchDetails searchDetails,
            string searchCode,
            SearchType searchType = SearchType.City)
        {
            if ((searchDetails.DepartureDate - searchDetails.ArrivalDate).Days >
                settings.MaximumNumberOfNights(searchDetails)
                || searchDetails.Rooms > settings.MaximumRoomNumber(searchDetails))
            {
                throw new Exception("MaximumNumberOfNights or MaximumRoomGuestNumber or MaximumRoomNumber exceeded");
            }

            return BuildSearchRequest(
                settings,
                serializer,
                searchDetails,
                searchCode,
                searchType,
                searchDetails.ArrivalDate,
                searchDetails.DepartureDate,
                searchDetails.RoomDetails);
        }

        public iVector.Search.Property.RoomDetails GetRoomDetailsFromThirdPartyRoomDetails(List<RoomDetails> thirdPartyRoomDetails)
        {
            var roomDetails = new iVector.Search.Property.RoomDetails();

            foreach (var propertyRoomBooking in thirdPartyRoomDetails)
            {
                var oRoomDetail = new iVector.Search.Property.RoomDetail
                {
                    Adults = propertyRoomBooking.Adults,
                    Children = propertyRoomBooking.Children
                };

                for (var i = 0; i <= propertyRoomBooking.ChildAges.Count - 1; i++)
                    oRoomDetail.ChildAges.Add(propertyRoomBooking.ChildAges[i]);

                roomDetails.Add(oRoomDetail);
            }

            return roomDetails;
        }

        public string CleanRequest(XmlDocument request)
        {
            var requestString = request.OuterXml;
            return requestString
                .Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "")
                .Replace(@"xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""", "");
        }

        public Intuitive.Helpers.Net.Request CreateWebRequest(string url, string soapAction, bool createLog = false, string logFileName = "")
        {
            return new Intuitive.Helpers.Net.Request
            {
                EndPoint = url,
                Method = RequestMethod.POST,
                Source = Source,
                SoapAction = soapAction,
                SOAP = true,
                ContentType = ContentTypes.Text_xml,
                LogFileName = logFileName,
                CreateLog = createLog,
            };
        }
    }
}
