namespace iVectorOne.Suppliers.PremierInn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Suppliers.PremierInn.Models;
    using iVectorOne.Suppliers.PremierInn.Models.Book;
    using iVectorOne.Suppliers.PremierInn.Models.Cancel;
    using iVectorOne.Suppliers.PremierInn.Models.Common;
    using iVectorOne.Suppliers.PremierInn.Models.Search;
    using iVectorOne.Suppliers.PremierInn.Models.Soap;
    using Microsoft.Extensions.Logging;
    using Address = iVectorOne.Suppliers.PremierInn.Models.Common.Address;
    using Erratum = iVectorOne.Models.Property.Booking.Erratum;
    using RoomDetails = iVectorOne.Models.Property.Booking.RoomDetails;

    public class PremierInn : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IPremierInnSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<PremierInn> _logger;
        private readonly ISecretKeeper _secretKeeper;

        public PremierInn(
            IPremierInnSettings settings,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<PremierInn> logger,
            ISecretKeeper secretKeeper)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
        }

        public string Source => ThirdParties.PREMIERINN;
        public bool SupportsRemarks { get; }
        public bool SupportsBookingSearch { get; }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return 0;
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

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Prebook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool preBookSuccess;
            var requests = new List<Request>();

            try
            {
                foreach (var room in propertyDetails.Rooms)
                {
                    var request = BuildAvailabilityUpdateRequest(propertyDetails, room);
                    var webRequest = new Request
                    {
                        EndPoint = _settings.GenericURL(propertyDetails),
                        Method = RequestMethod.POST,
                        ContentType = ContentTypes.Text_xml,
                        SoapAction = Models.Constants.SOAPAction,
                    };
                    webRequest.SetRequest(request);
                    requests.Add(webRequest);

                    await webRequest.Send(_httpClient, _logger);
                }

                var responses = requests
                    .Select(r => Helper.ConvertXmlToString(_serializer.CleanXmlNamespaces(r.ResponseXML), nameof(AvailabilityUpdateResponse)))
                    .Select(x => _serializer.DeSerialize<EnvelopeResponse<AvailabilityUpdateResponse>>(x))
                    .Select(r => r.Body.ProcessMessageResponse.Content.Parameters)
                    .ToList();

                if (responses.All(x => x.ErrorCode.Status == "00"))
                {
                    var cancellations = new Cancellations();

                    foreach (var room in propertyDetails.Rooms)
                    {
                        var tpRef = PremierInnTpRef.Decrypt(_secretKeeper, room.ThirdPartyReference);
                        switch (tpRef.CancellationPolicy.Category)
                        {
                            case 0:
                                cancellations.AddNew(DateTime.Now, propertyDetails.ArrivalDate, 0);
                                break;
                            case 1:
                                cancellations.AddNew(DateTime.Now, propertyDetails.ArrivalDate, room.TotalCost);
                                break;
                            case 2:
                            {
                                var endDate = DateTime.Now.AddDays(tpRef.CancellationPolicy.Days);
                                cancellations.AddNew(DateTime.Now, endDate, 0);
                                cancellations.AddNew(endDate.AddDays(1), propertyDetails.ArrivalDate, room.TotalCost);
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(tpRef.CancellationPolicy.Text))
                        {
                            propertyDetails.Errata.Add(new Erratum("Cancellation", tpRef.CancellationPolicy.Text));
                        }

                        if (tpRef.StatusWarningFlag.Length != 0)
                        {
                            foreach (var warning in tpRef.StatusWarningFlag)
                            {
                                propertyDetails.Errata.Add(new Erratum("StatusWarningFlags", warning));
                            }
                        }
                    }

                    propertyDetails.Cancellations = Cancellations.MergeMultipleCancellationPolicies(cancellations);
                    preBookSuccess = true;
                }
                else
                {
                    preBookSuccess = false;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBook Exception", ex.ToString());
                preBookSuccess = false;
            }
            finally
            {
                if (requests.Any())
                {
                    foreach (var request in requests)
                    {
                        propertyDetails.AddLog("PreBook", request);
                    }
                }
            }

            return preBookSuccess;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string reference;
            var requests = new List<Request>();

            try
            {
                foreach (var room in propertyDetails.Rooms)
                {
                    var request = BuildBookRequest(propertyDetails, room);
                    var webRequest = new Request
                    {
                        EndPoint = _settings.GenericURL(propertyDetails),
                        Method = RequestMethod.POST,
                        ContentType = ContentTypes.Text_xml,
                        SoapAction = Models.Constants.SOAPAction,
                    };
                    webRequest.SetRequest(request);
                    requests.Add(webRequest);

                    await webRequest.Send(_httpClient, _logger);
                }

                var responses = requests
                    .Select(r => Helper.ConvertXmlToString(_serializer.CleanXmlNamespaces(r.ResponseXML), nameof(BookingConfirmResponse)))
                    .Select(x => _serializer.DeSerialize<EnvelopeResponse<BookingConfirmResponse>>(x))
                    .Select(r => r.Body.ProcessMessageResponse.Content.Parameters)
                    .ToList();

                propertyDetails.SourceReference = responses.All(x => x.ErrorCode.Status == "00") 
                    ? "successful" 
                    : "failed";

                if (responses.Any(x => x.ErrorCode.Status == "00"))
                {
                    var references = new List<string>();
                    var sessionIds = new List<string>();

                    foreach (var response in responses)
                    {
                        sessionIds.Add(response.Session.ID);

                        if (response.ErrorCode.Status == "00")
                        {
                            references.Add(response.ConfirmationNumber);
                        }
                        else
                        {
                            references.Add("failed");
                            propertyDetails.Warnings.AddNew("Book Exception", response.ErrorCode.Text);
                        }
                    }

                    propertyDetails.SourceSecondaryReference = string.Join('|', sessionIds);
                    propertyDetails.TPRef1 = new PremierInnTpRef()
                    {
                        LeadGuestLastName = propertyDetails.LeadGuestLastName,
                        ArrivalDate = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd")
                    }.Encrypt(_secretKeeper);
                    reference = string.Join('|', references);

                    var tpRef = new PremierInnTpRef()
                    {
                        LeadGuestLastName = propertyDetails.LeadGuestLastName,
                        ArrivalDate = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd")
                    };
                }
                else
                {
                    reference = "failed";
                    propertyDetails.Warnings.AddNew("Book Exception", "Failed to confirm booking");
                }
                
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                reference = "failed";
            }
            finally
            {
                if (requests.Any())
                {
                    foreach (var request in requests)
                    {
                        propertyDetails.AddLog("Book", request);
                    }
                }
            }

            return reference;
        }

        #endregion

        #region Cancel

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var requests = new List<Request>();
            var updateRequests = new List<Request>();
            var cancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                for (var index = 0; index < propertyDetails.SourceReference.Split('|').Length; index++)
                {
                    if (propertyDetails.SourceReference.Split('|')[index] == "failed")
                    {
                        propertyDetails.Warnings.AddNew("Cancel failed", propertyDetails.SourceSecondaryReference.Split('|')[index]);
                        continue;
                    }
                    
                    var request = BuildCancelRequest(propertyDetails, propertyDetails.SourceReference.Split('|')[index]);
                    var webRequest = new Request
                    {
                        EndPoint = _settings.GenericURL(propertyDetails),
                        Method = RequestMethod.POST,
                        ContentType = ContentTypes.Text_xml,
                        SoapAction = Models.Constants.SOAPAction,
                    };
                    webRequest.SetRequest(request);
                    requests.Add(webRequest);

                    await webRequest.Send(_httpClient, _logger);
                }

                var responses = requests
                    .Select(r => Helper.ConvertXmlToString(_serializer.CleanXmlNamespaces(r.ResponseXML), nameof(ConfirmationNumberValidationResponse)))
                    .Select(x => _serializer.DeSerialize<EnvelopeResponse<ConfirmationNumberValidationResponse>>(x))
                    .Select(r => r.Body.ProcessMessageResponse.Content.Parameters)
                    .ToList();

                if (responses.All(x => string.IsNullOrEmpty(x.ErrorCode.Status) || x.ErrorCode.Status != "00"))
                {
                    cancellationResponse.Success = false;
                    cancellationResponse.TPCancellationReference = "failed";
                    propertyDetails.Warnings.AddNew("Cancellation Exception", responses.First().ErrorCode.Text);
                }
                else
                {
                    for (var index = 0; index < responses.Count; index++)
                    {
                        var request = BuildCancelUpdateRequest(
                            propertyDetails,
                            responses[index].BookerDetails.BookerName,
                            propertyDetails.SourceReference.Split('|')[index],
                            responses[index].Session.ID);

                        var webRequest = new Request
                        {
                            EndPoint = _settings.GenericURL(propertyDetails),
                            Method = RequestMethod.POST,
                            ContentType = ContentTypes.Text_xml,
                            SoapAction = Models.Constants.SOAPAction,
                        };
                        webRequest.SetRequest(request);
                        updateRequests.Add(webRequest);

                        await webRequest.Send(_httpClient, _logger);
                    }

                    var updateResponses = updateRequests
                        .Select(r => Helper.ConvertXmlToString(_serializer.CleanXmlNamespaces(r.ResponseXML), nameof(CancellationUpdateResponse)))
                        .Select(x => _serializer.DeSerialize<EnvelopeResponse<CancellationUpdateResponse>>(x))
                        .Select(r => r.Body.ProcessMessageResponse.Content.Parameters)
                        .ToList();

                    if (updateResponses.All(x => x.ErrorCode.Status != "00"))
                    {
                        cancellationResponse.Success = false;
                        propertyDetails.Warnings.AddNew("Cancellation Exception", updateResponses.First().ErrorCode.Text);
                    }
                    else
                    {
                        var cancellationsList = updateResponses
                            .Select(response => response.ErrorCode.Status == "00" ? response.CancellationNumber : "failed")
                            .ToList();

                        cancellationResponse.Success = true;
                        cancellationResponse.TPCancellationReference = string.Join('|', cancellationsList);
                    }
                }
            }
            catch (Exception ex)
            {
                cancellationResponse.Success = false;
                cancellationResponse.TPCancellationReference = "failed";
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
            }
            finally
            {
                if (requests.Any())
                {
                    foreach (var request in requests)
                    {
                        propertyDetails.AddLog("Cancellation", request);
                    }
                }

                if (updateRequests.Any())
                {
                    foreach (var request in updateRequests)
                    {
                        propertyDetails.AddLog("Cancellation update", request);
                    }
                }
            }

            return cancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        #endregion

        #region Helper

        public string BuildAvailabilityUpdateRequest(PropertyDetails propertyDetails, RoomDetails room)
        {
            var tpRef = PremierInnTpRef.Decrypt(_secretKeeper, room.ThirdPartyReference);
            var request = new AvailabilityUpdateRequest
            {
                Login = Helper.BuildLogin(_settings, propertyDetails),
                Parameters =
                    {
                        Session = { ID = tpRef.SessionId  },
                        MessageType = "AvailabilityUpdateRequest",
                        HotelCode = string.Join(',', propertyDetails.TPKey),
                        StayDateRange =
                        {
                            Start = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                            End = propertyDetails.DepartureDate.ToString("yyyy-MM-dd")
                        },
                        RatePlan = {RatePlanCode = tpRef.RateCode },
                        Rooms =
                        {
                            NumberofRooms = 1,
                            RoomDetails =
                            {
                                RoomType = room.RoomTypeCode,
                                Number = 1,
                                Adults = room.Adults,
                                Children = room.Children + room.Infants,
                                Cots = room.Infants != 0 ? "Yes" : "No",
                                Double = (room.Children == 0 && room.Infants < 2) ? "Yes" : "No",
                                Disabled = "No"
                            }
                        }
                    }
            };

            var content = _serializer.CleanXmlNamespaces(_serializer.Serialize(request)).InnerXml
                .Replace("<AvailabilityUpdateRequest>", "")
                .Replace("</AvailabilityUpdateRequest>", "");

            return Helper.CreateEnvelope(_serializer, content);
        }

        public string BuildBookRequest(PropertyDetails propertyDetails, RoomDetails room)
        {
            var tpRef = PremierInnTpRef.Decrypt(_secretKeeper, room.ThirdPartyReference);
            var card = propertyDetails.GeneratedVirtualCard;
            var request = new BookingConfirmRequest
            {
                Login = Helper.BuildLogin(_settings, propertyDetails),
                Parameters =
                {
                    Session = { ID = tpRef.SessionId },
                    PaymentCard =
                    {
                        CardType = "VI",
                        CardNumber = !string.IsNullOrEmpty(card.CardNumber) ? card.CardNumber : "4444333322221111",
                        ExpiryDate = !string.IsNullOrEmpty(card.ExpiryMonth) ? card.ExpiryMonth + card.ExpiryYear : "0225",
                        CardHolderName = !string.IsNullOrEmpty(card.CardHolderName) 
                            ? card.CardHolderName 
                            : $"{propertyDetails.LeadGuestFirstName} {propertyDetails.LeadGuestLastName}",
                    },
                    MessageType = "BookingConfirmRequest",
                    Rooms =
                    {
                        NumberofRooms = 1,
                        RoomDetails =
                        {
                            Number = 1,
                            GuestName = room.Passengers.Select(x => new GuestName
                            {
                                Title = x.Title,
                                Initials = x.FirstName[..1],
                                Surname = x.LastName
                            }).ToArray(),
                        }
                    },
                    Address =
                    {
                        AddressLine1 = propertyDetails.LeadGuestAddress1,
                        AddressLine2 = propertyDetails.LeadGuestTownCity,
                        AddressLine3 = propertyDetails.LeadGuestCounty,
                        AddressLine4 = propertyDetails.LeadGuestCountryCode,
                        PostCode = propertyDetails.LeadGuestPostcode,
                        TelephoneNumber = propertyDetails.LeadGuestPhone,
                        MobileNumber = propertyDetails.LeadGuestMobile,
                        EmailAddress = propertyDetails.LeadGuestEmail,
                    },
                    BookingCompanyName = "iVectorOne",
                    BookerDetails =
                    {
                        BookerName =
                        {
                            Title = propertyDetails.LeadGuestTitle,
                            Initials = propertyDetails.LeadGuestFirstName[..1],
                            Surname = propertyDetails.LeadGuestLastName
                        },
                        BookerAddress = new Address
                        {
                            AddressLine1 = propertyDetails.LeadGuestAddress1,
                            AddressLine2 = propertyDetails.LeadGuestTownCity,
                            AddressLine3 = propertyDetails.LeadGuestCounty,
                            AddressLine4 = propertyDetails.LeadGuestCountryCode,
                            PostCode = propertyDetails.LeadGuestPostcode,
                            TelephoneNumber = propertyDetails.LeadGuestPhone,
                            MobileNumber = propertyDetails.LeadGuestMobile,
                            EmailAddress = propertyDetails.LeadGuestEmail
                        }
                    },
                    BookingType = "Leisure",
                    SpecialRequirements =
                    {
                        Text = room.SpecialRequest
                    }
                }
            };
            
            var content = _serializer.CleanXmlNamespaces(_serializer.Serialize(request)).InnerXml
                .Replace("<BookingConfirmRequest>", "")
                .Replace("</BookingConfirmRequest>", "");

            return Helper.CreateEnvelope(_serializer, content);
        }

        public string BuildCancelRequest(PropertyDetails propertyDetails, string number)
        {
            var tpRef = PremierInnTpRef.Decrypt(_secretKeeper, propertyDetails.TPRef1);
            var request = new ConfirmationNumberValidationRequest
            {
                Login = Helper.BuildLogin(_settings, propertyDetails),
                Parameters =
                {
                    MessageType = "ConfirmationNumberValidationRequest",
                    Route = "Cancel",
                    ConfirmationNumber = number,
                    ArrivalDate = tpRef.ArrivalDate,
                    Surname = tpRef.LeadGuestLastName
                }
            };

            var content = _serializer.CleanXmlNamespaces(_serializer.Serialize(request)).InnerXml
                .Replace("<ConfirmationNumberValidationRequest>", "")
                .Replace("</ConfirmationNumberValidationRequest>", "");

            return Helper.CreateEnvelope(_serializer, content);
        }

        public string BuildCancelUpdateRequest(
            PropertyDetails propertyDetails,
            GuestName bookerName,
            string number,
            string sessionId)
        {
            var request = new CancellationUpdateRequest()
            {
                Login = Helper.BuildLogin(_settings, propertyDetails),
                Parameters =
                {
                    MessageType = "CancellationUpdateRequest",
                    Session = { ID = sessionId },
                    ConfirmationNumber = number,
                    BookerDetails =
                    {
                        BookerName = bookerName,
                    }
                }
            };

            var content = _serializer.CleanXmlNamespaces(_serializer.Serialize(request)).InnerXml
                .Replace("<CancellationUpdateRequest>", "")
                .Replace("</CancellationUpdateRequest>", "");

            return Helper.CreateEnvelope(_serializer, content);
        }
    }

    #endregion
}