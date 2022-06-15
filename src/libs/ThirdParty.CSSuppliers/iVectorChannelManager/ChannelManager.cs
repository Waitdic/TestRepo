namespace ThirdParty.CSSuppliers.iVectorChannelManager
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
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.iVectorChannelManager.Models;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class ChannelManager : IThirdParty, ISingleSource
    {
        private readonly IChannelManagerSettings _settings;

        private readonly Serializer _serializer;

        private readonly HttpClient _httpClient;

        private readonly ILogger<ChannelManager> _logger;

        public ChannelManager(IChannelManagerSettings settings, HttpClient httpClient, ILogger<ChannelManager> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
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

        public bool TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails)
        {
            return false;
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
            Request? webRequest = null;
            bool success = false;

            try
            {
                var requestXml = BuildPreBookRequest(propertyDetails);

                webRequest = new Request
                {
                    EndPoint = _settings.URL(propertyDetails),
                    Method = eRequestMethod.POST,
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
                    }

                    propertyDetails.LocalCost = response.NetCost;
                    propertyDetails.CurrencyID = response.CurrencyID;
                    propertyDetails.TotalCommission = response.Commission;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBookException", ex.ToString());
                success = false;
            }
            finally
            {
                if (webRequest?.RequestXML != null && !string.IsNullOrEmpty(webRequest.RequestXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "ChannelManager Prebook Request", webRequest.RequestXML);
                }

                if (webRequest?.ResponseXML != null && !string.IsNullOrEmpty(webRequest.ResponseXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "ChannelManager Prebook Response", webRequest.ResponseXML);
                }
            }

            return success;
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string reference = "failed";
            Request? request = null;

            try
            {
                var bookRequestXml = BuildBookRequest(propertyDetails);

                request = new Request
                {
                    EndPoint = _settings.URL(propertyDetails),
                    Method = eRequestMethod.POST,
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
                if (request?.RequestXML != null && !string.IsNullOrEmpty(request.RequestXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "IVC Book Request", request.RequestXML);
                }

                if (request?.ResponseXML != null && !string.IsNullOrEmpty(request.ResponseXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "IVC Book Response", request.ResponseXML);
                }
            }

            return reference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            Request? request = null;

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
                    EndPoint = _settings.URL(propertyDetails),
                    Method = eRequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    CreateLog = true,
                    Source = propertyDetails.Source,
                    LogFileName = "Cancellation",
                };

                request.SetRequest(cancelRequest.ToString());
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
                if (request?.RequestXML != null && !string.IsNullOrEmpty(request.RequestXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "IVC Cancel Request", request.RequestXML);
                }

                if (request?.ResponseXML != null && !string.IsNullOrEmpty(request.ResponseXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "IVC Cancel Response", request.ResponseXML);
                }
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
            foreach (string sWarning in exceptions)
            {
                sbWarning.AppendLine(Helper.RemoveLoginDetailsFromWarnings(sWarning));
            }

            throw new Exception(sbWarning.ToString());
        }

        private XmlDocument BuildPreBookRequest(PropertyDetails propertyDetails)
        {
            var request = new PreBookRequest
            {
                LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings),
                BrandID = _settings.BrandID(propertyDetails),
                PropertyReferenceID = propertyDetails.TPKey.ToSafeInt(),
                CheckInDate = propertyDetails.ArrivalDate,
                CheckOutDate = propertyDetails.ArrivalDate.AddDays(propertyDetails.Duration),
                Rooms = (List<PreBookRequest.Room>)propertyDetails.Rooms.Select(room => new PreBookRequest.Room
                {
                    RoomBookingToken = room.ThirdPartyReference,
                    Adults = room.Adults,
                    Children = room.Children,
                    Infants = room.Infants,
                    ChildAgeCSV = string.Join<int>(",", room.ChildAges),
                    PropertyRoomTypeID = room.PropertyRoomBookingID,
                    PropertyID = propertyDetails.PropertyID,
                    BrandID = _settings.BrandID(propertyDetails),
                })
            };

            return _serializer.Serialize(request);
        }

        private XmlDocument BuildBookRequest(PropertyDetails propertyDetails)
        {
            var bookRequest = new BookRequest
            {
                LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings),

                BookingReference = propertyDetails.BookingReference,
                PropertyReferenceID = propertyDetails.TPKey.ToSafeInt(),
                CheckInDate = propertyDetails.ArrivalDate,
                CheckOutDate = propertyDetails.ArrivalDate.AddDays(propertyDetails.Duration),

                Rooms = AddRoomBookingsToRequest(propertyDetails),
                HotelRequests = AddBookingCommentsToRequest(propertyDetails.BookingComments),

                LeadGuestDetails = new BookRequest.LeadGuestDetail
                {
                    Title = propertyDetails.LeadGuestTitle,
                    FirstName = propertyDetails.LeadGuestFirstName,
                    LastName = propertyDetails.LeadGuestLastName,
                    DateOfBirth = propertyDetails.DateOfBirth,
                    Telephone = propertyDetails.LeadGuestPhone
                },

                PaymentDetails = new BookRequest.PaymentDetail
                {
                    Amount = propertyDetails.LocalCost,
                    CurrencyCode = propertyDetails.CurrencyCode
                }
            };

            bookRequest.LeadGuestDetails.Address1 = propertyDetails.LeadGuestAddress1;
            bookRequest.LeadGuestDetails.Address2 = propertyDetails.LeadGuestAddress2;
            bookRequest.LeadGuestDetails.TownCity = propertyDetails.LeadGuestTownCity;
            bookRequest.LeadGuestDetails.County = propertyDetails.LeadGuestCounty;
            bookRequest.LeadGuestDetails.Postcode = propertyDetails.LeadGuestPostcode;
            bookRequest.LeadGuestDetails.Country = propertyDetails.LeadGuestBookingCountry;
            bookRequest.LeadGuestDetails.Email = propertyDetails.LeadGuestEmail;

            return _serializer.Serialize(bookRequest);
        }

        private static List<string> AddBookingCommentsToRequest(BookingComments bookingComments)
        {
            var requests = new List<string>();

            foreach (var comment in bookingComments)
            {
                requests.Add(comment.Text);
            }

            return requests;
        }

        private static List<BookRequest.GuestDetail> AddGuestDetailsToRequest(RoomDetails room)
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

        private static List<BookRequest.Room> AddRoomBookingsToRequest(PropertyDetails propertyDetails)
        {
            var roomBookings = new List<BookRequest.Room>();

            foreach (var room in propertyDetails.Rooms)
            {
                var roomBooking = new BookRequest.Room
                {
                    Adults = room.Adults,
                    Children = room.Children,
                    Infants = room.Infants,
                    ChildAgeCSV = string.Join<int>(",", room.ChildAges),
                    RoomBookingToken = room.ThirdPartyReference,
                    RoomType = room.RoomType,
                    MealBasis = room.MealBasis,
                    GuestDetails = AddGuestDetailsToRequest(room),
                };

                roomBookings.Add(roomBooking);
            }

            return roomBookings;
        }

        #region "Helper Classes"

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