namespace iVectorOne.CSSuppliers.AmadeusHotels.Support
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Intuitive;
    using iVectorOne.Constants;
    using iVectorOne.Lookups;
    using iVectorOne.Models.Property.Booking;
    using Models;
    using Models.Common;
    using Models.Header;
    using Models.Soap;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;

    public class AmadeusHelper
    {
        private readonly IAmadeusHotelsSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly ISecretKeeper _secretKeeper;
        private readonly string _source = ThirdParties.AMADEUSHOTELS;

        public AmadeusHelper(IAmadeusHotelsSettings settings, ITPSupport support, ISerializer serializer, ISecretKeeper secretKeeper)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
        }

        public XmlDocument MultiSingleAvailabilityHotelRequest(PropertyDetails propertyDetails, IEnumerable<RoomDetails> rooms)
        {
            var request = new Envelope<OTAHotelAvailRQ>
            {
                Header = SoapHeaderBuilder.BuildSoapHeader(
                    _settings,
                    propertyDetails,
                    AmadeusHotelsSoapActions.HotelMultiSingleHotelSoapAction,
                    true,
                    propertyDetails.TPRef1),
                Body = { Content = new OTAHotelAvailRQ()
                {
                    RateRangeOnly = true,
                    RateDetailsInd = true,
                    Version = "4",
                    EchoToken = "MultiSingle",
                    AvailRatesOnly = true,
                    SummaryOnly = true,
                    SortOrder = "PA",
                    SearchCacheLevel = "Live",
                    AvailRequestSegments =
                    {
                        AvailRequestSegment =
                        {
                            InfoSource = "Distribution",
                            HotelSearchCriteria =
                            {
                                AvailableOnlyIndicator = true,
                                Criterion =
                                {
                                    ExactMatch = true,
                                    HotelRef =
                                    {
                                        ChainCode = propertyDetails.TPKey.Split('_')[0],
                                        HotelCode = propertyDetails.TPKey.Split('_')[1],
                                        HotelCityCode = propertyDetails.ResortCode.Split('|')[0]
                                    },
                                    StayDateRange =
                                    {
                                        End = $"{propertyDetails.DepartureDate:yyyy-MM-dd}",
                                        Start = $"{propertyDetails.ArrivalDate:yyyy-MM-dd}"
                                    },
                                    RatePlanCandidates = HotelAvailRQBuilder.BuildRatePlanCandidate(rooms),
                                    RoomStayCandidates = HotelAvailRQBuilder.BuildRoomStayCandidate(
                                        rooms,
                                        _settings.IncludeRoomTypeCodeInSearch(propertyDetails))
                                }
                            }
                        }
                    }
                }}
            };

            return _serializer.Serialize(request);
        }

        public XmlDocument PricingCheckRequest(
            PropertyDetails propertyDetails,
            RoomDetails roomDetails,
            string sessionId,
            bool useRoomForRatePlan = false)
        {
            var request = new Envelope<OTAHotelAvailRQ>
            {
                Header = SoapHeaderBuilder.BuildSoapHeader(
                    _settings, propertyDetails,
                    AmadeusHotelsSoapActions.EnhancedPricingSoapAction,
                    true,
                    sessionId),
                Body = { Content =
                {
                    AvailRatesOnly = true,
                    EchoToken = "Pricing",
                    Version = "4.000",
                    PrimaryLangID = "EN",
                    SummaryOnly = false,
                    RateRangeOnly = false,
                    AvailRequestSegments = new AvailRequestSegments
                    {
                        AvailRequestSegment =
                        {
                            HotelSearchCriteria =
                            {
                                Criterion =
                                {
                                    ExactMatch = true,
                                    HotelRef =
                                    {
                                        ChainCode = propertyDetails.TPKey.Split('_')[0],
                                        HotelCode = propertyDetails.TPKey.Split('_')[1],
                                        HotelCityCode = propertyDetails.TPKey.Split('|')[0],
                                        HotelCodeContext = "1A"
                                    },
                                    StayDateRange =
                                    {
                                        Start = $"{propertyDetails.ArrivalDate:yyyy-MM-dd}",
                                        End = $"{propertyDetails.DepartureDate::yyyy-MM-dd}"
                                    },
                                    RatePlanCandidates = useRoomForRatePlan 
                                        ? HotelAvailRQBuilder.BuildRatePlanCandidate(new List<RoomDetails> { roomDetails })
                                        : HotelAvailRQBuilder.BuildRatePlanCandidate(propertyDetails.Rooms),
                                    RoomStayCandidates = HotelAvailRQBuilder.BuildSingleRoomStayCandidate(roomDetails)
                                }
                            }
                        }
                    }
                }}
            };

            return _serializer.Serialize(request);
        }

        public XmlDocument PNRAddTravellerInfo(
            PropertyDetails propertyDetails,
            RoomDetails[] rooms,
            string sessionId)
        {
            var passenger = rooms[0].Passengers[0];
            var dataElements = new List<DataElementsIndiv>
            {
                DataElementsBuilder(1, "AP", "7", _settings.Telephone(propertyDetails)),
                DataElementsBuilder(2, "AP", "P02", _settings.Email(propertyDetails)),
                DataElementsBuilder(3, "TK", "", "", true),
            };

            string recived = _settings.ReceivedFrom(propertyDetails);
            if (!string.IsNullOrEmpty(recived))
            {
                dataElements.Add(DataElementsBuilder(null, "RF", "P22", recived));
            }

            var request = new Envelope<PNRAddMultiElements>
            {
                Header = SoapHeaderBuilder.BuildSoapHeader(
                    _settings,
                    propertyDetails,
                    AmadeusHotelsSoapActions.EnhancedPricingSoapAction,
                    true,
                    sessionId),
                Body = { Content =
                {
                    PnrActions = new []{ 0 },
                    TravellerInfo =
                    {
                        ElementManagementPassenger =
                        {
                            Reference =
                            {
                                Qualifier = "PR",
                                Number = 1,
                            },
                            SegmentName = "NM",
                        },
                        PassengerData =
                        {
                            TravellerInformation =
                            {
                                Traveller =
                                {
                                    Surname = passenger.LastName,
                                    Quantity = 1
                                },
                                Passenger = { FirstName = passenger.FirstName }
                            }
                        }
                    },
                    DataElementsMaster = { DataElementsIndiv = dataElements.ToArray() }
                } }
            };

            return _serializer.Serialize(request);
        }

        public XmlDocument CreateSplitMultiRoomHotelSellRequest(
            PropertyDetails propertyDetails,
            List<RoomDetails> roomDetails,
            string tatooNumber, 
            string paymentCode,
            ref string sessionId)
        {
            sessionId = IncrementSession(sessionId);
            var request = new Envelope<HotelSell>
            {
                Header = SoapHeaderBuilder.BuildSoapHeader(
                    _settings,
                    propertyDetails,
                    AmadeusHotelsSoapActions.HotelSellSoapAction,
                    true,
                    sessionId),
                Body = { Content =
                {
                    TravelAgentRef =
                    {
                        Status = "APE",
                        Reference =
                        {
                            Type = "OT",
                            Value = "2"
                        }
                    },
                    RoomStayData = new []
                    {
                        BuildRoomStayForRoom(roomDetails[0], propertyDetails, tatooNumber, GetPaymentType(paymentCode))
                    },
                }}
            };

            return _serializer.Serialize(request);
        }

        public XmlDocument CreateHotelSellRequest(
            PropertyDetails propertyDetails,
            RoomDetails[] roomDetails,
            string tatooNumber,
            string[] roomPaymentCodes,
            ref string sessionId)
        {
            sessionId = IncrementSession(sessionId);
            var request = new Envelope<HotelSell>
            {
                Header = SoapHeaderBuilder.BuildSoapHeader(
                    _settings,
                    propertyDetails,
                    AmadeusHotelsSoapActions.HotelSellSoapAction,
                    true,
                    sessionId),
                Body =
                {
                    Content =
                    {
                        TravelAgentRef = { Status = "APE", Reference = { Type = "OT", Value = "2" } },
                        RoomStayData = roomDetails
                            .Select((room, index)
                                => BuildRoomStayForRoom(room, propertyDetails, tatooNumber,
                                    GetPaymentType(roomPaymentCodes[index])))
                            .ToArray()
                    }
                }
            };

            return _serializer.Serialize(request);
        }

        public XmlDocument CancelPNR(PropertyDetails propertyDetails, string bookingReference)
        {
            var request = new Envelope<PNRCancel>
            {
                Header = SoapHeaderBuilder.BuildSoapHeader(
                    _settings,
                    propertyDetails,
                    AmadeusHotelsSoapActions.CancelPNRSoapAction),
                Body = {Content =
                {
                    ReservationInfo = { Reservation = { ControlNumber = bookingReference }},
                    PnrActions = new []{ 10 },
                    CancelElements = { EntryType = "I" }
                }}
            };

            return _serializer.Serialize(request);
        }

        public XmlDocument PNREndTransaction(PropertyDetails propertyDetails, ref string sessionId)
        {
            sessionId = IncrementSession(sessionId);
            var request = new Envelope<PNRAddMultiElements>
            {
                Header = SoapHeaderBuilder.BuildSoapHeader(
                    _settings,
                    propertyDetails,
                    AmadeusHotelsSoapActions.PNRAddMultiElementSoapAction,
                    true,
                    sessionId),
                Body =
                {
                    Content =
                    {
                        PnrActions = new[] { 11, 30 },
                        DataElementsMaster =
                        {
                            DataElementsIndiv = new[]
                            {
                                new DataElementsIndiv
                                {
                                    ElementManagementData =
                                    {
                                        Reference =
                                        {
                                            Qualifier = "OT",
                                            Number = 1
                                        },
                                        SegmentName = "RF"
                                    },
                                    FreetextData =
                                    {
                                        FreetextDetail =
                                        {
                                            SubjectQualifier = "3",
                                            Type = "P22"
                                        },
                                        LongFreetext = $"Received From {propertyDetails.LeadGuestFirstName + propertyDetails.LeadGuestLastName} "
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return _serializer.Serialize(request);
        }

        public string RemoveCardNumbers(string xml)
        {
            try
            {
                // do the card number first
                var regex = new Regex(@"\d{15}[0-9]?");
                var matches = regex.Matches(xml);

                foreach (var match in matches)
                {
                    string cardNumber = match.ToString();
                    xml = xml.Replace(cardNumber, "XXXXXXXXXXXX" + cardNumber.Substring(12));
                }

                // then the cvv number
                regex = new Regex(@"<securityId>\d{3}[0-9]?</securityId>");
                matches = regex.Matches(xml);

                foreach (var match in matches)
                {
                    string cvvNumber = match.ToString();
                    xml = xml.Replace(cvvNumber, "<securityId>XXX</securityId>");
                }

                return xml;
            }
            catch (Exception)
            {
                // just return the original xml
                return xml;
            }
        }

        public XmlDocument EndSession(PropertyDetails propertyDetails, string sessionId)
        {
            var request = new Envelope<SecuritySignOut>
            {
                Header = SoapHeaderBuilder.BuildSoapHeader(
                    _settings,
                    propertyDetails,
                    AmadeusHotelsSoapActions.SignOutSoapAction,
                    true,
                    sessionId)
            };

            return _serializer.Serialize(request);
        }

        public static string[] SplitBuilder(string csvString, char separator = ',')
        {
            char[] arraySeparators = { separator };
            return csvString.Split(arraySeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        public string IncrementSession(string sessionToken)
        {
            string[] splitToken = SplitBuilder(sessionToken, '|');
            return $"{splitToken[0]}|{splitToken[1].ToSafeInt() + 1}|{splitToken[2]}";
        }

        private DataElementsIndiv DataElementsBuilder(
            int? number,
            string segmentName,
            string type,
            string longFreetext,
            bool indicator = false)
        {
            return new DataElementsIndiv
            {
                ElementManagementData =
                {
                    Reference = number == null 
                        ? new Reference()
                        : new Reference 
                        {
                            Qualifier = "OT",
                            Number = number.GetValueOrDefault()
                        },
                    SegmentName = segmentName
                },
                FreetextData = indicator ? new FreetextData() : new FreetextData
                {
                    FreetextDetail =
                    {
                        SubjectQualifier = "3",
                        Type = type
                    },
                    LongFreetext = longFreetext
                },
                TicketElement = !indicator ? new TicketElement() : new TicketElement
                {
                    Ticket = { Indicator = "OK" }
                } 
                    
            };
        }

        private RoomStayData BuildRoomStayForRoom(
            RoomDetails room,
            PropertyDetails propertyDetails,
            string tatooNumber,
            int paymentType)
        {
            bool isDirectBilling = _settings.BillingMethod(propertyDetails).ToLower() == "directbilling";
            return new RoomStayData
            {
                GlobalBookingInfo =
                {
                    RepresentativeParties =
                    {
                        OccupantList = { PassengerReference =
                        {
                            Type = "BHO",
                            Value = tatooNumber
                        } }
                    }
                },
                RoomList =
                {
                    RoomRateDetails =
                    {
                        HotelProductReference =
                        {
                            ReferenceDetails =
                            {
                                Type = "BC",
                                Value = room.ThirdPartyReference.Split('|')[0]
                            }
                        }
                    },
                    GuaranteeOrDeposit =
                    {
                        PaymentInfo =
                        {
                            PaymentDetails =
                            {
                                FormOfPaymentCode = isDirectBilling ? 9 : 1,
                                PaymentType = paymentType,
                                ServiceToPay = 3,
                            }
                        },
                        GroupCreditCardInfo = !isDirectBilling
                            ? GetCardDetails(propertyDetails, paymentType == 1)
                            : null,
                    },
                    GuestList = {OccupantList = {PassengerReference =
                    {
                        Type = "RMO",
                        Value = tatooNumber
                    }}}
                }
            };
        }

        private static int GetPaymentType(string ratecode)
        {
            return ratecode switch
            {
                "8" => 2,
                "31" => 1,
                _ => 0
            };
        }

        private GroupCreditCardInfo GetCardDetails(PropertyDetails propertyDetails, bool payLocal)
        {
            return !payLocal
                ? GetDetails(propertyDetails, GetCreditCardDetails(propertyDetails))
                : GetDetails(propertyDetails);
        }

        private GroupCreditCardInfo GetDetails(PropertyDetails propertyDetails, CCDetails? ccDetails = null)
        {
            var ccInfo = new CcInfo
            {
                VendorCode = ccDetails?.CCType ?? _support.TPCreditCardLookup(_source, propertyDetails.GeneratedVirtualCard.CardTypeID),
                CardNumber = ccDetails?.CCNumber ?? propertyDetails.GeneratedVirtualCard.CardNumber,
                SecurityId = ccDetails?.CCIdentifier ?? propertyDetails.GeneratedVirtualCard.CVV,
                ExpiryDate = ccDetails != null 
                    ? ccDetails.CCExpirationMonth + ccDetails.CCExpirationYear.Substring(2)
                    : propertyDetails.GeneratedVirtualCard.ExpiryMonth + propertyDetails.GeneratedVirtualCard.ExpiryYear,
                CcHolderName = ccDetails != null 
                    ? ccDetails.FirstName + ccDetails.LastName 
                    : propertyDetails.GeneratedVirtualCard.CardHolderName,
                Surname = ccDetails?.LastName ?? string.Empty,
                FirstName = ccDetails?.FirstName ?? string.Empty,
            };

            string line1 = string.Empty;
            string city = string.Empty;
            string zipCode = string.Empty;
            if (ccDetails != null)
            {
                line1 = ccDetails.Address1;
                city = ccDetails.City;
                zipCode = ccDetails.PostalCode;
            }
            //else
            //{
            //    line1 = propertyDetails.CardDetails.Address;
            //    city = propertyDetails.CardDetails.City;
            //    zipCode =  propertyDetails.CardDetails.Postcode;
            //}


            return new GroupCreditCardInfo
            {
                CreditCardInfo = { CcInfo = ccInfo },
                CardHolderAddress =
                {
                    AddressDetails =
                    {
                        Format = "5",
                        Line1 = line1
                    },
                    City = city,
                    ZipCode = zipCode
                }
            };
        }

        private CCDetails GetCreditCardDetails(PropertyDetails propertyDetails)
        {
            // AmadeusHotels need us to send through credit card details at the time of booking so we need to store them here
            // we are doing this by encrypting the details and adding them to the settings
            // they will need to be in the format
            // CID1@creditCardType1|creditCardNumber1|CCIdentifier1|CCExpirationMonth1|CCExpirationYear1|FirstName1|LastName1|Address1|City1|StateProvince1|Country1|PostalCode1#CID2@creditCardType2....
            // and then encrypted manually using intuitive.functions.encrypt
            // each card number will need a unique CID
            var cardDetailslookup = new Dictionary<string, string>();
            string encryptedCardDetails = _settings.EncryptedCardDetails(propertyDetails);
            string? decryptedCardDetails = _secretKeeper.Decrypt(encryptedCardDetails);

            if (decryptedCardDetails.Contains("#"))
            {
                foreach (string cardCIDPair in decryptedCardDetails.Split('#'))
                {
                    cardDetailslookup.Add(cardCIDPair.Split('@')[0], cardCIDPair.Split('@')[1]);
                }
            }
            else
            {
                cardDetailslookup.Add(decryptedCardDetails.Split('@')[0], decryptedCardDetails.Split('@')[1]);
            }

            return new CCDetails(cardDetailslookup[_settings.CID(propertyDetails)]);
        }

        public class CCDetails
        {
            public string CCType;
            public string CCNumber;
            public string CCIdentifier;
            public string CCExpirationMonth;
            public string CCExpirationYear;
            public string FirstName;
            public string LastName;
            public string Address1;
            public string City;
            public string StateProvince;
            public string Country;
            public string PostalCode;

            public CCDetails(string ccDetails)
            {
                string[] splitBuilder = SplitBuilder(ccDetails, '|');
                try
                {
                    CCType = splitBuilder[1];
                    CCNumber = splitBuilder[2];
                    CCIdentifier = splitBuilder[3];
                    CCExpirationMonth = splitBuilder[4];
                    CCExpirationYear = splitBuilder[5];
                    FirstName = splitBuilder[6];
                    LastName = splitBuilder[7];
                    Address1 = splitBuilder[8];
                    City = splitBuilder[9];
                    StateProvince = splitBuilder[10];
                    Country = splitBuilder[11];
                    PostalCode = splitBuilder[12];
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error getting lodged credit card details - {ex}");
                }
            }
        }
    }
}
