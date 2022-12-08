namespace Jobs
{
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Scheduling;
    using iVectorOne.Models;
    using iVectorOne.Models.Logging;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.PropertyBook;
    using iVectorOne.Services;
    using System.Text.Json;
    using System.Threading.Tasks;

    [ScheduledJob("Migrate To Booking Store", trigger: TriggerType.Once, description: "One off job to populate the Booking table from the APILog table")]
    public class MigrateToBookingStore : IScheduledJob
    {
        private readonly ISql _sql;
        private readonly ITokenService _tokenService;
        private readonly ICurrencyLookupRepository _currencyRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IPropertyContentRepository _contentRepository;

        public MigrateToBookingStore(
            ISqlFactory sqlFactory,
            ITokenService tokenService,
            ICurrencyLookupRepository currencyRepository,
            IBookingRepository bookingRepository,
            IPropertyContentRepository contentRepository)
        {
            _sql = Ensure.IsNotNull(sqlFactory, nameof(sqlFactory)).CreateSqlContext();
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _currencyRepository = Ensure.IsNotNull(currencyRepository, nameof(currencyRepository));
            _bookingRepository = Ensure.IsNotNull(bookingRepository, nameof(bookingRepository));
            _contentRepository = Ensure.IsNotNull(contentRepository, nameof(contentRepository));
        }

        public async Task ExecuteAsync(IScheduledJobContext context)
        {
            var logs = (await _sql.ReadAllAsync<APILogWrapper>("select * from APILog where Type != 'Prebook' and (BookingID = 0 or Type != 'Book')")).ToList();
            var accounts = await _sql.ReadAllAsync<Account>("select * from Account");

            foreach (var log in logs)
            {
                if (log.Type == LogType.Book)
                {
                    log.Request = DeserializeRequest(log);
                    log.Response = DeserializeResponse(log);
                }
                else
                {
                    log.SupplierBookingReference = GetSupplierBookingReference(log);
                }
            }

            await MigrateSuccessfulBookingsAsync(logs, accounts);

            await MigrateFailedBookingsAsync(logs, accounts);
        }

        private async Task MigrateSuccessfulBookingsAsync(List<APILogWrapper> logs, Account[] accounts)
        {
            foreach (var bookLog in logs.Where(l => l.Type == LogType.Book && l.Success && l.Request is not null))
            {
                var request = bookLog.Request!;
                var response = bookLog.Response;
                var account = accounts.FirstOrDefault(a => a.AccountID == bookLog.AccountID);
                var decodedBookingToken = _tokenService.DecodePropertyToken(request.BookingToken);

                int propertyId = decodedBookingToken?.PropertyID ?? 0;
                int supplierId = 0;

                if (propertyId > 0 && account is not null)
                {
                    var propertyContent = await _contentRepository.GetContentforPropertyAsync(propertyId, account, string.Empty);
                    supplierId = propertyContent.SupplierID;
                }
                decimal totalPrice = ExtractTotalPriceFromRoomTokens(request);
                decimal exchangeRate = 0;
                int isoCurrencyId = decodedBookingToken?.ISOCurrencyID ?? 0;

                if (isoCurrencyId > 0)
                {
                    exchangeRate = await _currencyRepository.GetExchangeRateFromISOCurrencyIDAsync(isoCurrencyId);
                }

                var linkedFailedBookingLogs = logs
                    .Where(l => l.Type == LogType.Book &&
                        l.AccountID == bookLog.AccountID &&
                        l.Request?.BookingReference == bookLog.Request?.BookingReference &&
                        !l.Success &&
                        l.BookingID == 0 &&
                        l.APILogID < bookLog.APILogID);

                var linkedCancelLogs = logs
                    .Where(l => l.Type != LogType.Book &&
                        l.AccountID == bookLog.AccountID &&
                        l.SupplierBookingReference == bookLog.Response?.SupplierBookingReference);

                bool cancelled = linkedCancelLogs.Any(l => l.Type == LogType.Cancel && l.Success);

                var booking = new Booking()
                {
                    AccountID = bookLog.AccountID,
                    BookingDateTime = bookLog.Time,
                    BookingID = bookLog.BookingID,
                    BookingReference = request.BookingReference,
                    DepartureDate = decodedBookingToken?.ArrivalDate ?? new DateTime(1900, 1, 1),
                    Duration = decodedBookingToken?.Duration ?? 0,
                    EstimatedGBPPrice = totalPrice * exchangeRate,
                    ISOCurrencyID = decodedBookingToken?.ISOCurrencyID ?? 0,
                    LeadGuestName = $"{request.LeadCustomer.CustomerTitle} {request.LeadCustomer.CustomerFirstName} {request.LeadCustomer.CustomerLastName}",
                    PropertyID = propertyId,
                    Status = cancelled ? BookingStatus.Cancelled :
                        bookLog.Success ? BookingStatus.Live : BookingStatus.Failed,
                    SupplierBookingReference = response?.SupplierBookingReference ?? string.Empty,
                    SupplierID = supplierId,
                    TotalPrice = totalPrice,
                };

                int bookingId = await _bookingRepository.StoreBookingAsync(booking);

                await UpdateLogBookingIDAsync(bookingId, bookLog);

                foreach (var log in linkedFailedBookingLogs)
                {
                    await UpdateLogBookingIDAsync(bookingId, log);
                }

                foreach (var linkedLog in linkedCancelLogs)
                {
                    await UpdateLogBookingIDAsync(bookingId, linkedLog);
                }
            }
        }

        private async Task MigrateFailedBookingsAsync(List<APILogWrapper> logs, Account[] accounts)
        {
            foreach (var bookLog in logs.Where(l => l.Type == LogType.Book && !l.Success && l.Request is not null))
            {
                var request = bookLog.Request!;
                var account = accounts.FirstOrDefault(a => a.AccountID == bookLog.AccountID)!;
                var booking = await _bookingRepository.GetBookingAsync(request.BookingReference, account);

                if (booking is not null)
                {
                    await UpdateLogBookingIDAsync(booking.BookingID, bookLog);

                    var linkedCancelLogs = logs
                        .Where(l => l.Type != LogType.Book &&
                            l.AccountID == bookLog.AccountID &&
                            l.SupplierBookingReference == bookLog.Response?.SupplierBookingReference);

                    foreach (var linkedLog in linkedCancelLogs)
                    {
                        await UpdateLogBookingIDAsync(booking.BookingID, linkedLog);
                    }
                }
            }
        }

        private static Request? DeserializeRequest(APILog bookLog)
        {
            Request? request = null;
            try
            {
                request = JsonSerializer.Deserialize<Request>(bookLog.RequestLog);
            }
            catch
            {
            }

            return request;
        }

        private static Response? DeserializeResponse(APILog bookLog)
        {
            Response? response = null;
            try
            {
                response = JsonSerializer.Deserialize<Response>(bookLog.ResponseLog);
            }
            catch
            {
            }

            return response;
        }

        private string? GetSupplierBookingReference(APILog log)
        {
            string reference = null!;
            try
            {
                var request = JsonSerializer.Deserialize<iVectorOne.SDK.V2.PropertyCancel.Request>(log.RequestLog);
                reference = request!.SupplierBookingReference;
            }
            catch
            {
            }

            return reference;
        }

        private decimal ExtractTotalPriceFromRoomTokens(Request request)
        {
            decimal totalPrice = 0;

            foreach (var room in request.RoomBookings)
            {
                var decodedRoomToken = _tokenService.DecodeRoomToken(room.RoomBookingToken);
                if (decodedRoomToken is not null)
                {
                    totalPrice += decodedRoomToken.LocalCost;
                }
            }

            return totalPrice;
        }

        private async Task UpdateLogBookingIDAsync(int bookingId, APILog log)
        {
            log.BookingID = bookingId;
            await _sql.ExecuteAsync(
                "update APILog set BookingID = @bookingId where APILogID = @apiLogId",
                new CommandSettings()
                    .WithParameters(new
                    {
                        apiLogId = log.APILogID,
                        bookingId,
                    }));
        }


        private class APILogWrapper : APILog
        {
            public Request? Request { get; set; }
            public Response? Response { get; set; }
            public string? SupplierBookingReference { get; set; }
        }
    }
}