namespace ThirdParty.CSSuppliers.Stuba
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class Stuba : IThirdParty, ISingleSource
    {
        private readonly IStubaSettings _settings;

        private readonly ITPSupport _support;

        private readonly HttpClient _httpClient;

        private readonly ILogger<Stuba> _logger;

        public string Source => ThirdParties.STUBA;

        public Stuba(IStubaSettings settings, ITPSupport support, HttpClient httpClient, ILogger<Stuba> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool result;

            try
            {
                var xml = await SendRequestAsync("Pre-Book", await BookReservationRequestAsync(propertyDetails, false), propertyDetails);
                ExtractErrata(xml, propertyDetails);
                result = CostsAndCancellation(propertyDetails, xml);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBookException", ex.ToString());
                result = false;
            }
            return result;
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string reference = "";

            try
            {
                var xml = await SendRequestAsync("Book", await BookReservationRequestAsync(propertyDetails, true), propertyDetails);

                if (xml.SelectSingleNode("BookingCreateResult/Booking/HotelBooking/Status").InnerText.ToLower() == "confirmed")
                {
                    reference = xml.SelectSingleNode("BookingCreateResult/Booking/Id").InnerText;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("BookException", ex.ToString());
                reference = "Failed";
            }
            return reference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                var xml = await SendRequestAsync("Cancel", CancelRequest(propertyDetails, true), propertyDetails);

                if (xml.SelectSingleNode("BookingCancelResult/Booking/HotelBooking/Status").InnerText.ToLower() == "cancelled")
                {
                    thirdPartyCancellationResponse.TPCancellationReference = xml.SelectSingleNode("BookingCancelResult/Booking/HotelBooking/Id").InnerText;
                    thirdPartyCancellationResponse.Success = true;
                }
            }

            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("CancelException", ex.ToString());
                thirdPartyCancellationResponse.TPCancellationReference = "Failed";
            }
            return thirdPartyCancellationResponse;
        }

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationFeeResult = new ThirdPartyCancellationFeeResult();

            try
            {
                var xml = await SendRequestAsync("Cancellation Charge", CancelRequest(propertyDetails, false), propertyDetails);
                decimal totalCancellationFee = 0m;

                foreach (var room in propertyDetails.Rooms)
                {
                    var lastDate = new DateTime(1900, 1, 1);
                    decimal roomAmount = 0m;
                    var roomNode = xml.SelectSingleNode(string.Format("BookingCancelResult/Booking/HotelBooking/Room[RoomType/@code = '{0}']", room.RoomTypeCode));

                    foreach (XmlNode cancelNode in roomNode.SelectNodes("CanxFees/Fee"))
                    {
                        var dStartDate = cancelNode.SelectSingleNode("@from").InnerText.ToSafeDate();
                        if (dStartDate < DateTime.Now && dStartDate > lastDate)
                        {
                            roomAmount = cancelNode.SelectSingleNode("Amount/@amt").InnerText.ToSafeDecimal();
                        }
                    }
                    totalCancellationFee += roomAmount;
                }

                thirdPartyCancellationFeeResult.Amount = totalCancellationFee;
                thirdPartyCancellationFeeResult.CurrencyCode = propertyDetails.ISOCurrencyCode;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("CancellationChargeException", ex.ToString());
            }

            return thirdPartyCancellationFeeResult;
        }

        private async Task<XmlDocument> SendRequestAsync(string requestType, string xml, PropertyDetails propertyDetails)
        {
            var request = new Request
            {
                EndPoint = _settings.GenericURL(propertyDetails),
                Method = RequestMethod.POST,
                Source = ThirdParties.STUBA,
                LogFileName = requestType,
                CreateLog = true,
                UseGZip = true
            };
            request.SetRequest(xml);

            try
            {
                await request.Send(_httpClient, _logger);
            }
            finally
            {
                propertyDetails.AddLog(requestType, request);
            }

            return request.ResponseXML;
        }

        private async Task<string> BookReservationRequestAsync(PropertyDetails propertyDetails, bool confirm)
        {
            string org = _settings.Organisation(propertyDetails);
            string user = _settings.User(propertyDetails);
            string password = _settings.Password(propertyDetails);
            string version = _settings.Version(propertyDetails);
            string currencyCode = _settings.Currency(propertyDetails);

            EnsurePassengerNamesNotNull(propertyDetails);
            var request = new XElement("BookingCreate",
                            new XElement("Authority",
                                new XElement("Org", org),
                                new XElement("User", user),
                                new XElement("Password", password),
                                new XElement("Currency", currencyCode),
                                new XElement("Version", version)
                            ),
                            new XElement("HotelBooking",
                            new XElement("QuoteId", propertyDetails.Rooms.FirstOrDefault().ThirdPartyReference.Split('|')[1].ToSafeString()),
                            new XElement("HotelStayDetails",
                            new XElement("Nationality", await GetNationalityAsync(propertyDetails)), from oRoom in propertyDetails.Rooms
                                select new XElement("Room",
                                    new XElement("Guests", from oGuest in oRoom.Passengers.Where(p => p.PassengerType == PassengerType.Adult)
                                        select new XElement("Adult", new XAttribute("title", oGuest.Title), new XAttribute("first", oGuest.FirstName), new XAttribute("last", oGuest.LastName)), from oGuest in oRoom.Passengers.Where(p => p.PassengerType != PassengerType.Adult)
                                        select new XElement("Child", new XAttribute("title", oGuest.Title), new XAttribute("first", oGuest.FirstName), new XAttribute("last", oGuest.LastName), new XAttribute("age", oGuest.Age)))))),
                            new XElement("CommitLevel", confirm ? "confirm" : "prepare")
                            );
            return request.ToString();
        }

        private string CancelRequest(PropertyDetails propertyDetails, bool confirm)
        {
            string org = _settings.Organisation(propertyDetails);
            string user = _settings.User(propertyDetails);
            string password = _settings.Password(propertyDetails);
            string version = _settings.Version(propertyDetails);
            string currencyCode = _settings.Currency(propertyDetails);

            var request = new XElement("BookingCancel",
                            new XElement("Authority",
                            new XElement("Org", org),
                            new XElement("User", user),
                            new XElement("Password", password),
                            new XElement("Currency", currencyCode),
                            new XElement("Version", version)),
                                new XElement("BookingId", propertyDetails.SourceReference),
                                new XElement("CommitLevel", confirm ? "confirm" : "prepare")
                            );

            return request.ToString();
        }

        private void EnsurePassengerNamesNotNull(PropertyDetails propertyDetails)
        {
            var rand = new Random();
            int charAsInt = char.ConvertToUtf32("A", 0);
            foreach (var guest in propertyDetails.Rooms.SelectMany(room => room.Passengers))
            {
                if (string.IsNullOrEmpty(guest.Title))
                {
                    guest.Title = rand.Next(2) == 0 ? "Ms" : "Mr";
                }
                if (string.IsNullOrEmpty(guest.FirstName))
                {
                    guest.FirstName = char.ConvertFromUtf32(charAsInt);
                    charAsInt += 1; // names need to be unique
                }
                if (string.IsNullOrEmpty(guest.LastName))
                {
                    guest.LastName = "Test";
                }
            }
        }

        private async Task<string> GetNationalityAsync(PropertyDetails propertyDetails)
        {
            string nationality = string.Empty;
            if (!string.IsNullOrWhiteSpace(propertyDetails.ISONationalityCode))
            {
                nationality = await _support.TPNationalityLookupAsync(ThirdParties.STUBA, propertyDetails.ISONationalityCode);
            }
            if (string.IsNullOrEmpty(nationality))
            {
                nationality = _settings.LeadGuestNationality(propertyDetails);
            }
            return nationality;
        }

        private bool CostsAndCancellation(PropertyDetails propertyDetails, XmlDocument xml)
        {
            bool available = false;

            foreach (var room in propertyDetails.Rooms)
            {
                // Check costs
                var roomNode = xml.SelectSingleNode(string.Format("BookingCreateResult/Booking/HotelBooking/Room[RoomType/@code = '{0}']", room.RoomTypeCode));
                room.LocalCost = roomNode.SelectSingleNode("TotalSellingPrice/@amt").InnerText.ToSafeDecimal();
                room.GrossCost = roomNode.SelectSingleNode("TotalSellingPrice/@amt").InnerText.ToSafeDecimal();

                // Cancellation charges
                foreach (XmlNode cancelNode in roomNode.SelectNodes("CanxFees/Fee"))
                {
                    DateTime startDate;

                    if (cancelNode.SelectSingleNode("@from") is null)
                    {
                        startDate = DateTime.Now.Date;
                    }
                    else
                    {
                        startDate = cancelNode.SelectSingleNode("@from").InnerText.ToSafeDate();
                    }

                    var endDate = new DateTime(2099, 12, 25);

                    var nextStartDate = cancelNode.NextSibling;
                    if (nextStartDate is not null)
                    {
                        endDate = nextStartDate.SelectSingleNode("@from").InnerText.ToSafeDate().AddDays(-1);
                    }

                    decimal amount = cancelNode.SelectSingleNode("Amount/@amt").InnerText.ToSafeDecimal();
                    propertyDetails.Cancellations.AddNew(startDate, endDate, amount);
                }
            }

            propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);

            propertyDetails.Cancellations.Solidify(SolidifyType.Sum);
            available = xml.SelectNodes("BookingCreateResult/Booking/HotelBooking/Room").Count == propertyDetails.Rooms.Count;

            return available;
        }

        private void ExtractErrata(XmlDocument response, PropertyDetails propertyDetails)
        {
            var oErrataNodes = response.SelectNodes("BookingCreateResult/Booking/HotelBooking/Room/Messages/Message[Type = 'Supplier Notes']/Text");
            if (0 is var arg2 && (oErrataNodes?.Count) is { } arg1 && arg1 > arg2)
            {
                foreach (XmlNode oErratum in oErrataNodes)
                {
                    propertyDetails.Errata.AddNew("Important Informations", oErratum.InnerText);
                }
            }
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        #region Stuff

        public bool SupportsRemarks => false;

        public bool SupportsBookingSearch => false;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails, false);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source) => false;

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails oBookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails oPropertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        public string CreateReconciliationReference(string sInputReference)
        {
            return "";
        }

        public void EndSession(PropertyDetails oPropertyDetails)
        {
        }

        #endregion

    }
}