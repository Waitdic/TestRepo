
namespace iVectorOne.Suppliers.HBSi
{
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Suppliers.HBSi.Models;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    public class HBSi : IThirdParty, IMultiSource
    {
        #region "Properties"

        public List<string> Sources => Constant.HBSiSources;

        private readonly IHBSiSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HBSi> _logger;
        private readonly ISecretKeeper _secretKeeper;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails, source);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails, source);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return string.Equals(_settings.PaymentMethod(info, source).ToLower(), "vcard");
        }

        #endregion

        #region "Constructors"
        public HBSi(
            IHBSiSettings settings, 
            ITPSupport support, 
            ISerializer serializer, 
            HttpClient httpClient, 
            ILogger<HBSi> logger, 
            ISecretKeeper secretKeeper)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
        }

        #endregion

        #region "Prebook"

        public async Task<bool> PreBookAsync(PropertyDetails oPropertyDetails)
        {
            var source = oPropertyDetails.Source;
            bool bResult = true;
            bool bPriceChanged = false;
            var oRequest = new Request();
            var bUsePassangerAge = _settings.UsePassengerAge(oPropertyDetails, source);

            try
            {
                var xmlRequest = BuildAvailabilityRequest(oPropertyDetails, HBSiHelper.Now());
                oRequest = await SendRequest(oPropertyDetails, xmlRequest, "AvailabilityCheck");
                var oPreBookRS = Envelope<OtaHotelAvailRs>.DeSerialize(_serializer, oRequest.ResponseXML);

                var roomStays = oPreBookRS.RoomStays.SelectMany(oRoomStay =>
                {
                    int numberOfUnit = oRoomStay.RoomTypes.First().NumberOfUnits;
                    return Enumerable.Range(1, numberOfUnit).Select(_ => new
                    {
                        RoomStay = oRoomStay,
                        RoomTypeCode = HBSiHelper.GetRoomIdentyCode(oRoomStay, bUsePassangerAge)
                    });
                }).ToList();

                var oRooms = oPropertyDetails.Rooms.Select(oRoom =>
                {
                    var roomStayCodePair = roomStays.FirstOrDefault(x => string.Equals(oRoom.ThirdPartyReference, x.RoomTypeCode));
                    if (roomStayCodePair != null)
                    {
                        roomStays.Remove(roomStayCodePair);
                        roomStayCodePair.RoomStay.RoomTypes.First().NumberOfUnits = 1;
                    }

                    return new
                    {
                        RoomDetails = oRoom,
                        RoomIdentityCode = oRoom.ThirdPartyReference,
                        RoomStay = roomStayCodePair.RoomStay
                    };
                }).ToList();

                foreach (var oRoom in oRooms)
                {
                    if (oRoom.RoomStay == null)
                    {
                        // 'no available room found or room sold out
                        return false;
                    }
                    // 'update cost
                    decimal roomTotal = oRoom.RoomStay.Total.AmountAfterTax.ToSafeDecimal();
                    if (oRoom.RoomDetails.LocalCost != roomTotal)
                    {
                        oRoom.RoomDetails.LocalCost = roomTotal;
                        oRoom.RoomDetails.GrossCost = roomTotal;
                        bPriceChanged = true;
                    }

                    var tpReference = _serializer.Serialize(oRoom.RoomStay).OuterXml;
                    tpReference = tpReference.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                    oRoom.RoomDetails.ThirdPartyReference = _secretKeeper.Encrypt(tpReference);
                };

                // 'price change warning
                if (bPriceChanged)
                {
                    oPropertyDetails.Warnings.AddNew("Third Party / Prebook Price Changed", "Price Changed");
                }

                // 'get cancellations
                var oCancellations = GetCancellations(oPreBookRS.RoomStays, oPropertyDetails.ArrivalDate);
                oPropertyDetails.Cancellations = oCancellations;

                // 'add in any additional taxes as errata
                var oRateNodes = oPreBookRS.RoomStays.SelectMany(rs => rs.RoomRates.SelectMany(rr => rr.Rates));

                decimal nAdditionalTax = oRateNodes.SelectMany(rate => rate.Taxes)
                                            .Where(tax => string.Equals(tax.TaxType, "Exclusive"))
                                            .Select(tax => tax.Amount.ToSafeDecimal()).Sum();
                if (nAdditionalTax > 0)
                {
                    oPropertyDetails.Errata.AddNew("Additional Taxes", $"Additional taxes totalling {nAdditionalTax} not included in rate");
                }

                // 'supplier specific errata
                if (!string.IsNullOrEmpty(_settings.SupplierErrata(oPropertyDetails, source)))
                {
                    oPropertyDetails.Errata.AddNew("Important", _settings.SupplierErrata(oPropertyDetails, source));
                }

                // 'build up property tp reference
                var basicInfo = oPreBookRS.RoomStays.First().BasicPropertyInfo;
                oPropertyDetails.TPRef1 = $"{basicInfo.ChainCode}|{basicInfo.BrandCode}";
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("PreBookException", ex.Message);
                bResult = false;
            }
            finally
            {
                oPropertyDetails.AddLog("Prebook", oRequest);
            }

            return bResult;
        }

        private Cancellations GetCancellations(List<RoomStay> oRoomStays, DateTime dPropertyArrivalDate)
        {
            string[] oPolicyCodes = { "CXP", "CFC" };

            var oCnxs = oRoomStays.SelectMany(oRoomStay =>
            {
                var penalties = oRoomStay.CancelPenalties
                .Where(oPenalty => oPolicyCodes.Contains(oPenalty.PolicyCode))
                .Select(oPenalty =>
                {
                    decimal amount = 0.00M;
                    if (!string.IsNullOrEmpty(oPenalty.AmountPercent.Amount))
                    {
                        amount = oPenalty.AmountPercent.Amount.ToSafeDecimal();
                    }
                    else if (!string.IsNullOrEmpty(oPenalty.AmountPercent.Percent))
                    {
                        amount = oRoomStay.Total.AmountAfterTax.ToSafeDecimal() / 100 * oPenalty.AmountPercent.Percent.ToSafeInt();
                    }
                    else if (!string.IsNullOrEmpty(oPenalty.AmountPercent.NmbrOfNights))
                    {
                        int iNights = oPenalty.AmountPercent.NmbrOfNights.ToSafeInt();
                        // 'get whole list of rates
                        amount = oRoomStay.RoomRates.SelectMany(oRoomRate => oRoomRate.Rates.SelectMany(oRate =>
                        {
                            int iUnitMultiPlier = oRate.UnitMultiplier.ToSafeInt();
                            decimal dAmountAfterTax = oRate.RateBase.AmountAfterTax.ToSafeDecimal();
                            return Enumerable.Range(1, iUnitMultiPlier).Select(_ => dAmountAfterTax);
                        })).Take(iNights).Sum();
                    }
                    var startDate = oPenalty.Deadline.AbsoluteDeadline.ToSafeDate();

                    return new
                    {
                        amount,
                        startDate
                    };
                }).OrderBy(x => x.startDate).ToList();

                return penalties.Select((penalty, idx) => new Cancellation
                {
                    Amount = penalty.amount,
                    StartDate = penalty.startDate,
                    EndDate = penalties.Last() == penalty //penalties.Count == idx + 1
                        ? new DateTime(2099, 12, 31)
                        : penalties[idx + 1].startDate.AddDays(-1)
                });
            }).ToArray();

            var oCancellations = new Cancellations();
            oCancellations.AddRange(oCnxs);

            // 'merge room cancellations
            oCancellations = Cancellations.MergeMultipleCancellationPolicies(oCancellations);

            return oCancellations;
        }

        #endregion

        #region "Book"
        public async Task<string> BookAsync(PropertyDetails oPropertyDetails)
        {
            var source = oPropertyDetails.Source;
            string sReference = "";
            var oRequest = new Request();

            try
            {
                var xmlBookRq = BuildBookRequest(oPropertyDetails, HBSiHelper.Now());
                oRequest = await SendRequest(oPropertyDetails, xmlBookRq, "Book request");
                var bookRs = Envelope<OtaHotelResRs>.DeSerialize(_serializer, oRequest.ResponseXML);

                // 'Check for errors or pending bookings
                if (!bookRs.Errors.Any() && bookRs.IsSuccess())
                {
                    // 'get the reservation ID
                    sReference = bookRs.HotelReservations.First().ResGlobalInfo.HotelReservationIds.First(res => string.Equals(res.ResIdType, "8")).ResIdValue;

                    if (_settings.WaitForSupplierReference(oPropertyDetails, source))
                    {
                        // 'try to get the PMS id
                        string sPMSID = bookRs.HotelReservations.First().ResGlobalInfo.HotelReservationIds.First(res => string.Equals(res.ResIdType, "10")).ResIdValue;
                        //'if no PMS ID received (non-seamless transaction)
                        if (string.IsNullOrEmpty(sPMSID))
                        {
                            var dStartTime = HBSiHelper.Now();
                            // 'keep trying to get the PMS ID from the DB until we either recieve it or hit a timeout
                            while (string.IsNullOrEmpty(sPMSID) && (DateTime.Now - dStartTime).TotalSeconds <= _settings.MaxConfirmationWaitTimeInSeconds(oPropertyDetails, source))
                            {
                                // TODO: new TP version do not have TPConfirmationReferenceStore_Get stored procedure,
                                if (string.IsNullOrEmpty(sPMSID))
                                {
                                    // NOTE: this code slow performance
                                    System.Threading.Thread.Sleep(_settings.ConfirmationCheckRepeatInMs(oPropertyDetails, source));
                                }
                            }

                            // 'set it
                            if (!string.IsNullOrEmpty(sPMSID))
                            {
                                sReference = $"{sReference}|{sPMSID}";
                            }
                            else
                            {
                                var otaReadRq = BuildConfirmationBookingRequest(oPropertyDetails, dStartTime, sReference);
                                var oConfiramtionRequest = await SendRequest(oPropertyDetails, otaReadRq, "Confirm booking");
                                var oConfirmationResponse = Envelope<OtaResRetrieveRs>.DeSerialize(_serializer, oRequest.ResponseXML);

                                if (oConfirmationResponse.IsSuccess()
                                    && oConfirmationResponse.ReservationsList.FirstOrDefault(x => string.Equals(x.ResStatus, "Book")) != null)
                                {
                                    sPMSID = oConfirmationResponse.ReservationsList.First().ResGlobalInfo.HotelReservationIds
                                                        .FirstOrDefault(r => string.Equals(r.ResIdType, "10"))?.ResIdValue ?? "";

                                    if (string.IsNullOrEmpty(sPMSID))
                                    {
                                        sReference = Constant.Failed;
                                    }
                                    else
                                    {
                                        sReference = $"{sReference}|{sPMSID}";
                                    }
                                }
                                else
                                {
                                    sReference = Constant.Failed;
                                }
                            }
                        }
                        else
                        {
                            sReference = $"{sReference}|{sPMSID}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("BookException", ex.Message);
                sReference = "";
            }
            finally
            {
                oPropertyDetails.AddLog("Book", oRequest);
            }

            oPropertyDetails.SourceSecondaryReference = $"{oPropertyDetails.LeadGuestFirstName}|{oPropertyDetails.LeadGuestLastName}";
            return sReference;
        }

        #endregion

        #region "Cancel"

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails oPropertyDetails)
        {
            var oRequest = new Request();
            var oThirdPartyCancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                // 'Build and send Cancellation
                var cancelRq = BuildCancellationRequest(oPropertyDetails, HBSiHelper.Now());
                oRequest = await SendRequest(oPropertyDetails, cancelRq, "Cancellation");
                var cancelRs = Envelope<OtaCancelRs>.DeSerialize(_serializer, oRequest.ResponseXML);

                if (cancelRs.IsSuccess() && !cancelRs.Errors.Any())
                {
                    // 'store cancel id in cancellation reference field
                    oThirdPartyCancellationResponse.TPCancellationReference = cancelRs.CancelInfoRS.UniqueId.Id;
                    oThirdPartyCancellationResponse.Success = true;
                }
                else
                {
                    oThirdPartyCancellationResponse.Success = false;
                }
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("Cancellation Exception", ex.Message);
                oThirdPartyCancellationResponse.Success = false;
            }
            finally
            {
                oPropertyDetails.AddLog("Cancellation", oRequest);
            }

            return oThirdPartyCancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        #endregion


        #region "OtherMethods"

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return "";
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        #endregion


        #region "Helper Functions"

        private async Task<Request> SendRequest(PropertyDetails oPropertyDetails, XmlDocument xmlDocument, string sLogFileName)
        {
            var oRequest = new Request
            {
                EndPoint = _settings.GenericURL(oPropertyDetails, oPropertyDetails.Source),
                Method = RequestMethod.POST,
                LogFileName = sLogFileName,
                Source = oPropertyDetails.Source,
            };
            oRequest.SetRequest(xmlDocument);
            await oRequest.Send(_httpClient, _logger);

            return oRequest;
        }

        private XmlDocument BuildAvailabilityRequest(PropertyDetails oPropertyDetails, DateTime dTimeStamp)
        {
            var source = oPropertyDetails.Source;
            var oRatePlans = oPropertyDetails.Rooms.Select(oRoom =>
            {
                string[] refs = oRoom.ThirdPartyReference.Split('|');
                return $"{refs[1]}|{refs[2]}";
            }).Distinct().ToList();

            var hotelPreBookRequest = new OtaHotelAvailRq
            {
                Target = _settings.Target(oPropertyDetails, source),
                Version = _settings.Version(oPropertyDetails, source),
                BestOnly = "false",
                SummaryOnly = "false",
                TimeStamp = dTimeStamp.ToString(Constant.TimeStampFormat),
                Pos = HBSiHelper.GetPos(oPropertyDetails, _settings, source),
                AvailRequestSegmets =
                {
                    new AvailRequestSegment
                    {
                        HotelSearchCriteria =
                        {
                            new Criterion
                            {
                                StayDateRange =
                                {
                                    Start = oPropertyDetails.ArrivalDate.ToString(Constant.DateFormat),
                                    End = oPropertyDetails.DepartureDate.ToString(Constant.DateFormat)
                                },
                                RatePlanCandidates = oRatePlans.Select((sRatePlan, iRatePlanCodeIndex) =>
                                {
                                    string []refs = sRatePlan.Split('|');

                                    return new RatePlanCandidate
                                    {
                                        RatePlanCode = refs[0],
                                        RPH = $"{iRatePlanCodeIndex + 1}",
                                        HotelRefs =
                                        {
                                            new HotelRef
                                            {
                                                HotelCode = oPropertyDetails.TPKey.Split('|')[0]
                                            }
                                        },
                                        MealsIncluded =
                                        {
                                            MealPlanCodes = refs[1]
                                        }
                                    };
                                }).ToList(),
                                RoomStayCandidates = oPropertyDetails.Rooms.Select((oRoom, iRoomIdx) =>
                                {
                                    string[] refs = oRoom.ThirdPartyReference.Split('|');
                                    return new RoomStayCandidate
                                    {
                                        RoomTypeCode = refs[0],
                                        Quantity = 1,
                                        RPH = iRoomIdx + 1,
                                        RatePlanCandidateRPH = oRatePlans.IndexOf($"{refs[1]}|{refs[2]}") + 1,
                                        GuestCounts = GetGuestCounts(_settings.UsePassengerAge(oPropertyDetails, source), oRoom)
                                    };
                                }).ToList()
                            }
                        }
                    }
                }
            };

            return Envelope<OtaHotelAvailRq>.Serialize(_serializer, hotelPreBookRequest, _settings, oPropertyDetails, oPropertyDetails.TPKey.Split('|')[0],
                                                        "HotelAvailRQ", dTimeStamp.ToString("yyyyMMddhhmmssfff"), source);
        }

        public XmlDocument BuildBookRequest(PropertyDetails oPropertyDetails, DateTime dTimeStamp)
        {
            var source = oPropertyDetails.Source;
            var guests = oPropertyDetails.Rooms.SelectMany(room => room.Passengers).Select((pax, paxIdx) => new
            {
                pax,
                rph = paxIdx + 1
            });

            int iLeadPassengerIndex = guests.First(g => g.pax.PassengerType == PassengerType.Adult
                    && string.Equals(g.pax.FirstName, oPropertyDetails.LeadGuestFirstName)
                    && string.Equals(g.pax.LastName, oPropertyDetails.LeadGuestLastName)).rph;

            var oResGuests = guests.Select(oGuest =>
            {
                bool bUseAge = _settings.UsePassengerAge(oPropertyDetails, source);
                var pax = oGuest.pax;
                var customer = new Customer
                {
                    PersonName =
                        {
                            GivenName = pax.FirstName,
                            Surname = pax.LastName,
                        }
                };
                if (oGuest.rph == iLeadPassengerIndex)
                {
                    if (_settings.UseLeadGuestDetails(oPropertyDetails, source))
                    {
                        customer.Telephone = new Telephone
                        {
                            PhoneNumber = oPropertyDetails.LeadGuestPhone
                        };
                        customer.Email = oPropertyDetails.LeadGuestEmail;
                        customer.Address = new Models.Address
                        {
                            AddressLines = new List<string>
                            {
                                oPropertyDetails.LeadGuestAddress1,
                                oPropertyDetails.LeadGuestAddress2
                            },
                            CityName = oPropertyDetails.LeadGuestTownCity,
                            PostalCode = oPropertyDetails.LeadGuestPostcode,
                            CountryName =
                            {
                                Code = oPropertyDetails.SellingCountry
                            }
                        };
                    }
                    else
                    {
                        customer.Telephone = new Telephone
                        {
                            PhoneNumber = _settings.AgentPhoneNumber(oPropertyDetails, source),
                        };
                        customer.Email = _settings.AgentEmailAddress(oPropertyDetails, source);
                        customer.Address = new Models.Address
                        {
                            AddressLines =
                            {
                                _settings.AgentAddress(oPropertyDetails, source)
                            },
                            CityName = _settings.AgentCity(oPropertyDetails, source),
                            PostalCode = _settings.AgentPostCode(oPropertyDetails, source),
                            CountryName =
                            {
                                Code = _settings.AgentCountryCode(oPropertyDetails, source)
                            }
                        };
                    }
                }

                return new ResGuest
                {
                    ResGuestRPH = oGuest.rph,
                    Age = bUseAge ? $"{HBSiHelper.GetDefaultAge(pax.PassengerType)}" : "",
                    AgeQualifyingCode = HBSiHelper.GetAgeQualifyingCode(pax.PassengerType),
                    Profiles =
                    {
                        new ProfileInfo
                        {
                            Profile =
                            {
                                ProfileType = Constant.ProfileType_Customer,
                                Customer = customer
                            }
                        }
                    }
                };
            }).ToList();

            var payment = new DepositPayments();
            if (!string.Equals(_settings.PaymentMethod(oPropertyDetails, source), Constant.PaymentMethod_DirectBill))
            {
                var acceptPayment = new AcceptedPayment
                {
                    RPH = iLeadPassengerIndex
                };
                // 'Select payment method
                string sPaymentMethod = _settings.PaymentMethod(oPropertyDetails, source).ToLower();
                if (string.Equals(sPaymentMethod, "vcard"))
                {
                    // 'Set details from pre-generated virtual card
                    string sExpiryDate = oPropertyDetails.GeneratedVirtualCard.ExpiryMonth.PadLeft(2, '0') + oPropertyDetails.GeneratedVirtualCard.ExpiryYear.Substring(2, 2);
                    acceptPayment.PaymentCard = new PaymentCard
                    {
                        CardType = "1",
                        CardCode = _support.TPCreditCardLookup(oPropertyDetails.Source, oPropertyDetails.GeneratedVirtualCard.CardTypeID),
                        CardNumber = oPropertyDetails.GeneratedVirtualCard.CardNumber,
                        ExpireDate = sExpiryDate
                    };
                }

                payment.RequiredPayment.AcceptedPayments.Add(acceptPayment);
                payment.RequiredPayment.AmountPercent.Amount = oPropertyDetails.LocalCost.ToString();
                payment.RequiredPayment.Deadline.AbsoluteDeadline = oPropertyDetails.DepartureDate.ToString(Constant.DateFormat);
            }

            var uniqBookingId = $"{oPropertyDetails.BookingReference}{oPropertyDetails.ComponentNumber}";
            if (uniqBookingId.Length > Constant.BookingUniqIdMaxLength) 
            {
                throw new Exception($"Booking reference + component length should be equal or less than {Constant.BookingUniqIdMaxLength}");
            } 

            var bookRq = new OtaHotelResRq
            {
                Target = _settings.Target(oPropertyDetails, source),
                Version = _settings.Version(oPropertyDetails, source),
                TimeStamp = dTimeStamp.ToString(Constant.TimeStampFormat),
                ResStatus = "Commit",
                Pos = HBSiHelper.GetPos(oPropertyDetails, _settings, source),
                HotelReservations =
                {
                    new HotelReservation
                    {
                        RoomStayReservation = "true",
                        CreateDateTime = dTimeStamp.ToString(Constant.TimeStampFormat),
                        CreatorID = source,
                        UniqueId =
                        {
                            Type = "14",
                            Id = uniqBookingId
                        },
                        RoomStays = oPropertyDetails.Rooms.Select(oRoomDetails =>
                        {
                            var tpReference = _secretKeeper.Decrypt(oRoomDetails.ThirdPartyReference);
                            var oRoomStay = _serializer.DeSerialize<RoomStay>(tpReference);
                            oRoomStay.ResGuestRPHs = oRoomDetails.Passengers.Select(pax => new ResGuestRPH
                                {
                                    RPH = guests.First(g => g.pax == pax).rph
                                }).ToList();

                            return oRoomStay;
                        }).ToList(),
                        ResGuests = oResGuests,
                        ResGlobalInfo =
                        {
                            Comments = oPropertyDetails.BookingComments.Select(comment => new Comment
                            {
                                Text = comment.Text
                            }).ToList(),
                            DepositPayments = payment
                        }
                    }
                }
            };

            return Envelope<OtaHotelResRq>.Serialize(_serializer, bookRq, _settings, oPropertyDetails, oPropertyDetails.TPKey.Split('|')[0],
                                                        "HotelResRQ", dTimeStamp.ToString("yyyyMMddhhmmssfff"), source);
        }

        public XmlDocument BuildConfirmationBookingRequest(PropertyDetails oPropertyDetails, DateTime dTimeStamp, string sReservationID)
        {
            var source = oPropertyDetails.Source;
            var otaReadRq = new OtaReadRq
            {
                Version = "1.001",
                PrimaryLangID = "en-us",
                ReadRequests =
                {
                    new ReadRequest
                    {
                        UniqueId =
                        {
                            Type = "8",
                            Id = sReservationID.Trim()
                        }
                    }
                }
            };
            return Envelope<OtaReadRq>.Serialize(_serializer, otaReadRq, _settings, oPropertyDetails, oPropertyDetails.TPKey.Split('|')[0],
                                                        "ReadRQ", dTimeStamp.ToString("yyyyMMddhhmmssfff"), source);
        }

        public XmlDocument BuildCancellationRequest(PropertyDetails oPropertyDetails, DateTime dTimeStamp)
        {
            var source = oPropertyDetails.Source;
            string[] refs = oPropertyDetails.SourceReference.Split('|');
            string[] seconaryRefs = oPropertyDetails.SourceSecondaryReference.Split('|');
            var cancelRs = new OtaCancelRq
            {
                Target = _settings.Target(oPropertyDetails, source),
                Version = _settings.Version(oPropertyDetails, source),
                TimeStamp = dTimeStamp.ToString(Constant.TimeStampFormat),
                CancelType = "Commit",
                Pos = HBSiHelper.GetPos(oPropertyDetails, _settings, source),
                UniqueId =
                {
                    Type = "8",
                    Id = refs[0],
                    CompanyName = _settings.PartnerName(oPropertyDetails, source)
                },
                Verification =
                {
                    PersonName =
                    {
                        GivenName = seconaryRefs[0],
                        Surname = seconaryRefs[1]
                    }
                }
            };

            return Envelope<OtaCancelRq>.Serialize(_serializer, cancelRs, _settings, oPropertyDetails, oPropertyDetails.TPKey.Split('|')[0],
                                                        "CancelRQ", dTimeStamp.ToString("yyyyMMddhhmmssfff"), source);
        }

        public static List<GuestCount> GetDefaultAgesGuestCounts(RoomDetails oRoomDetail)
        {
            var guestCounts = new List<GuestCount>();
            if (oRoomDetail.Adults > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    Age = HBSiHelper.GetDefaultAge(PassengerType.Adult),
                    Count = oRoomDetail.Adults
                });
            }
            if (oRoomDetail.Children > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    Age = HBSiHelper.GetDefaultAge(PassengerType.Child),
                    Count = oRoomDetail.Children
                });
            }
            if (oRoomDetail.Infants > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    Age = HBSiHelper.GetDefaultAge(PassengerType.Infant),
                    Count = oRoomDetail.Infants
                });
            }
            return guestCounts;
        }

        public static List<GuestCount> GetQualifyingAgeGuestCounts(RoomDetails oRoomDetail)
        {
            var guestCounts = new List<GuestCount>();
            if (oRoomDetail.Adults > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    AgeQualifyingCode = HBSiHelper.GetAgeQualifyingCode(PassengerType.Adult),
                    Count = oRoomDetail.Adults
                });
            }
            if (oRoomDetail.Children > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    AgeQualifyingCode = HBSiHelper.GetAgeQualifyingCode(PassengerType.Child),
                    Count = oRoomDetail.Children
                });
            }
            if (oRoomDetail.Infants > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    AgeQualifyingCode = HBSiHelper.GetAgeQualifyingCode(PassengerType.Infant),
                    Count = oRoomDetail.Infants
                });
            }
            return guestCounts;
        }

        public static List<GuestCount> GetGuestCounts(bool bUsePassangerAge, RoomDetails oRoomDetails)
        {
            return (bUsePassangerAge
                ? GetDefaultAgesGuestCounts(oRoomDetails)
                : GetQualifyingAgeGuestCounts(oRoomDetails)).ToList();
        }

        #endregion
    }
}
