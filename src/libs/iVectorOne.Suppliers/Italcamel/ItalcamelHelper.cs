using iVectorOne.Suppliers.Italcamel.Models.Cancel;

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
            string[] searchCodes,
            DateTime arrivalDate,
            DateTime departureDate,
            iVector.Search.Property.RoomDetails roomDetails)
        {
            var request = new Envelope<GetAvailability>
            {
                Body =
                {
                    Content =
                    {
                        Request =
                        {
                            Username = settings.Login(searchDetails),
                            Password = settings.Password(searchDetails),
                            LanguageUID = settings.LanguageID(searchDetails),
                            Accommodations = searchCodes.Length != 0 ? searchCodes : Array.Empty<string>(),
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

            return serializer.Serialize(request).OuterXml;
        }

        public string BuildSearchRequest(
            IItalcamelSettings settings,
            ISerializer serializer,
            PropertyDetails propertyDetails,
            string[] searchCodes)
        {
            return BuildSearchRequest(
                settings,
                serializer,
                propertyDetails,
                searchCodes,
                propertyDetails.ArrivalDate,
                propertyDetails.DepartureDate,
                GetRoomDetailsFromThirdPartyRoomDetails(propertyDetails.Rooms));
        }

        public string BuildSearchRequest(
            IItalcamelSettings settings,
            ISerializer serializer,
            SearchDetails searchDetails,
            string[] searchCodes)
        {
            return BuildSearchRequest(
                settings,
                serializer,
                searchDetails,
                searchCodes,
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

        public string BuildBookingChargeRequest(IItalcamelSettings settings, ISerializer serializer, PropertyDetails propertyDetails, string UID)
        {
            var request = new Envelope<GetBookingCharge>
            {
                Body =
                {
                    Content =
                    {
                        Username = settings.Login(propertyDetails),
                        Password = settings.Password(propertyDetails),
                        LanguageUID = settings.LanguageID(propertyDetails),
                        BookingUID = UID,
                    }
                }
            };

            return serializer.Serialize(request).OuterXml;
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
