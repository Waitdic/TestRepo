namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Lookups;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.iVectorConnect.Models.Common;
    using ThirdParty.CSSuppliers.iVectorConnect.Models;

    public abstract class IVCBase : IThirdParty
    {
        private const string DateFormat = "yyyy-MM-dd";

        private readonly IIVectorConnectSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public IVCBase(
            IIVectorConnectSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public abstract string Source { get; }

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

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return 0;
        }

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        public bool PreBook(PropertyDetails propertyDetails)
        {
            Request? webRequest = null;
            bool success = false;

            try
            {
                // 1. Build the XML
                var requestXml = BuildPreBookXml(propertyDetails);

                // 2.Send The Request
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
                webRequest.Send(_httpClient, _logger).RunSynchronously();

                var responseXml = webRequest.ResponseXML;
                var response = _serializer.DeSerialize<PropertyPreBookResponse>(responseXml);

                // Add Warnings If Any
                ThrowWhenHasExceptions(response.ReturnStatus.Exceptions);

                // 2b. Check the Response Was OK
                if (response.ReturnStatus.Success)
                {
                    // 3.ProcessResults

                    // 3a. Properties
                    success = true;

                    // booking token
                    propertyDetails.TPRef1 = response.BookingToken;

                    decimal dLocalCost = response.TotalPrice;
                    decimal dRoomCost = dLocalCost / propertyDetails.Rooms.Count;
                    foreach (var oRoomDetails in propertyDetails.Rooms.Where(oRoomDetails => dRoomCost != oRoomDetails.LocalCost))
                    {
                        oRoomDetails.LocalCost = dRoomCost;
                    }

                    // Cancellations
                    foreach (var oCancellation in response.Cancellations)
                    {
                        propertyDetails.Cancellations.AddNew(oCancellation.StartDate, oCancellation.EndDate,
                            oCancellation.Amount);
                    }

                    // Errata
                    foreach (var oErratum in response.Errata)
                    {
                        propertyDetails.Errata.AddNew(oErratum.ErratumSubject, oErratum.ErratumDescription);
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
                // store the request and response xml on the property booking
                if (webRequest?.RequestXML != null && !string.IsNullOrEmpty(webRequest.RequestXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "IVC Prebook Request", webRequest.RequestXML);
                }

                if (webRequest?.ResponseXML != null && !string.IsNullOrEmpty(webRequest.ResponseXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "IVC Prebook Response", webRequest.ResponseXML);
                }
            }

            return success;
        }

        public string Book(PropertyDetails propertyDetails)
        {
            string reference = "failed";
            Request? request = null;

            try
            {
                var bookRequestXml = BuildBookXml(propertyDetails);

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
                request.Send(_httpClient, _logger).RunSynchronously();

                var response = _serializer.DeSerialize<BasketBookResponse>(request.ResponseXML);
                var returnStatus = response.PropertyBookings.PropertyBookResponse.ReturnStatus;

                // Add Warnings If Any
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

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
        {
            Request? request = null;
            Request? preCancelRequest = null;
            var response = new ThirdPartyCancellationResponse { Success = true };

            try
            {
                // send off another precancel to get the ref and amount (doesn't get stored in secondary ref for some reason -should be changed
                var preCancelWebRequest = new PreCancelRequest
                {
                    LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings),
                    BookingReference = propertyDetails.SourceReference,
                };

                //2.Send Request
                preCancelRequest = new Request
                {
                    EndPoint = _settings.URL(propertyDetails),
                    Method = eRequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    CreateLog = true,
                    Source = propertyDetails.Source,
                    LogFileName = "Pre-Cancel",
                };

                preCancelRequest.SetRequest(preCancelWebRequest.ToString());
                preCancelRequest.Send(_httpClient, _logger).RunSynchronously();

                // 3b. Process the Response
                // store the amount as TPRef2
                var preCancelResponse = _serializer.DeSerialize<PreCancelResponse>(preCancelRequest.ResponseXML);

                // 1.Build the XML
                var cancelRequest = new CancelRequest
                {
                    LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings),
                    BookingReference = propertyDetails.SourceReference,
                    CancellationCost = preCancelResponse.CancellationCost,
                    CancellationToken = preCancelResponse.CancellationToken,
                };

                // 2. Send the Request
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
                request.Send(_httpClient, _logger).RunSynchronously();

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
                if (preCancelRequest?.RequestXML != null && !string.IsNullOrEmpty(preCancelRequest.RequestXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "IVC 2nd PreCancel Request2", preCancelRequest.RequestXML);
                }

                if (preCancelRequest?.ResponseXML != null && !string.IsNullOrEmpty(preCancelRequest.ResponseXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "IVC 2nd PreCancel Response", preCancelRequest.ResponseXML);
                }

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

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationFeeResult = new ThirdPartyCancellationFeeResult();
            Request? webRequest = null;

            try
            {
                // 1.Build XML Request
                var preCancelRequest = new PreCancelRequest
                {
                    LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings),
                    BookingReference = propertyDetails.SourceReference
                };

                var preCancelRequestXml = _serializer.Serialize(preCancelRequest);

                // 2. Send Request
                webRequest = new Request
                {
                    EndPoint = _settings.URL(propertyDetails),
                    Method = eRequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    CreateLog = true,
                    Source = propertyDetails.Source,
                    LogFileName = "Pre-Cancel"
                };

                webRequest.SetRequest(preCancelRequestXml);
                webRequest.Send(_httpClient, _logger).RunSynchronously();

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
                // store the request and response xml on the property booking
                if (webRequest?.RequestXML != null && !string.IsNullOrEmpty(webRequest.RequestXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "IVC 1st PreCancel Request2", webRequest.RequestXML);
                }

                if (webRequest?.ResponseXML != null && !string.IsNullOrEmpty(webRequest.ResponseXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "IVC 1st PreCancel Response", webRequest.ResponseXML);
                }
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
            foreach (string sWarning in exceptions)
            {
                sbWarning.AppendLine(Helper.RemoveLoginDetailsFromWarnings(sWarning));
            }

            throw new Exception(sbWarning.ToString());
        }

        private XmlDocument BuildPreBookXml(PropertyDetails propertyDetails)
        {
            string[] thirdPartyReference = propertyDetails.Rooms[0].ThirdPartyReference.Split('|');

            var request = new PropertyPreBookRequest
            {
                LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings),
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
                     }).ToArray()
            };

            return _serializer.Serialize(request);
        }

        private XmlDocument BuildBookXml(PropertyDetails propertyDetails)
        {
            var bookRequest = new BasketBookRequest
            {
                LoginDetails = Helper.GetLoginDetails(propertyDetails, _settings),
                PropertyBookings = new PropertyBookings
                {
                    PropertyBookRequest = new PropertyBookRequest
                    {
                        BookingToken = propertyDetails.TPRef1,
                        ArrivalDate = propertyDetails.ArrivalDate.ToString(DateFormat),
                        Duration = propertyDetails.Duration,
                        ExpectedTotal = propertyDetails.LocalCost,
                        Request = AddBookingCommentsToRequest(propertyDetails.BookingComments),
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

            if (_settings.UseAgentDetails(propertyDetails))
            {
                var address = new Address(_settings.AgentAddress(propertyDetails), propertyDetails.Source, _support);
                bookRequest.LeadCustomer.CustomerAddress1 = address.Line1;
                bookRequest.LeadCustomer.CustomerAddress2 = address.Line2;
                bookRequest.LeadCustomer.CustomerTownCity = address.City;
                bookRequest.LeadCustomer.CustomerCounty = address.County;
                bookRequest.LeadCustomer.CustomerPostcode = address.PostCode;
                bookRequest.LeadCustomer.CustomerBookingCountryID = address.BookingCountryID;
                bookRequest.LeadCustomer.CustomerEmail = _settings.AgentEmailAddress(propertyDetails);
            }
            else
            {
                bookRequest.LeadCustomer.CustomerAddress1 = propertyDetails.LeadGuestAddress1;
                bookRequest.LeadCustomer.CustomerAddress2 = propertyDetails.LeadGuestAddress2;
                bookRequest.LeadCustomer.CustomerTownCity = propertyDetails.LeadGuestTownCity;
                bookRequest.LeadCustomer.CustomerCounty = propertyDetails.LeadGuestCounty;
                bookRequest.LeadCustomer.CustomerPostcode = propertyDetails.LeadGuestPostcode;
                bookRequest.LeadCustomer.CustomerBookingCountryID = propertyDetails.LeadGuestBookingCountryID;
                bookRequest.LeadCustomer.CustomerEmail = propertyDetails.LeadGuestEmail;
            }

            return _serializer.Serialize(bookRequest);
        }

        private static string AddBookingCommentsToRequest(BookingComments bookingComments)
        {
            return bookingComments.Count > 0
                ? bookingComments.ToString()
                : string.Empty;
        }

        private static GuestDetail[] AddGuestDetailsToRequest(PropertyDetails propertyDetails)
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

            return guestDetails.ToArray();
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

        #region "Helper Classes"

        private class Address
        {
            public string Line1 { get; }

            public string Line2 { get; }

            public string City { get; }

            public string County { get; }

            public string PostCode { get; }

            public int BookingCountryID { get; }

            public Address(string config, string source, ITPSupport support)
            {
                string[] items = config.Split('|');

                Line1 = items[0];
                Line2 = items[1];
                City = items[2];
                County = items[3];
                PostCode = items[4];
                BookingCountryID = support.TPBookingCountryCodeLookup(source, items[5]);
            }
        }

        #endregion
    }
}
