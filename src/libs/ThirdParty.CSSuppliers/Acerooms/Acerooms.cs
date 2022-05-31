namespace ThirdParty.CSSuppliers.Acerooms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using Intuitive;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Search.Models;
    using ThirdParty.CSSuppliers.AceRooms.Models;

    public class Acerooms : IThirdParty
    {
        #region "Properties"

        private readonly IAceroomsSettings _settings;
        private readonly ITPSupport _support;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Acerooms> _logger;

        public string Source => ThirdParties.ACEROOMS;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public bool TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails)
        {
            return false;
        }

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool SupportsRemarks => false;

        public bool SupportsBookingSearch => false;

        public string CreateReconciliationReference(string inputReference)
        {
            return "";
        }

        #endregion

        #region "Constructors"

        public Acerooms(IAceroomsSettings settings, ITPSupport support, HttpClient httpClient, ILogger<Acerooms> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region "Prebook"

        public bool PreBook(PropertyDetails propertyDetails)
        {
            bool prebookSuccess = true;
            var request = new Request();

            try
            {
                AceroomsPrebookRequest aceroomsPrebookRequest = CreatePrebookRequest(propertyDetails);
                string requestString = JsonConvert.SerializeObject(aceroomsPrebookRequest);

                request = CreateWebRequest(ref propertyDetails, "PreBookRoom", "Prebook", eRequestMethod.POST, requestString);
                request.Send(_httpClient, _logger).RunSynchronously();

                string responseString = request.ResponseString;
                AceroomsPrebookResponse response = responseString is not null and not ""
                           ? JsonConvert.DeserializeObject<AceroomsPrebookResponse>(responseString)
                           : throw new Exception("Third Party Error, Response was Invalid", new Exception("Third Party Error, Response was Invalid"));

                propertyDetails.Errata = GetErrataForAllRooms(response.Hotels); // store errata for all rooms
                propertyDetails.Cancellations = GetCancellationTermsForAllRooms(propertyDetails, response.Hotels);
                propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);
                GetPrebookToken(ref propertyDetails, response);

                prebookSuccess = response.Hotels.Any();
            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", exception.ToString());
                prebookSuccess = false;
            }
            finally
            {
                if (request.RequestString != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.ACEROOMS, "Acerooms PreBook Request", request.RequestString);
                }

                if (request.ResponseString != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.ACEROOMS, "Acerooms PreBook Response", request.ResponseString);
                }
            }

            return prebookSuccess;
        }

        #endregion

        #region "Book"

        public string Book(PropertyDetails propertyDetails)
        {
            string reference = "";
            var webRequest = new Request();

            try
            {
                AceroomsBookRequest aceroomsBookRequest = CreateBookRequest(propertyDetails);
                string requestString = JsonConvert.SerializeObject(aceroomsBookRequest);

                webRequest = CreateWebRequest(ref propertyDetails, "ConfirmRoom", "Book", eRequestMethod.POST, requestString);
                webRequest.Send(_httpClient, _logger).RunSynchronously();

                AceroomsBookResponse response = JsonConvert.DeserializeObject<AceroomsBookResponse>(webRequest.ResponseString);

                reference = (response.Booking.Rooms != null &&
                             string.IsNullOrEmpty(response.ErrorInfo) &&
                             response.Booking.Rooms.All(r => r.Status.Equals("Confirmed"))) ? GetRoomBookingIDs(response) : "failed";

            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Book Exception", exception.ToString());
                reference = "failed";
            }
            finally
            {
                if (webRequest.RequestString != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.ACEROOMS, "Acerooms Book Request", webRequest.RequestString);
                }

                if (!string.IsNullOrEmpty(webRequest.RequestString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.ACEROOMS, "Acerooms Book Response", webRequest.ResponseString);
                }
            }

            return reference;
        }

        #endregion

        #region "Booking Search"

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        #endregion

        #region "Booking Status Update"

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        #endregion

        #region "Cancel Booking"

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
        {
            List<string> roomBookingIDs = propertyDetails.SourceReference.Split(',').ToList(); // Create a list of all room bookings to be cancelled
            ThirdPartyCancellationResponse cancellationResponse = new ThirdPartyCancellationResponse
            {
                Success = true
            };

            try
            {
                List<string> cancelReferences = new List<string>();

                // sent a cancel request for each room
                foreach (var roomBookingID in roomBookingIDs)
                {
                    var webRequest = CreateWebRequest(ref propertyDetails, "CancelRoom/", "Cancel", eRequestMethod.GET);
                    var responseSucess = CancelRoomBooking(roomBookingID, ref cancellationResponse, webRequest, ref cancelReferences, ref propertyDetails);

                    if (!responseSucess)
                    {
                        cancellationResponse.Success = false;
                        break;
                    }
                }

                if (cancellationResponse.Success)
                {
                    cancellationResponse.CurrencyCode = _support.CurrencyLookup(propertyDetails.CurrencyID);
                    cancellationResponse.TPCancellationReference = string.Join("|", cancelReferences);
                    cancellationResponse.CostRecievedFromThirdParty = cancellationResponse.Amount > 0;
                }

            }
            catch (Exception exception)
            {
                cancellationResponse.Success = false;

                cancellationResponse.TPCancellationReference = "failed";

                propertyDetails.Warnings.AddNew("Cancellation Exception", exception.ToString());
            }

            return cancellationResponse;
        }

        #endregion

        #region "Get cancellation cost"
        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Helper Classes"

        /// <summary>
        /// Creates and returns a AceroomPrebookRequest object
        /// </summary>
        /// <param name="propertyDetails">Thee property details</param>
        /// <returns>a AceroomsPrebookRequest object</returns>
        private AceroomsPrebookRequest CreatePrebookRequest(PropertyDetails propertyDetails)
        {
            List<AceroomsPrebookRequest.Room> thirdpartyRooms = new List<AceroomsPrebookRequest.Room>();
            AceroomsPrebookRequest aceroomsPrebookRequest = new AceroomsPrebookRequest();

            foreach (var room in propertyDetails.Rooms)
            {
                var thirdpartyRoom = new AceroomsPrebookRequest.Room();
                var prebookReference = room.ThirdPartyReference.Split('~'); // split TPReference to get searchToken and room ID

                thirdpartyRoom.RoomID = prebookReference[1];
                thirdpartyRoom.RoomNumber = room.PropertyRoomBookingID;
                thirdpartyRooms.Add(thirdpartyRoom);
            }

            aceroomsPrebookRequest.Rooms = thirdpartyRooms;
            aceroomsPrebookRequest.SearchToken = propertyDetails.Rooms.First().ThirdPartyReference.Split('~')[0]; // Gets the searchtoken out of thirdparty ref(By default the fist element is search token, second is RoomID)

            return aceroomsPrebookRequest;
        }

        /// <summary>
        /// Creates a list of errata for all rooms in the hotel
        /// </summary>
        /// <param name="hotels">The list of hotels</param>
        /// <returns>a list of erratum</returns>
        public Errata GetErrataForAllRooms(List<AceroomsPrebookResponse.HotelDetails> hotels)
        {
            Errata errata = new Errata();

            foreach (var hotel in hotels)
            {
                foreach (var room in hotel.Rooms)
                {
                    errata.Add(new Erratum("Remarks", room.Rate.Remarks));
                }
            }

            return errata;
        }

        /// <summary>
        /// Creates and retuns a list of cancellation details
        /// </summary>
        /// <param name="propertyDetails">The property details</param>
        /// <param name="hotels">The list of hotels</param>
        /// <returns>a lsit of cancellations</returns>
        public Cancellations GetCancellationTermsForAllRooms(PropertyDetails propertyDetails, List<AceroomsPrebookResponse.HotelDetails> hotels)
        {
            Cancellations cancellations = new Cancellations();

            foreach (var hotel in hotels)
            {
                foreach (var room in hotel.Rooms)
                {
                    foreach (var cancelationPlicy in room.Rate.CancelPolicies)
                    {
                        cancellations.Add(new Cancellation(DateTime.Now.Date, propertyDetails.ArrivalDate, cancelationPlicy.Amount));
                    }
                }
            }

            return cancellations;
        }

        /// <summary>
        /// Creates and returns a Aceroom book request 
        /// </summary>
        /// <param name="propertyDetails">The property details</param>
        /// <returns>A Aceroom book request</returns>
        private AceroomsBookRequest CreateBookRequest(PropertyDetails propertyDetails)
        {
            AceroomsBookRequest aceroomRequest = new AceroomsBookRequest();
            List<AceroomsBookRequest.RoomDetails> roomList = new List<AceroomsBookRequest.RoomDetails>();

            foreach (var room in propertyDetails.Rooms)
            {
                roomList.Add(new AceroomsBookRequest.RoomDetails
                {
                    RoomNumber = room.PropertyRoomBookingID,
                    PreBookingToken = room.ThirdPartyReference.Split('~')[2],
                    SpecialRequest = room.SpecialRequest
                });

                List<AceroomsBookRequest.Guest> guests = new List<AceroomsBookRequest.Guest>();

                foreach (var passenger in room.Passengers)
                {
                    guests.Add(GetGuestDetails(passenger));
                }

                roomList.Last().Guests = guests;

            }

            aceroomRequest.Rooms = roomList;
            aceroomRequest.SearchToken = propertyDetails.Rooms.FirstOrDefault().ThirdPartyReference.Split('~')[0];
            aceroomRequest.ClientReference = propertyDetails.BookingReference;
            return aceroomRequest;
        }

        private string GetPassengerType(PassengerType type)
        {
            return type == PassengerType.Adult ? "AD" : "CH";
        }

        private AceroomsBookRequest.Guest GetGuestDetails(Passenger passenger)
        {
            var guest = new AceroomsBookRequest.Guest
            {
                Title = passenger.Title,
                FirstName = passenger.FirstName,
                LastName = passenger.LastName,
                Type = GetPassengerType(passenger.PassengerType)
            };

            if (guest.Type.Equals("CH"))
            {
                guest.Age = passenger.Age;
            }

            return guest;
        }

        /// <summary>
        /// Gets the room booking id from responses
        /// </summary>
        /// <param name="response">The Aceroom book response</param>
        /// <returns>A string with booking ids</returns>
        private string GetRoomBookingIDs(AceroomsBookResponse response)
        {
            StringBuilder bookingIDs = new StringBuilder();

            foreach (var room in response.Booking.Rooms)
            {
                bookingIDs.Append($"{room.BookingRoomID},");
            }

            return bookingIDs.ToString().TrimEnd(','); // remove the excess comma and return 
        }

        /// <summary>
        /// Cancel room booking with supplied booking identifer
        /// </summary>
        /// <param name="roomBookingID">The room booking id</param>
        /// <param name="cancellationResponse">The third party cancellation response</param>
        /// <param name="webRequest">The web request to be sent</param>
        /// <param name="cancelReferences">List of cancellation identifiers</param>
        /// <param name="propertyDetails">The propererty details</param>
        /// <returns>A tuple with a bool indicating sucess of cancellation and cancellation references</returns>
        private bool CancelRoomBooking(string roomBookingID, ref ThirdPartyCancellationResponse cancellationResponse,
                                                        Request webRequest, ref List<string> cancelReferences,
                                                        ref PropertyDetails propertyDetails)
        {
            webRequest.EndPoint += roomBookingID;
            webRequest.Send(_httpClient, _logger).RunSynchronously();

            if (webRequest.ResponseString != "")
            {
                propertyDetails.Logs.AddNew(ThirdParties.ACEROOMS, "Acerooms Cancellation Response ", $"The room with Id:{roomBookingID} wasn't cancelled \n" + webRequest.ResponseString);
            }

            AceroomsCancellationResponse response = JsonConvert.DeserializeObject<AceroomsCancellationResponse>(webRequest.ResponseString);
            cancellationResponse.Amount += response.Booking.CancellationAmount;
            cancelReferences.Add(response.AuditData.CancelRef);

            return response.Booking.Status.Equals("Cancelled");
        }

        /// <summary>
        /// Creates and returns a web request object
        /// </summary>
        /// <param name="propertyDetails">The property details</param>
        /// <param name="url">The url adress</param>
        /// <param name="method">The web request method</param>
        /// <param name="requestString">The request body</param>
        /// <returns>A web request</returns>
        private Request CreateWebRequest(
            ref PropertyDetails propertyDetails,
            string url,
            string logFileName,
            eRequestMethod method,
            string requestString = "")
        {
            IThirdPartyAttributeSearch searchDetails = new SearchDetails
            {
                ThirdPartyConfigurations = propertyDetails.ThirdPartyConfigurations,
            };

            var webRequest = new Request
            {
                Source = ThirdParties.ACEROOMS,
                Method = method,
                UseGZip = true,
                ContentType = ContentTypes.Application_json,
                CreateLog = propertyDetails.CreateLogs,
                LogFileName = logFileName,
                Accept = "application/json",
                EndPoint = _settings.BaseURL(searchDetails) + url
            };

            webRequest.SetRequest(requestString);
            webRequest.Headers.AddNew("APIKey", _settings.APIKey(searchDetails));
            webRequest.Headers.AddNew("Signature", _settings.Signature(searchDetails));

            return webRequest;
        }

        /// <summary>
        /// Match rooms accoding to the propertyRoomBooking id and the pre-book token
        /// </summary>
        /// <param name="propertyDetails">The property details object</param>
        /// <param name="prebookResponse">The Acerooms pre-book response</param>
        private void GetPrebookToken(ref PropertyDetails propertyDetails, AceroomsPrebookResponse prebookResponse)
        {
            foreach (RoomDetails room in propertyDetails.Rooms)
            {
                foreach (AceroomsPrebookResponse.RoomsDetails responseRoom in prebookResponse.Hotels.FirstOrDefault().Rooms)
                {
                    if (responseRoom.RoomNumber == room.PropertyRoomBookingID)
                    {
                        room.ThirdPartyReference += ("~" + responseRoom.Rate.PreBookingToken);
                    }
                }
            }
        }

        #endregion

        #region "End Session"

        public void EndSession(PropertyDetails propertyDetails)
        {

        }

        #endregion
    }
}

