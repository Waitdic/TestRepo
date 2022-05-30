namespace ThirdParty.CSSuppliers.Helpers.W2M
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using iVector.Search.Property;
    using ThirdParty.Models;
    using ThirdParty.CSSuppliers.Models.W2M;
    using ThirdParty.CSSuppliers.Xml.W2M;

    internal class SoapRequestXmlBuilder
    {
        private const string DocumentType = "DNI";

        private readonly ISerializer _serializer;

        public SoapRequestXmlBuilder(ISerializer serializer)
        {
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #region Availability

        internal string BuildAvailabilityRequest(BaseRequestParameters parameters, string startDate,
            string endDate, RoomDetail room, string leadGuestNationality, IEnumerable<string> hotelCodes)
        {
            var paxes = BuildPaxes(room);

            var hotelAvailability = new HotelAvailability(
                new HotelAvailabilityRequest(
                    new Login(parameters.Username, parameters.Password),
                    new Paxes(paxes),
                    new HotelRequest(
                        new SearchSegmentsHotels(
                            new SearchSegmentHotels(startDate, endDate),
                            new HotelCodes(hotelCodes.ToList()),
                            leadGuestNationality),
                        new RelativePaxesDistribution(
                            new List<RelativePaxDistribution>
                            {
                                BuildRelativePaxDistribution(paxes)
                            })),
                    new AvailabilityAdvancedOptions
                    {
                        ShowHotelInfo = true,
                        ShowOnlyBestPriceCombination = false,
                        ShowCancellationPolicies = true,
                        TimeOut = 8000
                    },
                    Constants.SoapVersion,
                    parameters.Language),
                parameters.SoapPrefix);

            var xml = _serializer.CleanXmlNamespaces(_serializer.Serialize(hotelAvailability));
            return AddSoapEnvelopeNodes(xml.OuterXml, parameters.SoapPrefix);
        }

        private static List<Pax> BuildPaxes(RoomDetail room)
        {
            var paxList = new List<Pax>();
            var paxCounter = 1;

            for (var i = 0; i < room.Adults; i++)
            {
                paxList.Add(new Pax
                {
                    IdPax = paxCounter
                });
                paxCounter++;
            }

            for (var i = 0; i < room.Children; i++)
            {
                paxList.Add(new Pax
                {
                    IdPax = paxCounter,
                    Age = room.ChildAges[i].ToString()
                });
                paxCounter++;
            }

            for (var i = 0; i < room.Infants; i++)
            {
                paxList.Add(new Pax
                {
                    IdPax = paxCounter,
                    Age = "1"
                });
                paxCounter++;
            }

            return paxList;
        }

        #endregion

        #region Prebook

        internal string BuildAvailabilityCheck(PreBookRequestParameters parameters, string ratePlanCode)
        {
            var login = new Login(parameters.BaseRequestParameters.Username,
                parameters.BaseRequestParameters.Password);

            var startDate = parameters.ArrivalDate.ToString(Constants.DateTimeFormat);
            var endDate = parameters.DepartureDate.ToString(Constants.DateTimeFormat);

            var hotelOption = new CheckAvailabilityHotelOption(ratePlanCode);
            var searchSegmentsHotels = new SearchSegmentsHotels(
                new SearchSegmentHotels(startDate, endDate),
                new HotelCodes(new List<string> { parameters.HotelCode }),
                parameters.LeadGuestNationality);

            var checkAvailabilityRequest = new CheckAvailabilityRequest(hotelOption, searchSegmentsHotels);

            var availabilityRequest = new AvailabilityRequest(login, checkAvailabilityRequest, Constants.SoapVersion,
                parameters.BaseRequestParameters.Language);

            var availabilityCheck = new AvailabilityCheck(availabilityRequest);

            var xml = _serializer.CleanXmlNamespaces(_serializer.Serialize(availabilityCheck));
            return AddSoapEnvelopeNodes(xml.OuterXml, parameters.BaseRequestParameters.SoapPrefix);
        }

        internal string BuildBookingRules(PreBookRequestParameters parameters, string ratePlanCode)
        {
            var startDate = parameters.ArrivalDate.ToString(Constants.DateTimeFormat);
            var endDate = parameters.DepartureDate.ToString(Constants.DateTimeFormat);

            var hotelBookingRules = new HotelBookingRules(
                new HotelBookingRulesRequestWrapper(
                    new Login(parameters.BaseRequestParameters.Username, parameters.BaseRequestParameters.Password),
                    new HotelBookingRulesRequest(
                        new RequestHotelOption(ratePlanCode),
                        new SearchSegmentsHotels(
                            new SearchSegmentHotels(startDate, endDate),
                            new HotelCodes(new List<string> { parameters.HotelCode }),
                            parameters.LeadGuestNationality)),
                    Constants.SoapVersion,
                    parameters.BaseRequestParameters.Language));

            var xml = _serializer.CleanXmlNamespaces(_serializer.Serialize(hotelBookingRules));
            return AddSoapEnvelopeNodes(xml.OuterXml, parameters.BaseRequestParameters.SoapPrefix);
        }

        #endregion

        #region Book

        internal string BuildHotelBookingRequest(BookRequestParameters parameters, string bookingCode,
            ThirdParty.Models.Property.Booking.RoomDetails room)
        {
            var paxList = new List<Pax> { BuildFirstPax(parameters.LeadGuest, room.Passengers) };

            paxList.AddRange(GetAdditionalPax(room));

            var comments = new Comments();
            if (!string.IsNullOrWhiteSpace(parameters.HotelRequest))
            {
                comments = new Comments(new List<Comment>
                {
                    new Comment(Constants.Comments.GeneralBookingCommentType, parameters.HotelRequest)
                });
            }

            var hotelBooking = new HotelBooking(
                new BookingRequest(
                    new Login(parameters.BaseParameters.Username, parameters.BaseParameters.Password),
                    new Paxes(paxList),
                    new Holder(new RelativePax(1)),
                    string.IsNullOrWhiteSpace(parameters.ExternalReference)
                        ? string.Empty : parameters.ExternalReference,
                    comments,
                    new Elements(
                        new HotelElement(bookingCode,
                            new RelativePaxesDistribution(
                                new List<RelativePaxDistribution>
                                {
                                            BuildRelativePaxDistribution(paxList)
                                }),
                            new HotelBookingInfo(
                                new Price(new PriceRange(0, decimal.MaxValue, parameters.Currency)),
                                parameters.HotelCode,
                                parameters.StartDate,
                                parameters.EndDate),
                            comments)),
                    Constants.SoapVersion,
                    parameters.BaseParameters.Language));

            var xml = _serializer.CleanXmlNamespaces(_serializer.Serialize(hotelBooking));
            return AddSoapEnvelopeNodes(xml.OuterXml, parameters.BaseParameters.SoapPrefix, false);
        }
        private static Pax BuildFirstPax(LeadGuest leadGuest, Passengers passengers)
        {
            var passenger = passengers.First();

            var pax = new Pax
            {
                IdPax = 1,
                Name = passenger.FirstName,
                Surname = passenger.LastName,
                Age = passenger.Age > 0 ? passenger.Age.ToString() : string.Empty,
                PhoneNumbers = new PhoneNumbers { PhoneNumber = { Text = leadGuest.Phone } },
                Address = leadGuest.Address,
                City = leadGuest.City,
                Country = leadGuest.Country,
                Email = leadGuest.Email,
                Nationality = leadGuest.Nationality,
                PostalCode = leadGuest.PostCode,
            };

            if (!string.IsNullOrWhiteSpace(leadGuest.PassportNumber))
            {
                pax.Document = new Document { Type = DocumentType, Text = leadGuest.PassportNumber };
            }

            return pax;
        }

        private static IEnumerable<Pax> GetAdditionalPax(ThirdParty.Models.Property.Booking.RoomDetails room)
        {
            for (int i = 2; i <= room.Passengers.Count; i++)
            {
                var passenger = room.Passengers[i - 1];
                yield return new Pax
                {
                    Name = passenger.FirstName,
                    Surname = passenger.LastName,
                    Age = passenger.Age > 0 || passenger.PassengerType == PassengerType.Infant ? passenger.Age.ToString() : string.Empty,
                    IdPax = i
                };
            }
        }

        #endregion

        #region Cancel

        internal string BuildCancellation(BaseRequestParameters parameters, string reservationLocator)
        {
            var cancelBooking = new CancelBookingRequest(
                new CancellationRequest(
                    new Login(parameters.Username, parameters.Password),
                    new CancelRequest(reservationLocator),
                    new CancellationAdvancedOptions
                    {
                        ShowBreakdownPrice = false,
                        ShowCompleteInfo = false,
                        ShowHotelInfo = false,
                        ShowOnlyBasicInfo = false,
                        OnlyCancellationFees = true,
                        ShowOnlyBestPriceCombination = false,
                        TimeOut = 0
                    },
                    Constants.SoapVersion,
                    parameters.Language));

            var xml = _serializer.CleanXmlNamespaces(_serializer.Serialize(cancelBooking));
            return AddSoapEnvelopeNodes(xml.OuterXml, parameters.SoapPrefix);
        }

        #endregion

        #region Common Functions

        private static string AddSoapEnvelopeNodes(string inputXml, string xmlNamespace, bool includeHeader = true)
        {
            var sb = new StringBuilder();

            sb.Append(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""");
            sb.Append($@" xmlns=""{xmlNamespace}"">");

            if (includeHeader)
            {
                sb.Append("<soapenv:Header/>");
            }

            sb.Append("<soapenv:Body>");
            sb.Append(inputXml);
            sb.Append(@"</soapenv:Body>
                             </soapenv:Envelope>");

            return sb.ToString();
        }

        private static RelativePaxDistribution BuildRelativePaxDistribution(IEnumerable<Pax> paxList)
        {
            var relativePaxList = new List<RelativePax>();

            foreach (var pax in paxList)
            {
                relativePaxList.Add(new RelativePax(pax.IdPax));
            }

            return new RelativePaxDistribution(new RelativePaxes(relativePaxList));
        }

        #endregion
    }
}
