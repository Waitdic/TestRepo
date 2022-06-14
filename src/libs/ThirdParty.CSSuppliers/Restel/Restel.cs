namespace ThirdParty.CSSuppliers.Restel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.Restel.Models;
    using static RestelThirdPartyReference;
    using static RestelCommon;
    using System.Threading.Tasks;

    public class Restel : IThirdParty, ISingleSource
    {
        private readonly IRestelSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ISecretKeeper _secretKeeper;
        private readonly ILogger<Restel> _logger;

        public Restel(
            IRestelSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ISecretKeeper secretKeeper,
            ILogger<Restel> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string Source => ThirdParties.RESTEL;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            string codusu = _settings.Codusu(propertyDetails);

            bool preBookSuccess = false;
            Request? request = null;

            bool nameSupplied = _settings.GuestNamesAvailable(propertyDetails);

            try
            {
                // If the name is supplied, then can continue with normal pre-booking procedure
                if (nameSupplied)
                {
                    request = CreateRequest(_settings, propertyDetails, "PreBook");
                    request.SetRequest(Reservation(propertyDetails));
                    await request.Send(_httpClient, _logger);

                    var response = _serializer.DeSerialize<RestelPreBookResponse>(request.ResponseXML);

                    // If Restel Return an Error Bomb Out
                    if (response.Parametros.Estado == "00")
                    {
                        // store reference to use in book
                        propertyDetails.TPRef1 = response.Parametros.Localizador;

                        // get the costs from the response and match up with the room bookings
                        decimal totalCost = response.Parametros.ImporteTotalReserva.ToSafeDecimal();

                        if (propertyDetails.LocalCost != totalCost)
                        {
                            foreach (var roomDetails in propertyDetails.Rooms)
                            {
                                decimal cost = totalCost / propertyDetails.Rooms.Count;
                                roomDetails.LocalCost = cost;
                                roomDetails.GrossCost = cost;
                            }
                        }
                    }
                }
                else // if We don't have guest names, All we can do is do the search again.
                {
                    string resortCode = propertyDetails.GeographyLevel3ID == 0
                        ? await _support.TPResortCodeByPropertyIdLookupAsync(ThirdParties.RESTEL, propertyDetails.PropertyID)
                        : await _support.TPResortCodeByGeographyIdLookupAsync(ThirdParties.RESTEL, propertyDetails.GeographyLevel3ID);

                    var xmlAvailabilityRequest = CreateAvailabilityRequestXml(
                        resortCode,
                        propertyDetails.Duration,
                        propertyDetails.ArrivalDate,
                        propertyDetails.DepartureDate,
                        codusu,
                        propertyDetails.Rooms.Select(roomDetail => $"{roomDetail.Adults}-{roomDetail.Children}"),
                        _serializer);

                    // get response
                    request = CreateRequest(_settings, propertyDetails, "Restel Availability Check");
                    request.SetRequest(xmlAvailabilityRequest);
                    await request.Send(_httpClient, _logger);

                    var response = _serializer.DeSerialize<RestelAvailabilityResponse>(request.ResponseXML);

                    // If we don't have any results, fail
                    if (response.Param.Hotls.Num != 0)
                    {
                        // Get the XML for the hotel we want
                        var hotel = response.Param.Hotls.Hot.First(h => h.Cod == propertyDetails.TPKey);

                        // for each room check the price and get the reference
                        foreach (var roomDetails in propertyDetails.Rooms)
                        {
                            string occupancy = roomDetails.Adults + "-" + roomDetails.Children;
                            var restelReference = FromEncryptedString(roomDetails.ThirdPartyReference, _secretKeeper);
                            string roomCode = restelReference.RoomCode;
                            string mealBasis = restelReference.MealBasis;
                            string roomType = roomDetails.RoomType;

                            // Get the Details for the Room we want
                            var rooms = from pax in hotel.Res.Pax
                                where pax.Cod == occupancy
                                from hab in pax.Hab
                                where hab.Cod == roomCode
                                      && string.Equals(hab.Desc, roomType, StringComparison.CurrentCultureIgnoreCase)
                                from reg in hab.Reg
                                where reg.Cod == mealBasis
                                select reg;

                            var room = rooms.First();

                            // Grab the totalCost for the room
                            decimal totalRoomCost = room.Prr;

                            // Now we have the overall cost for all rooms, Check for Price Changes
                            if (totalRoomCost != roomDetails.LocalCost)
                            {
                                roomDetails.LocalCost = totalRoomCost;
                            }

                            // Now Grab the updated References incase they have changed
                            restelReference.ThirdPartyReferences = room.Lin.ToList();

                            string reference = restelReference.ToEncryptedString(_secretKeeper);

                            // only change the lin's if they have actually changed
                            if (roomDetails.ThirdPartyReference != reference)
                            {
                                roomDetails.ThirdPartyReference = reference;
                            }
                        }
                    }
                }

                // cancellation charges
                await GetCancellationsAsync(propertyDetails);

                // If we've reached here without incident mark as a success (not failing the prebook based on any errata or value added info errors)
                if (propertyDetails.Warnings.Count == 0)
                {
                    preBookSuccess = true;
                }

                // Get Errata
                await GetErrataAsync(propertyDetails);

                // Get Value Added Info
                await GetAddedValuesAsync(propertyDetails);
            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("PreBook Exception", exception.ToString());
                preBookSuccess = false;
            }
            finally
            {
                // store the request and response xml on the property pre-booking
                string logTitle = nameSupplied
                    ? "Restel Pre-Book"
                    : "Restel Availability Check";

                if (request != null && !string.IsNullOrWhiteSpace(request.RequestString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.RESTEL, $"{logTitle} Request", request.RequestString);
                }

                if (request != null && !string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.RESTEL, $"{logTitle} Response", request.ResponseString);
                }
            }

            return preBookSuccess;
        }

        private async Task GetCancellationsAsync(PropertyDetails propertyDetails)
        {
            string requestLog = string.Empty;
            string responseLog = string.Empty;
            var cancellations = new Cancellations();
            try
            {
                // We seem to have to do this call once per Room Booking
                int roomNumber = 1;
                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    var rates = new List<decimal>();

                    var request = new RestelGetCancellationsRequest
                    {
                        Nombre = "Gastos de cancelacion de reservas",
                        Parametros =
                        {
                            DatosReserva =
                            {
                                Hotel = propertyDetails.TPKey
                            }
                        }
                    };

                    // Get the DailyReferences from the TPReference
                    List<string> dailyRefsList;
                    if (_secretKeeper.IsEncrypted(roomDetails.ThirdPartyReference))
                    {
                        dailyRefsList = FromEncryptedString(roomDetails.ThirdPartyReference, _secretKeeper).ThirdPartyReferences;
                    }
                    else
                    {
                        string dailyRef = roomDetails.ThirdPartyReference.Split('|')[1];
                        dailyRefsList = dailyRef.Split('@').ToList();
                    }

                    foreach (string lin in dailyRefsList.Where(lin => !string.IsNullOrEmpty(lin)))
                    {
                        request.Parametros.DatosReserva.Lin.Add(lin);

                        // pick out the rates
                        decimal dRate = lin.Split('#')[3].ToSafeDecimal();
                        rates.Add(dRate);
                    }


                    // Send the Request
                    var webRequest = CreateRequest(_settings, propertyDetails, $"Cancellation Costs Room {roomNumber}");
                    webRequest.SetRequest(_serializer.Serialize(request));
                    await webRequest.Send(_httpClient, _logger);

                    // Grab the Logs to add to the booking later.
                    requestLog += $"{Environment.NewLine}Room {roomNumber}{Environment.NewLine}{webRequest.RequestLog}";
                    responseLog += $"{Environment.NewLine}Room {roomNumber}{Environment.NewLine}{webRequest.ResponseLog}";

                    // Grab the Response
                    var response = _serializer.DeSerialize<RestelGetCancellationsResponse>(webRequest.ResponseXML);

                    foreach (var politicaCanc in response.Parametros.PoliticaCanc)
                    {
                        int startDay = politicaCanc.DiasAntelacion.ToSafeInt();
                        int nightsCharged = politicaCanc.NochesGasto.ToSafeInt();
                        decimal percentage = politicaCanc.EstComGasto.ToSafeDecimal();
                        var cancellation = new Cancellation
                        {
                            StartDate = propertyDetails.ArrivalDate.AddDays(startDay * -1),
                            EndDate = propertyDetails.ArrivalDate
                        };

                        // Now work out if we are doing a % or a number of nights
                        if (nightsCharged > 0)
                        {
                            if (rates.Count > 0)
                            {
                                // use the daily rates
                                decimal total = rates.Take(nightsCharged).Sum();
                                cancellation.Amount = total.ToSafeMoney();
                            }
                            else
                            {
                                cancellation.Amount = propertyDetails.LocalCost / propertyDetails.Duration * nightsCharged;
                            }
                        }
                        else
                        {
                            cancellation.Amount = propertyDetails.LocalCost * (percentage / 100m);
                        }

                        // Only Add the policy if we have an amount
                        if (cancellation.Amount > 0)
                        {
                            cancellations.Add(cancellation);
                        }
                    }

                    roomNumber += 1;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Cancellation Exception", ex.ToString());
            }
            finally
            {
                // add logs to booking
                propertyDetails.Logs.AddNew(ThirdParties.RESTEL, "Restel Cancellation Costs Request", requestLog);
                propertyDetails.Logs.AddNew(ThirdParties.RESTEL, "Restel Cancellation Costs Response", responseLog);
            }

            propertyDetails.Cancellations = cancellations;
        }

        private async Task GetErrataAsync(PropertyDetails propertyDetails)
        {
            Request? webRequest = null;

            try
            {
                // Build the request
                var request = new RestelGetErrataRequest
                {
                    Parametros =
                    {
                        CodigoCobol = propertyDetails.TPKey,
                        Entrada = propertyDetails.ArrivalDate.ToString("dd-MM-yyyy"),
                        Salida = propertyDetails.DepartureDate.ToString("dd-MM-yyyy")
                    }
                };

                // Send the Request to Restel
                webRequest = CreateRequest(_settings, propertyDetails, "Get Errata");
                webRequest.SetRequest(_serializer.Serialize(request));
                await webRequest.Send(_httpClient, _logger);

                // Grab the Response
                var response = _serializer.DeSerialize<RestelGetErrataResponse>(webRequest.ResponseXML);

                // Grab the Errata out of the XML response
                if (response.Parametros?.Hotel?.Observaciones != null)
                {
                    foreach (var observacion in response.Parametros.Hotel.Observaciones)
                    {
                        propertyDetails.Errata.AddNew("Important Information", observacion.ObsTexto);
                    }
                }
            }
            catch (Exception)
            {
                // Do Nothing as we don't want to fail the Pre-book based on the errata.
            }
            finally
            {
                if (webRequest != null)
                {
                    propertyDetails.Logs.AddNew(ThirdParties.RESTEL, "Errata", webRequest.RequestLog, webRequest.ResponseLog);
                }
            }
        }

        private async Task GetAddedValuesAsync(PropertyDetails propertyDetails)
        {
            Request? webRequest = null;

            try
            {
                // Build The Request
                var request = new RestelGetAddedValuesRequest
                {
                    Nombre = "Petición de Valores Añadidos",
                    Parametros =
                    {
                        Idioma = 2,
                        Fentrada = propertyDetails.ArrivalDate.ToString("dd/MM/yyyy"),
                        Codhot = propertyDetails.TPKey
                    }
                };

                // Send the Request to Restel
                webRequest = CreateRequest(_settings, propertyDetails, "Get Added Values");
                webRequest.SetRequest(_serializer.Serialize(request));
                await webRequest.Send(_httpClient, _logger);

                // Grab the Response
                var response = _serializer.DeSerialize<RestelGetAddedValuesResponse>(webRequest.ResponseXML);

                // Grab Each Value Add out the response and add it as an Erratum
                if (response.Parametros?.Valorafegit != null)
                {
                    foreach (var valorafegit in response.Parametros.Valorafegit)
                    {
                        propertyDetails.Errata.AddNew(valorafegit.Nom,
                            $"{valorafegit.Descripcio}{Environment.NewLine}{valorafegit.Duracio}");
                    }
                }
            }
            catch (Exception)
            {
                // Do Nothing Here as we don't want to fail the booking based on
            }
            finally
            {
                if (webRequest != null)
                {
                    propertyDetails.Logs.AddNew("Value Add", webRequest.RequestLog, webRequest.ResponseLog);
                }
            }
        }

        private XmlDocument Reservation(PropertyDetails propertyDetails)
        {
            var firstPassenger = propertyDetails.Rooms[0].Passengers[0];

            var preBookRequest = new RestelPrebookRequest
            {
                Parametros =
                {
                    CodigoHotel = propertyDetails.TPKey,
                    NombreCliente = $"{firstPassenger.FirstName} {firstPassenger.LastName}",
                    Observaciones =  propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any() ?
                     string.Join("\n", propertyDetails.Rooms.Select(x => x.SpecialRequest)):
                     "",
                    NumMensaje = string.Empty,
                    FormaPago = 25,
                    Res =
                    {
                        Lin = (from roomDetails in propertyDetails.Rooms
                            from lin in FromEncryptedString(roomDetails.ThirdPartyReference, _secretKeeper).ThirdPartyReferences
                            where !string.IsNullOrEmpty(lin)
                            select lin).ToArray()
                    },
                    Idioma = 2
                }
            };

            return _serializer.Serialize(preBookRequest);
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string reference;
            string preBookRequestLog = string.Empty;
            string preBookResponseLog = string.Empty;
            string bookRequestLog = string.Empty;
            string bookResponseLog = string.Empty;

            bool nameSupplied = _settings.GuestNamesAvailable(propertyDetails);

            try
            {
                if (!nameSupplied)
                {
                    var prebookRequest = Reservation(propertyDetails);

                    var prebookWebRequest = CreateRequest(_settings, propertyDetails, "PreBook");
                    prebookWebRequest.SetRequest(prebookRequest);
                    await prebookWebRequest.Send(_httpClient, _logger);

                    // Grab the Logs for later
                    preBookRequestLog = prebookWebRequest.RequestLog;
                    preBookResponseLog = prebookWebRequest.ResponseLog;

                    // Grab the Response
                    var preBookResponse = _serializer.DeSerialize<RestelPreBookResponse>(prebookWebRequest.ResponseXML);

                    // store reference to use in book
                    propertyDetails.TPRef1 = preBookResponse.Parametros.Localizador;
                    if (preBookResponse.Parametros.Estado != "00")
                    {
                        throw new Exception();
                    }
                }

                var bookRequest = new RestelBookRequest
                {
                    Parametros =
                    {
                        Localizador = propertyDetails.TPRef1,
                        // AE confirms reservation, AI annuls reservation
                        Accion = "AE",
                        Afiliacion = "RS",
                        Idioma = 2
                    }
                };

                // Send the request
                var bookWebRequest = CreateRequest(_settings, propertyDetails, "Book");
                bookWebRequest.CreateErrorLog = true;
                bookWebRequest.SetRequest(_serializer.Serialize(bookRequest));
                await bookWebRequest.Send(_httpClient, _logger);

                // Grab the Logs
                bookRequestLog = bookWebRequest.RequestLog;
                bookResponseLog = bookWebRequest.ResponseLog;

                // Grab the Response
                var response = _serializer.DeSerialize<RestelBookResponse>(bookWebRequest.ResponseXML);

                // save booking code
                // "00" indicates successful booking; anything else is a fail
                if (response.Parametros.Estado == "00")
                {
                    // "localizador_corto" (trans. short locator) needed for cancellation
                    propertyDetails.SourceSecondaryReference = response.Parametros.LocalizadorCorto;
                    reference = response.Parametros.Localizador;
                }
                else
                {
                    reference = "Failed";
                }
            }
            catch (Exception ex)
            {
                reference = "Failed";
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
            }
            finally
            {
                if (!nameSupplied)
                {
                    // Store the Pre-Book Logs
                    propertyDetails.Logs.AddNew(ThirdParties.RESTEL, "Restel PreBook Request", preBookRequestLog);
                    propertyDetails.Logs.AddNew(ThirdParties.RESTEL, "Restel PreBook Response", preBookResponseLog);
                }

                // Store the Book Logs
                propertyDetails.Logs.AddNew(ThirdParties.RESTEL, "Restel Book Request", bookRequestLog);
                propertyDetails.Logs.AddNew(ThirdParties.RESTEL, "Restel Book Response", bookResponseLog);
            }

            return reference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            Request? webRequest = null;

            try
            {
                // Build the Request
                var request = new RestelCancellationRequest
                {
                    Parametros =
                    {
                        LocalizadorLargo = propertyDetails.SourceReference,
                        LocalizadorCorto = propertyDetails.SourceSecondaryReference
                    }
                };

                // Send the Request
                webRequest = CreateRequest(_settings, propertyDetails, "Cancellation");
                webRequest.SetRequest(_serializer.Serialize(request));
                await webRequest.Send(_httpClient, _logger);

                // Grab the Response
                var response = _serializer.DeSerialize<RestelCancellationResponse>(webRequest.ResponseXML);

                // "00" indicates successful cancellation; anything else is a fail
                if (response.Parametros.Estado == "00")
                {
                    // sometimes returns a comment as well as a reference; last eight characters are the actual cancellation reference, so get total length and then select cancel reference
                    int commentLength = response.Parametros.LocalizadorBaja.Length;
                    thirdPartyCancellationResponse.TPCancellationReference = response.Parametros.LocalizadorBaja.Substring(commentLength - 9, 8);
                    thirdPartyCancellationResponse.Success = true;
                }
                else
                {
                    throw new Exception("Cancellation request did not return success");
                }
            }
            catch (Exception ex)
            {
                thirdPartyCancellationResponse.TPCancellationReference = "Failed";
                thirdPartyCancellationResponse.Success = false;
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
            }
            finally
            {
                // store the request and response xml on the property booking
                if (webRequest != null)
                {
                    propertyDetails.Logs.AddNew(ThirdParties.RESTEL, "Restel Cancellation Request", webRequest.RequestLog);
                    propertyDetails.Logs.AddNew(ThirdParties.RESTEL, "Restel Cancellation Response", webRequest.ResponseLog);
                }
            }

            return thirdPartyCancellationResponse;
        }

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            string codusu = _settings.Codusu(propertyDetails);
            Request? webRequest = null;

            try
            {
                // Build up the request
                var request = new RestelCancellationCostRequest
                {
                    Nombre = "Info gastos de cancelación",
                    Parametros =
                    {
                        Usuario = codusu,
                        Localizador = propertyDetails.SourceReference,
                        Idioma = 2
                    }
                };

                // Send the request to restel
                webRequest = CreateRequest(_settings, propertyDetails, "Cancellation Costs");
                webRequest.SetRequest(_serializer.Serialize(request));
                await webRequest.Send(_httpClient, _logger);

                // Grab the Response
                var response = _serializer.DeSerialize<RestelCancellationCostResponse>(webRequest.ResponseXML);

                // Pull out the cancellation Charges
                var helper = new List<CancellationHelper>();
                foreach (var politicaCanc in response.Parametros.PoliticaCanc)
                {
                    var cancellation = new CancellationHelper();
                    int nightsCharged = politicaCanc.NochesGasto.ToSafeInt();
                    decimal percentage = politicaCanc.EstComGasto.ToSafeDecimal();
                    cancellation.NightsBeforeArrival = politicaCanc.DiasAntelacion.ToSafeInt();
                    cancellation.StartDate = propertyDetails.ArrivalDate.AddDays(-cancellation.NightsBeforeArrival);

                    // Now work out if we are doing a % or a number of nights
                    if (nightsCharged > 0)
                    {
                        cancellation.Amount = propertyDetails.LocalCost / propertyDetails.Duration * nightsCharged;
                    }
                    else
                    {
                        cancellation.Amount = propertyDetails.LocalCost * (percentage / 100m);
                    }

                    // Add it to the collection if it could potentially apply
                    if (cancellation.StartDate <= DateTime.Now)
                    {
                        helper.Add(cancellation);
                    }
                }

                // Choose the cancellation policy we want to use.
                var finalCancellationPolicy = new CancellationHelper();
                int counter = 0;
                foreach (var cancellation in helper)
                {
                    if (counter == 0)
                    {
                        finalCancellationPolicy.StartDate = cancellation.StartDate;
                        finalCancellationPolicy.Amount = cancellation.Amount;
                        finalCancellationPolicy.NightsBeforeArrival = cancellation.NightsBeforeArrival;
                    }
                    else
                    {
                        int nightsBeforeArrival = cancellation.StartDate.Subtract(propertyDetails.ArrivalDate).Days.ToSafeInt();
                        if (cancellation.NightsBeforeArrival < finalCancellationPolicy.NightsBeforeArrival && cancellation.NightsBeforeArrival >= nightsBeforeArrival)
                        {
                            finalCancellationPolicy.StartDate = cancellation.StartDate;
                            finalCancellationPolicy.Amount = cancellation.Amount;
                            finalCancellationPolicy.NightsBeforeArrival = cancellation.NightsBeforeArrival;
                        }
                    }

                    counter += 1;
                }

                // Load the return class
                var cancellationFeeResult = new ThirdPartyCancellationFeeResult
                    {
                        Amount = finalCancellationPolicy.Amount, CurrencyCode = await _support.TPCurrencyLookupAsync(propertyDetails.Source, propertyDetails.CurrencyCode),
                        Success = true
                    };

                return cancellationFeeResult;
            }
            catch (Exception)
            {
                return new ThirdPartyCancellationFeeResult();
            }
            finally
            {
                if (webRequest != null)
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, "Cancellation Costs", webRequest.RequestLog, webRequest.ResponseLog);
                }
            }
        }

        private class CancellationHelper
        {
            public DateTime StartDate;
            public decimal Amount;
            public int NightsBeforeArrival;
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
    }
}
