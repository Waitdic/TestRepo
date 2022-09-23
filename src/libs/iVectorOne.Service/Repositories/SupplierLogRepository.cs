using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Intuitive;
using iVectorOne.Lookups;
using iVectorOne.Models;
using iVectorOne.Models.Property.Booking;
using iVectorOne.Models.SupplierLog;
using iVectorOne.SDK.V2.PropertyBook;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace iVectorOne.Repositories
{
    public class SupplierLogRepository : ISupplierLogRepository
    {
        private readonly string _connectionString;
        private readonly ICurrencyLookupRepository _currencyRepository;
        private readonly ITPSupport _support;
        private readonly ILogger<SupplierLogRepository> _logger;

        public SupplierLogRepository(string connectionString, ICurrencyLookupRepository currencyRepository, ITPSupport support, ILogger<SupplierLogRepository> logger)
        {
            _connectionString = connectionString;
            _currencyRepository = Ensure.IsNotNull(currencyRepository, nameof(currencyRepository));
            _support = Ensure.IsNotNull(support, nameof(support));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task LogPrebookAsync(SupplierLogs supplierLogs, Account account, DateTime prebookDateAndTime)
        {
            try
            {
                await using var con = new SqlConnection(_connectionString);

                foreach (var log in supplierLogs.OfType<SupplierPrebookLog>())
                {
                    await con.ExecuteAsync(
                    @"INSERT INTO [PrebookLog] (
                             [AccountName]
                            ,[AccountID]
                            ,[System]
                            ,[SupplierName]
                            ,[SupplierID]
                            ,[PrebookDateAndTime]
                            ,[ResponseTime]
                            ,[Successful]
                            ,[RequestPayload]
                            ,[ResponsePayload]
                        ) VALUES (
                             @AccountName
                            ,@AccountID
                            ,@System
                            ,@SupplierName
                            ,@SupplierID
                            ,@PrebookDateAndTime
                            ,@ResponseTime
                            ,@Successful
                            ,@RequestPayload
                            ,@ResponsePayload)",
                    new
                    {
                        AccountName = account.Login,
                        account.AccountID,
                        System = account.Environment.ToString(),
                        log.SupplierName,
                        log.SupplierId,
                        PrebookDateAndTime = prebookDateAndTime,
                        ResponseTime = log.Request.RequestDuration,
                        Successful = log.Request.Success,
                        RequestPayload = log.Request.RequestString,
                        ResponsePayload = log.Request.ResponseString

                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Supplier Prebook log exception");
            }
        }

        public async Task LogBookAsync(SupplierLogs supplierLogs, Account account, DateTime bookDateAndTime,
            PropertyDetails propertyDetails, Response response, bool success)
        {
            try
            {
                var currencyId = await _support.ISOCurrencyIDLookupAsync(propertyDetails.ISOCurrencyCode);
                var exchangeRate = await _currencyRepository.GetExchangeRateFromISOCurrencyIDAsync(currencyId);

                await using var con = new SqlConnection(_connectionString);

                foreach (var log in supplierLogs.OfType<SupplierBookLog>())
                {
                    await con.ExecuteAsync(
                    @"INSERT INTO [BookLog] (
                             [AccountName]
                            ,[AccountID]
                            ,[System]
                            ,[SupplierName]
                            ,[SupplierID]
                            ,[PropertyID]
                            ,[BookDateAndTime]
                            ,[ResponseTime]
                            ,[Successful]
                            ,[RequestPayload]
                            ,[ResponsePayload]
                            ,[BookingReference]
                            ,[SupplierBookingReference]
                            ,[LeadGuestName]
                            ,[DepartureDate]
                            ,[Duration]
                            ,[TotalPrice]
                            ,[Currency]
                            ,[EstimatedGBPPrice]
                        ) VALUES (
                             @AccountName
                            ,@AccountID
                            ,@System
                            ,@SupplierName
                            ,@SupplierID
                            ,@PropertyID
                            ,@BookDateAndTime
                            ,@ResponseTime
                            ,@Successful,
                            ,@RequestPayload
                            ,@ResponsePayload
                            ,@BookingReference
                            ,@SupplierBookingReference
                            ,@LeadGuestName
                            ,@DepartureDate
                            ,@Duration
                            ,@TotalPrice
                            ,@Currency
                            ,@EstimatedGBPPrice)",
                    new
                    {
                        AccountName = account.Login,
                        account.AccountID,
                        System = account.Environment.ToString(),
                        log.SupplierName,
                        log.SupplierId,
                        propertyDetails.PropertyID,
                        BookDateAndTime = bookDateAndTime,
                        ResponseTime = log.Request.RequestDuration,
                        Successful = success,
                        RequestPayload = log.Request.RequestString,
                        ResponsePayload = log.Request.ResponseString,
                        propertyDetails.BookingReference,
                        response.SupplierBookingReference,
                        LeadGuestFirstName = $"{propertyDetails.LeadGuestFirstName} {propertyDetails.LeadGuestLastName}",
                        propertyDetails.DepartureDate,
                        propertyDetails.Duration,
                        TotalPrice = propertyDetails.LocalCost,
                        Currency = propertyDetails.ISOCurrencyCode,
                        EstimatedGBPPrice = propertyDetails.LocalCost * exchangeRate
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Supplier Book log exception");
            }
        }
    }
}
