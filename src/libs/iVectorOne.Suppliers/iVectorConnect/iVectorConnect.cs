namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Lookups;
    using Microsoft.Extensions.Logging;
    using ThirdParty.CSSuppliers.iVectorConnect.Models;
    using ThirdParty.CSSuppliers.iVectorConnect.Models.Common;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class iVectorConnect : IThirdParty, IMultiSource
    {
        private const string DateFormat = "yyyy-MM-dd";

        private readonly IiVectorConnectSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<iVectorConnect> _logger;

        public iVectorConnect(
            IiVectorConnectSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<iVectorConnect> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public List<string> Sources => Helper.iVectorConnectSources;

        public bool SupportsRemarks => false;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source) => true;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source) => 0;

        public bool RequiresVCard(VirtualCardInfo info, string source) => false;

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            var webRequest = new Request();
            bool success = false;

            try
            {
                var request = BuildPreBookRequest(propertyDetails);

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails, propertyDetails.Source),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_xml,
                    CreateLog = true,
                    Source = propertyDetails.Source,
                    LogFileName = "Pre-Book"
                };

                webRequest.SetRequest(_serializer.Serialize(request));
                await webRequest.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<PropertyPreBookResponse>(webRequest.ResponseXML);

                ThrowWhenHasExceptions(response.ReturnStatus.Exceptions);

                if (response.ReturnStatus.Success)
                {
                    success = true;

                    propertyDetails.TPRef1 = response.BookingToken;

                    propertyDetails.LocalCost = response.TotalPrice;
                    decimal roomCost = propertyDetails.LocalCost / propertyDetails.Rooms.Count;

                    foreach (var roomDetails in propertyDetails.Rooms.Where(o => roomCost != o.LocalCost))
                    {
                        roomDetails.LocalCost = roomCost;
                    }

                    foreach (var cancellation in response.Cancellations)
                    {
                        propertyDetails.Cancellations.AddNew(cancellation.StartDate, cancellation.EndDate, cancellation.Amount);
                    }

                    foreach (var erratum in response.Errata)
                    {
                        propertyDetails.Errata.AddNew(erratum.ErratumSubject, erratum.ErratumDescription);
                    }
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
            var webRequest = new Request();

            try
            {
                var request = await BuildBookRequestAsync(propertyDetails);

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails, propertyDetails.Source),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    Source = propertyDetails.Source,
                    LogFileName = "Book",
                    CreateLog = true
                };

                webRequest.SetRequest(_serializer.Serialize(request));
                await webRequest.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<BasketBookResponse>(webRequest.ResponseXML);
                var returnStatus = response.PropertyBookings.PropertyBookResponse.ReturnStatus;

                ThrowWhenHasExceptions(returnStatus.Exceptions);

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
                propertyDetails.AddLog("Book", webRequest);
            }

            return reference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var preCancelRequest = new Request();
            var response = new ThirdPartyCancellationResponse
            {
                Success = true,
                TPCancellationReference = propertyDetails.SourceReference,
                Amount = propertyDetails.LocalCost,
            };

            try
            {
                // send off another precancel to get the ref and amount (doesn't get stored in secondary ref for some reason -should be changed
                var preCancelWebRequest = new PreCancelRequest
                {
                    LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings, propertyDetails.Source),
                    BookingReference = propertyDetails.SourceReference,
                };

                //2.Send Request
                preCancelRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails, propertyDetails.Source),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    CreateLog = true,
                    Source = propertyDetails.Source,
                    LogFileName = "Pre-Cancel",
                };

                preCancelRequest.SetRequest(_serializer.Serialize(preCancelWebRequest));
                await preCancelRequest.Send(_httpClient, _logger);

                // 3b. Process the Response
                // store the amount as TPRef2
                var preCancelResponse = _serializer.DeSerialize<PreCancelResponse>(preCancelRequest.ResponseXML);

                // 1.Build the XML
                var cancelRequest = new CancelRequest
                {
                    LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings, propertyDetails.Source),
                    BookingReference = propertyDetails.SourceReference,
                    CancellationCost = preCancelResponse.CancellationCost,
                    CancellationToken = preCancelResponse.CancellationToken,
                };

                // 2. Send the Request
                request = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails, propertyDetails.Source),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    CreateLog = true,
                    Source = propertyDetails.Source,
                    LogFileName = "Cancellation",
                };

                request.SetRequest(_serializer.Serialize(cancelRequest));
                await request.Send(_httpClient, _logger);

                // 3. Process Response
                var cancelResponse = _serializer.DeSerialize<CancelResponse>(request.ResponseXML);

                // Add Warnings If Any
                ThrowWhenHasExceptions(cancelResponse.ReturnStatus.Exceptions);

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
                propertyDetails.AddLog("PreCancel", preCancelRequest);
                propertyDetails.AddLog("Cancel", request);
            }

            return response;
        }

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationFeeResult = new ThirdPartyCancellationFeeResult();
            var webRequest = new Request();

            try
            {
                // 1.Build XML Request
                var preCancelRequest = new PreCancelRequest
                {
                    LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings, propertyDetails.Source),
                    BookingReference = propertyDetails.SourceReference
                };

                var preCancelRequestXml = _serializer.Serialize(preCancelRequest);

                // 2. Send Request
                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails, propertyDetails.Source),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    CreateLog = true,
                    Source = propertyDetails.Source,
                    LogFileName = "Pre-Cancel"
                };

                webRequest.SetRequest(preCancelRequestXml);
                await webRequest.Send(_httpClient, _logger);

                // 3a. Check the Response was OK
                var preCancelResponse = _serializer.DeSerialize<PreCancelResponse>(webRequest.ResponseXML);

                // Add Warnings If Any
                ThrowWhenHasExceptions(preCancelResponse.ReturnStatus.Exceptions);

                // 3b. Process the Response
                thirdPartyCancellationFeeResult.Success = preCancelResponse.ReturnStatus.Success;

                // store the amount as secondary ref
                propertyDetails.SourceSecondaryReference += preCancelResponse.CancellationCost;
                thirdPartyCancellationFeeResult.Amount = preCancelResponse.CancellationCost;

                // store the cancellation token in TPRef1 - overwrites old booking token
                propertyDetails.SourceSecondaryReference += $"|{preCancelResponse.CancellationToken}";

            }
            catch (Exception ex)
            {
                thirdPartyCancellationFeeResult.Success = false;
                propertyDetails.Warnings.AddNew("CancellationCosts Exception", ex.Message);
            }
            finally
            {
                propertyDetails.AddLog("PreCancel", webRequest);
            }

            return thirdPartyCancellationFeeResult;
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

        private static void ThrowWhenHasExceptions(string[] exceptions)
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

        private PropertyPreBookRequest BuildPreBookRequest(PropertyDetails propertyDetails)
        {
            string[] thirdPartyReference = propertyDetails.Rooms[0].ThirdPartyReference.Split('|');

            var request = new PropertyPreBookRequest
            {
                LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings, propertyDetails.Source),
                BookingToken = thirdPartyReference[0],
                ArrivalDate = propertyDetails.ArrivalDate.ToString(DateFormat),
                Duration = propertyDetails.Duration,
                RoomBookings = propertyDetails.Rooms.Select(oRoom => new RoomBooking
                     {
                         RoomBookingToken = thirdPartyReference[1],
                         GuestConfiguration =
                         {
                             Adults = oRoom.Adults,
                             Children = oRoom.Children,
                             Infants = oRoom.Infants,
                             ChildAges = oRoom.ChildAges.ToArray()
                         }
                     }).ToList(),
            };

            return request;
        }

        private async Task<BasketBookRequest> BuildBookRequestAsync(PropertyDetails propertyDetails)
        {
            var bookRequest = new BasketBookRequest
            {
                LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings, propertyDetails.Source),
                ExternalReference = propertyDetails.BookingReference,
                PropertyBookings = new PropertyBookings
                {
                    PropertyBookRequest = new PropertyBookRequest
                    {
                        BookingToken = propertyDetails.TPRef1,
                        ArrivalDate = propertyDetails.ArrivalDate.ToString(DateFormat),
                        Duration = propertyDetails.Duration,
                        ExpectedTotal = propertyDetails.LocalCost,
                        Request = propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any() ?
                                     string.Join("\n", propertyDetails.Rooms.Select(x => x.SpecialRequest)) :
                                     string.Empty,
                        RoomBookings = AddRoomBookingsToRequest(propertyDetails),
                    }
                },
                LeadCustomer = new LeadCustomer
                {
                    CustomerTitle = propertyDetails.LeadGuestTitle,
                    CustomerFirstName = propertyDetails.LeadGuestFirstName,
                    CustomerLastName = propertyDetails.LeadGuestLastName,
                },
                GuestDetails = AddGuestDetailsToRequest(propertyDetails),
            };

            if (_settings.UseAgentDetails(propertyDetails, propertyDetails.Source))
            {
                var address = await Address.SetupAddressAsync(
                    _settings.AgentAddress(propertyDetails, propertyDetails.Source),
                    propertyDetails.Source,
                    propertyDetails.SubscriptionID,
                    _support);
                bookRequest.LeadCustomer.CustomerAddress1 = address.Line1;
                bookRequest.LeadCustomer.CustomerAddress2 = address.Line2;
                bookRequest.LeadCustomer.CustomerTownCity = address.City;
                bookRequest.LeadCustomer.CustomerCounty = address.County;
                bookRequest.LeadCustomer.CustomerPostcode = address.PostCode;
                bookRequest.LeadCustomer.CustomerBookingCountryID = address.BookingCountryID;
                bookRequest.LeadCustomer.CustomerEmail = _settings.AgentEmailAddress(propertyDetails, propertyDetails.Source);
            }
            else
            {
                bookRequest.LeadCustomer.CustomerAddress1 = propertyDetails.LeadGuestAddress1;
                bookRequest.LeadCustomer.CustomerAddress2 = propertyDetails.LeadGuestAddress2;
                bookRequest.LeadCustomer.CustomerTownCity = propertyDetails.LeadGuestTownCity;
                bookRequest.LeadCustomer.CustomerCounty = propertyDetails.LeadGuestCounty;
                bookRequest.LeadCustomer.CustomerPostcode = propertyDetails.LeadGuestPostcode;
                bookRequest.LeadCustomer.CustomerBookingCountryID = (await _support.TPCountryCodeLookupAsync(
                    propertyDetails.Source,
                    propertyDetails.LeadGuestCountryCode,
                    propertyDetails.SubscriptionID)).ToSafeInt();
                bookRequest.LeadCustomer.CustomerEmail = propertyDetails.LeadGuestEmail;
            }

            return bookRequest;
        }

        private static List<GuestDetail> AddGuestDetailsToRequest(PropertyDetails propertyDetails)
        {
            var guestDetails = new List<GuestDetail>();
            int guestId = 1;

            foreach (var passenger in propertyDetails.Rooms.SelectMany(room => room.Passengers))
            {
                guestDetails.Add(new GuestDetail
                {
                    GuestID = guestId,
                    Type = passenger.PassengerType,
                    Title = passenger.Title,
                    FirstName = passenger.FirstName,
                    LastName = passenger.LastName,
                    Age = passenger.Age,
                    DateOfBirth = passenger.DateOfBirth.ToString(DateFormat)
                });

                guestId++;
            }

            return guestDetails;
        }

        private static RoomBooking[] AddRoomBookingsToRequest(PropertyDetails propertyDetails)
        {
            var roomBookings = new List<RoomBooking>();
            int guestId = 1;

            foreach (var room in propertyDetails.Rooms)
            {
                var guestIDs = new List<int>();
                foreach (var guest in room.Passengers)
                {
                    guestIDs.Add(guestId);
                    guestId++;
                }

                var roomBooking = new RoomBooking
                {
                    RoomBookingToken = room.ThirdPartyReference.Split('|')[1],
                    GuestIDs = guestIDs.ToArray()
                };

                roomBookings.Add(roomBooking);
            }

            return roomBookings.ToArray();
        }

        #region Helper Classes

        private class Address
        {
            public string Line1 { get; set; }

            public string Line2 { get; set; }

            public string City { get; set; }

            public string County { get; set; }

            public string PostCode { get; set; }

            public int BookingCountryID { get; set; }

            public static async Task<Address> SetupAddressAsync(string config, string source, int subscriptionId, ITPSupport support)
            {
                string[] items = config.Split('|');

                return new Address()
                {
                    Line1 = items[0],
                    Line2 = items[1],
                    City = items[2],
                    County = items[3],
                    PostCode = items[4],
                    BookingCountryID = (await support.TPCountryCodeLookupAsync(source, items[5], subscriptionId)).ToSafeInt(),
                };
            }
        }

        #endregion
    }
}