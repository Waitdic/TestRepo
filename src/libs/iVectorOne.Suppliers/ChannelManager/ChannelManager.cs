namespace iVectorOne.Suppliers.ChannelManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.ChannelManager.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;

    public class ChannelManager : IThirdParty, ISingleSource
    {
        private readonly IChannelManagerSettings _settings;

        private readonly ITPSupport _support;

        private readonly ISerializer _serializer;

        private readonly HttpClient _httpClient;

        private readonly ILogger<ChannelManager> _logger;

        public ChannelManager(
            IChannelManagerSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<ChannelManager> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string Source => ThirdParties.CHANNELMANAGER;

        public bool SupportsRemarks => false;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return true;
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return 0;
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            var webRequest = new Request();
            bool success = false;

            try
            {
                var requestXml = BuildPreBookRequest(propertyDetails);

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    CreateLog = true,
                    Source = propertyDetails.Source,
                    LogFileName = "Pre-Book"
                };

                webRequest.SetRequest(requestXml);
                await webRequest.Send(_httpClient, _logger);

                var responseXml = webRequest.ResponseXML;
                var response = _serializer.DeSerialize<PreBookResponse>(responseXml);

                ThrowWhenHasExceptions(response.ReturnStatus.Errors);

                if (response.ReturnStatus.Success)
                {
                    success = true;

                    foreach (var roomDetails in propertyDetails.Rooms)
                    {
                        var matchingRoom = response.Rooms.First(r => r.Seq == roomDetails.PropertyRoomBookingID);
                        roomDetails.LocalCost = matchingRoom.NetCost;
                        roomDetails.ThirdPartyReference = matchingRoom.RoomBookingToken;
                    }

                    propertyDetails.LocalCost = response.NetCost;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBookException", ex.ToString());
                success = false;
            }
            finally
            {
                propertyDetails.AddLog("Prebook", webRequest);
            }

            return success;
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string reference = "failed";
            var request = new Request();

            try
            {
                var bookRequestXml = await BuildBookRequestAsync(propertyDetails);

                request = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    Source = propertyDetails.Source,
                    LogFileName = "Book",
                    CreateLog = true
                };

                request.SetRequest(bookRequestXml);
                await request.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<BookResponse>(request.ResponseXML);
                var returnStatus = response.ReturnStatus;

                ThrowWhenHasExceptions(returnStatus.Errors);

                if (returnStatus.Success)
                {
                    reference = response.BookingReference;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("BookException", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Book", request);
            }

            return reference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var request = new Request();

            var response = new ThirdPartyCancellationResponse { Success = true };

            try
            {
                var cancellationCost = await GetCancellationCostAsync(propertyDetails);

                var cancelRequest = new CancelRequest
                {
                    LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings),
                    BookingReference = propertyDetails.SourceReference,
                    CancellationCost = cancellationCost.Amount,
                    LeadGuestFirstName = propertyDetails.LeadGuestFirstName,
                    LeadGuestLastName = propertyDetails.LeadGuestLastName,
                    Guests = new CancelRequest.GuestConfiguration
                    {
                        Adults = propertyDetails.Adults,
                        Children = propertyDetails.Children,
                        Infants = propertyDetails.Infants
                    }
                };

                request = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    CreateLog = true,
                    Source = propertyDetails.Source,
                    LogFileName = "Cancellation",
                };

                request.SetRequest(_serializer.Serialize(cancelRequest));
                await request.Send(_httpClient, _logger);

                var cancelResponse = _serializer.DeSerialize<CancelResponse>(request.ResponseXML);

                ThrowWhenHasExceptions(cancelResponse.ReturnStatus.Errors);

                if (!cancelResponse.ReturnStatus.Success)
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                propertyDetails.Warnings.AddNew("Cancel Exception", ex.Message);
            }
            finally
            {
                propertyDetails.AddLog("Cancel", request);
            }

            return response;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        public void EndSession(PropertyDetails propertyDetails)
        {

        }

        private static void ThrowWhenHasExceptions(List<string> exceptions)
        {
            if (!exceptions.Any())
                return;

            var sbWarning = new StringBuilder();
            foreach (string warning in exceptions)
            {
                sbWarning.AppendLine(Helper.RemoveLoginDetailsFromWarnings(warning));
            }

            throw new Exception(sbWarning.ToString());
        }

        private XmlDocument BuildPreBookRequest(PropertyDetails propertyDetails)
        {
            var request = new PreBookRequest
            {
                LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings),
                BrandID = _settings.BrandCode(propertyDetails),
                PropertyReferenceID = propertyDetails.TPKey.ToSafeInt(),
                CheckInDate = propertyDetails.ArrivalDate,
                CheckOutDate = propertyDetails.ArrivalDate.AddDays(propertyDetails.Duration),
                Rooms = propertyDetails.Rooms
                    .Select(room => new PreBookRequest.Room
                        {
                            RoomBookingToken = room.ThirdPartyReference,
                            Adults = room.Adults,
                            Children = room.Children,
                            Infants = room.Infants,
                            ChildAgeCSV = string.Join(",", room.ChildAges),
                            Seq = room.PropertyRoomBookingID,
                            BrandID = _settings.BrandCode(propertyDetails),
                        })
                    .ToList()
            };

            return _serializer.Serialize(request);
        }

        private async Task<XmlDocument> BuildBookRequestAsync(PropertyDetails propertyDetails)
        {
            var bookRequest = new BookRequest
            {
                LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings),

                BookingReference = propertyDetails.BookingReference,
                PropertyReferenceID = propertyDetails.TPKey.ToSafeInt(),
                CheckInDate = propertyDetails.ArrivalDate,
                CheckOutDate = propertyDetails.ArrivalDate.AddDays(propertyDetails.Duration),

                Rooms = BuildRoomBookings(propertyDetails),
                HotelRequests = BuildBookingComments(propertyDetails.BookingComments),

                LeadGuestDetails = new BookRequest.LeadGuestDetail
                {
                    Title = propertyDetails.LeadGuestTitle,
                    FirstName = propertyDetails.LeadGuestFirstName,
                    LastName = propertyDetails.LeadGuestLastName,
                    DateOfBirth = propertyDetails.LeadGuestDateOfBirth,
                    Telephone = propertyDetails.LeadGuestPhone
                },
            };

            bookRequest.LeadGuestDetails.Address1 = propertyDetails.LeadGuestAddress1;
            bookRequest.LeadGuestDetails.Address2 = propertyDetails.LeadGuestAddress2;
            bookRequest.LeadGuestDetails.TownCity = propertyDetails.LeadGuestTownCity;
            bookRequest.LeadGuestDetails.County = propertyDetails.LeadGuestCounty;
            bookRequest.LeadGuestDetails.Postcode = propertyDetails.LeadGuestPostcode;
            bookRequest.LeadGuestDetails.Country = await _support.TPCountryLookupAsync(
                propertyDetails.Source,
                propertyDetails.LeadGuestCountryCode,
                propertyDetails.SubscriptionID);
            bookRequest.LeadGuestDetails.Email = propertyDetails.LeadGuestEmail;

            return _serializer.Serialize(bookRequest);
        }

        private static List<BookRequest.Room> BuildRoomBookings(PropertyDetails propertyDetails)
        {
            var roomBookings = new List<BookRequest.Room>();

            foreach (var room in propertyDetails.Rooms)
            {
                var roomBooking = new BookRequest.Room
                {
                    Seq = room.PropertyRoomBookingID,
                    Adults = room.Adults,
                    Children = room.Children,
                    Infants = room.Infants,
                    ChildAgeCSV = string.Join(",", room.ChildAges),
                    RoomBookingToken = room.ThirdPartyReference,
                    RoomType = room.RoomType,
                    MealBasis = room.MealBasis,
                    GuestDetails = BuildGuestDetails(room),
                };

                roomBookings.Add(roomBooking);
            }

            return roomBookings;
        }

        private static List<BookRequest.GuestDetail> BuildGuestDetails(RoomDetails room)
        {
            var guestDetails = new List<BookRequest.GuestDetail>();

            foreach (var passenger in room.Passengers)
            {
                guestDetails.Add(new BookRequest.GuestDetail
                {
                    GuestType = passenger.PassengerType.ToString(),
                    Title = passenger.Title,
                    FirstName = passenger.FirstName,
                    LastName = passenger.LastName,
                    DateOfBirth = passenger.DateOfBirth
                });
            }

            return guestDetails;
        }

        private static List<string> BuildBookingComments(BookingComments bookingComments)
        {
            var requests = new List<string>();

            foreach (var comment in bookingComments)
            {
                requests.Add(comment.Text);
            }

            return requests;
        }


        #region Helper Classes

        private class Address
        {
            public string Line1 { get; }

            public string Line2 { get; }

            public string City { get; }

            public string County { get; }

            public string PostCode { get; }

            public string CountryCode { get; }

            public Address(string config)
            {
                string[] items = config.Split('|');

                Line1 = items[0];
                Line2 = items[1];
                City = items[2];
                County = items[3];
                PostCode = items[4];
                CountryCode = items[5];
            }
        }

        #endregion
    }
}