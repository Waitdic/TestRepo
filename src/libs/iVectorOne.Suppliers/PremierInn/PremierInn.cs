﻿using System;
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
using iVectorOne.Models.Property.Booking;
using iVectorOne.Suppliers.PremierInn.Models;
using iVectorOne.Suppliers.PremierInn.Models.Book;
using iVectorOne.Suppliers.PremierInn.Models.Common;
using iVectorOne.Suppliers.PremierInn.Models.Search;
using iVectorOne.Suppliers.PremierInn.Models.Soap;
using Microsoft.Extensions.Logging;
using Address = iVectorOne.Suppliers.PremierInn.Models.Common.Address;
using Erratum = iVectorOne.Models.Property.Booking.Erratum;
using RoomDetails = iVectorOne.Models.Property.Booking.RoomDetails;

namespace iVectorOne.Suppliers.PremierInn
{
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

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            var preBookSuccess = false;
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
                    reference = string.Join('|', references);
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

        public Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

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
                                Number = room.Children + room.Infants + room.Adults,
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
                        CardNumber = "4444333322221111",
                        ExpiryDate = "0225",
                        CardHolderName = $"{propertyDetails.LeadGuestFirstName} {propertyDetails.LeadGuestLastName}",
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
                }
            };
            
            var content = _serializer.CleanXmlNamespaces(_serializer.Serialize(request)).InnerXml
                .Replace("<BookingConfirmRequest>", "")
                .Replace("</BookingConfirmRequest>", "");

            return Helper.CreateEnvelope(_serializer, content);
        }
    }
}
