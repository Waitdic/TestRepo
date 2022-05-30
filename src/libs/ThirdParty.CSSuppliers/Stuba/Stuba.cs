namespace ThirdParty.CSSuppliers.Stuba
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Xml;
    using System.Xml.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class Stuba : IThirdParty
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

        public bool PreBook(PropertyDetails oPropertyDetails)
        {
            bool bReturn;

            try
            {
                var oXML = SendRequest("Pre-Book", BookReservationRequest(oPropertyDetails, false), oPropertyDetails);
                ExtractErrata(oXML, oPropertyDetails);
                bReturn = CostsAndCancellation(oPropertyDetails, oXML);
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("PreBookException", ex.ToString());
                bReturn = false;
            }
            return bReturn;
        }

        public string Book(PropertyDetails oPropertyDetails)
        {
            string sReference = "";

            try
            {
                var oXML = SendRequest("Book", BookReservationRequest(oPropertyDetails, true), oPropertyDetails);

                if (oXML.SelectSingleNode("BookingCreateResult/Booking/HotelBooking/Status").InnerText.ToLower() == "confirmed")
                {
                    sReference = oXML.SelectSingleNode("BookingCreateResult/Booking/Id").InnerText;
                }
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("BookException", ex.ToString());
                sReference = "Failed";
            }
            return sReference;
        }

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails oPropertyDetails)
        {
            var oThirdPartyCancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                var oXML = SendRequest("Cancel", CancelRequest(oPropertyDetails, true), oPropertyDetails);

                if (oXML.SelectSingleNode("BookingCancelResult/Booking/HotelBooking/Status").InnerText.ToLower() == "cancelled")
                {
                    oThirdPartyCancellationResponse.TPCancellationReference = oXML.SelectSingleNode("BookingCancelResult/Booking/HotelBooking/Id").InnerText;
                    oThirdPartyCancellationResponse.Success = true;
                }
            }

            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("CancelException", ex.ToString());
                oThirdPartyCancellationResponse.TPCancellationReference = "Failed";
            }
            return oThirdPartyCancellationResponse;
        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails oPropertyDetails)
        {
            var oThirdPartyCancellationFeeResult = new ThirdPartyCancellationFeeResult();

            try
            {
                var oXML = SendRequest("Cancellation Charge", CancelRequest(oPropertyDetails, false), oPropertyDetails);
                decimal nTotalCancellationFee = 0m;

                foreach (RoomDetails oRoom in oPropertyDetails.Rooms)
                {
                    var dLastDate = new DateTime(1900, 1, 1);
                    decimal nRoomAmount = 0m;
                    var oRoomNode = oXML.SelectSingleNode(string.Format("BookingCancelResult/Booking/HotelBooking/Room[RoomType/@code = '{0}']", oRoom.RoomTypeCode));

                    foreach (XmlNode oCXL in oRoomNode.SelectNodes("CanxFees/Fee"))
                    {
                        var dStartDate = oCXL.SelectSingleNode("@from").InnerText.ToSafeDate();
                        if (dStartDate < DateTime.Now && dStartDate > dLastDate)
                        {
                            nRoomAmount = oCXL.SelectSingleNode("Amount/@amt").InnerText.ToSafeDecimal();
                        }
                    }
                    nTotalCancellationFee += nRoomAmount;
                }

                oThirdPartyCancellationFeeResult.Amount = nTotalCancellationFee;
                oThirdPartyCancellationFeeResult.CurrencyCode = oPropertyDetails.CurrencyCode;
            }

            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("CancellationChargeException", ex.ToString());
            }

            return oThirdPartyCancellationFeeResult;
        }

        private XmlDocument SendRequest(string RequestType, string XML, PropertyDetails PropertyDetails)
        {

            var oRequest = new Request();
            oRequest.EndPoint = _settings.get_URL(PropertyDetails);
            oRequest.SetRequest(XML);
            oRequest.Method = eRequestMethod.POST;
            oRequest.Source = ThirdParties.STUBA;
            oRequest.LogFileName = RequestType;
            oRequest.CreateLog = true;
            oRequest.UseGZip = true;
            oRequest.Send(_httpClient, _logger);

            if (!string.IsNullOrEmpty(oRequest.RequestLog))
            {
                PropertyDetails.Logs.AddNew(ThirdParties.STUBA, "Stuba " + RequestType + " Request", oRequest.RequestLog);
            }

            if (!string.IsNullOrEmpty(oRequest.ResponseLog))
            {
                PropertyDetails.Logs.AddNew(ThirdParties.STUBA, "Stuba " + RequestType + " Response", oRequest.ResponseLog);
            }

            return oRequest.ResponseXML;
        }

        private string BookReservationRequest(PropertyDetails oPropertyDetails, bool Confirm)
        {

            string sOrg = _settings.get_Organisation(oPropertyDetails);
            string sUser = _settings.get_Username(oPropertyDetails);
            string sPassword = _settings.get_Password(oPropertyDetails);
            string sVersion = _settings.get_Version(oPropertyDetails);
            string sCurrencyCode = _settings.get_Currency(oPropertyDetails);

            EnsurePassengerNamesNotNull(oPropertyDetails);
            var oRequest = new XElement("BookingCreate",
                            new XElement("Authority",
                                new XElement("Org", sOrg),
                                new XElement("User", sUser),
                                new XElement("Password", sPassword),
                                new XElement("Currency", sCurrencyCode),
                                new XElement("Version", sVersion)
                            ),
                            new XElement("HotelBooking",
                            new XElement("QuoteId", oPropertyDetails.Rooms.FirstOrDefault().ThirdPartyReference.Split('|')[1].ToSafeString()),
                            new XElement("HotelStayDetails",
                            new XElement("Nationality", GetNationality(oPropertyDetails)), from oRoom in oPropertyDetails.Rooms
                                select new XElement("Room",
                                    new XElement("Guests", from oGuest in oRoom.Passengers.Where(p => p.PassengerType == PassengerType.Adult)
                                        select new XElement("Adult", new XAttribute("title", oGuest.Title), new XAttribute("first", oGuest.FirstName), new XAttribute("last", oGuest.LastName)), from oGuest in oRoom.Passengers.Where(p => p.PassengerType != PassengerType.Adult)
                                        select new XElement("Child", new XAttribute("title", oGuest.Title), new XAttribute("first", oGuest.FirstName), new XAttribute("last", oGuest.LastName), new XAttribute("age", oGuest.Age)))))),
                            new XElement("CommitLevel", Confirm ? "confirm" : "prepare")
                            );
            return oRequest.ToString();
        }

        private string CancelRequest(PropertyDetails oPropertyDetails, bool bConfirm)
        {
            string sOrg = _settings.get_Organisation(oPropertyDetails);
            string sUser = _settings.get_Username(oPropertyDetails);
            string sPassword = _settings.get_Password(oPropertyDetails);
            string sVersion = _settings.get_Version(oPropertyDetails);
            string sCurrencyCode = _settings.get_Currency(oPropertyDetails);

            var oRequest = new XElement("BookingCancel",
    new XElement("Authority",
    new XElement("Org", sOrg),
    new XElement("User", sUser),
    new XElement("Password", sPassword),
    new XElement("Currency", sCurrencyCode),
    new XElement("Version", sVersion)
    ),
    new XElement("BookingId", oPropertyDetails.SourceReference),
    new XElement("CommitLevel", bConfirm ? "confirm" : "prepare")
    );

            return oRequest.ToString();
        }

        private void EnsurePassengerNamesNotNull(PropertyDetails propertyDetails)
        {
            var rand = new Random();
            int charAsInt = char.ConvertToUtf32("A", 0);
            foreach (Passenger guest in propertyDetails.Rooms.SelectMany(room => room.Passengers))
            {
                if (string.IsNullOrEmpty(guest.Title))
                {
                    /* TODO ERROR: Skipped WarningDirectiveTrivia
                    #Disable Warning SCS0005 ' Weak random generator
                    */
                    guest.Title = rand.Next(2) == 0 ? "Ms" : "Mr";
                    /* TODO ERROR: Skipped WarningDirectiveTrivia
                    #Enable Warning SCS0005 ' Weak random generator
                    */
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

        private string GetNationality(PropertyDetails propertyDetails)
        {
            string sNationality = "";
            if (propertyDetails.LeadGuestNationalityID != 0)
            {
                sNationality = _support.TPNationalityLookup(ThirdParties.STUBA, propertyDetails.LeadGuestNationalityID);
            }
            if (string.IsNullOrEmpty(sNationality))
            {
                sNationality = _settings.get_Nationality(propertyDetails);
            }
            return sNationality;
        }

        private bool CostsAndCancellation(PropertyDetails oPropertyDetails, XmlDocument oXML)
        {
            bool bAvailable = false;

            foreach (RoomDetails oRoom in oPropertyDetails.Rooms)
            {

                // Check costs
                var oRoomNode = oXML.SelectSingleNode(string.Format("BookingCreateResult/Booking/HotelBooking/Room[RoomType/@code = '{0}']", oRoom.RoomTypeCode));
                oRoom.LocalCost = oRoomNode.SelectSingleNode("TotalSellingPrice/@amt").InnerText.ToSafeDecimal();
                oRoom.GrossCost = oRoomNode.SelectSingleNode("TotalSellingPrice/@amt").InnerText.ToSafeDecimal();

                // Cancellation charges
                foreach (XmlNode oCXL in oRoomNode.SelectNodes("CanxFees/Fee"))
                {
                    DateTime dStartDate;

                    if (oCXL.SelectSingleNode("@from") is null)
                    {
                        dStartDate = DateTime.Now.Date;
                    }
                    else
                    {
                        dStartDate = oCXL.SelectSingleNode("@from").InnerText.ToSafeDate();
                    }

                    var dEndDate = new DateTime(2099, 12, 25);

                    var oNextStartDate = oCXL.NextSibling;
                    if (oNextStartDate is not null)
                    {
                        dEndDate = oNextStartDate.SelectSingleNode("@from").InnerText.ToSafeDate().AddDays(-1);
                    }

                    decimal nAmount = oCXL.SelectSingleNode("Amount/@amt").InnerText.ToSafeDecimal();
                    oPropertyDetails.Cancellations.AddNew(dStartDate, dEndDate, nAmount);
                }

            }

            oPropertyDetails.LocalCost = oPropertyDetails.Rooms.Sum(r => r.LocalCost);

            oPropertyDetails.Cancellations.Solidify(SolidifyType.Sum);
            bAvailable = oXML.SelectNodes("BookingCreateResult/Booking/HotelBooking/Room").Count == oPropertyDetails.Rooms.Count;

            return bAvailable;
        }

        private void ExtractErrata(XmlDocument oResponse, PropertyDetails oPropertyDetails)
        {
            var oErrataNodes = oResponse.SelectNodes("BookingCreateResult/Booking/HotelBooking/Room/Messages/Message[Type = 'Supplier Notes']/Text");
            if (0 is var arg2 && (oErrataNodes?.Count) is { } arg1 && arg1 > arg2)
            {
                foreach (XmlNode oErratum in oErrataNodes)
                    oPropertyDetails.Errata.AddNew("Important Informations", oErratum.InnerText);
            }
        }

        private bool IThirdParty_SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.get_AllowCancellations(searchDetails);
        }

        bool IThirdParty.SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source) => IThirdParty_SupportsLiveCancellation(searchDetails, source);

        #region Stuff

        public bool SupportsRemarks
        {
            get
            {
                return false;
            }
        }

        public bool SupportsBookingSearch
        {
            get
            {
                return false;
            }
        }

        private bool IThirdParty_TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails)
        {
            return false;
        }

        bool IThirdParty.TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails) => IThirdParty_TakeSavingFromCommissionMargin(searchDetails);

        private int IThirdParty_OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.get_OffsetCancellationDays(searchDetails, false);
        }

        int IThirdParty.OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails) => IThirdParty_OffsetCancellationDays(searchDetails);

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

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