namespace iVectorOne.Services.Transfer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.TransferBook;

    /// <summary>Service responsible for talking to the third party and handling the book, and returning book responses.</summary>
    public class BookService : IBookService
    {
        /// <summary>The transfer details factory</summary>
        private readonly ITransferDetailsFactory _transferDetailsFactory;

        /// <summary>The log repository</summary>
        private readonly ITransferAPILogRepository _logRepository;

        /// <summary>Factory that creates the third party class</summary>
        private readonly ITransferThirdPartyFactory _thirdPartyFactory;

        /// <summary>Creates a book response using information from the transfer details</summary>
        private readonly ITransferBookResponseFactory _responseFactory;

        /// <summary>The supplier log repository</summary>
        private readonly ITransferSupplierLogRepository _supplierLogRepository;

        /// <summary>The transfer booking repository</summary>
        private readonly ITransferBookingRepository _bookingRepository;

        /// <summary>Initializes a new instance of the <see cref="BookService" /> class.</summary>
        /// <param name="transferDetailsFactory">The transfer details factory.</param>
        /// <param name="logRepository">Repository for saving pre book logs to the database</param>
        /// <param name="thirdPartyFactory">Factory that creates the correct third party class</param>
        /// <param name="responseFactory">Creates a book response using information from the transfer details</param>
        /// <param name="supplierLogRepository">Repository for saving supplier logs to the database</param>
        /// <param name="bookingRepository">Repository for saving the booking to the database</param>
        public BookService(
            ITransferDetailsFactory transferDetailsFactory,
            ITransferAPILogRepository logRepository,
            ITransferThirdPartyFactory thirdPartyFactory,
            ITransferBookResponseFactory responseFactory,
            ITransferSupplierLogRepository supplierLogRepository,
            ITransferBookingRepository bookingRepository)
        {
            _transferDetailsFactory = Ensure.IsNotNull(transferDetailsFactory, nameof(transferDetailsFactory));
            _logRepository = Ensure.IsNotNull(logRepository, nameof(logRepository));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _responseFactory = Ensure.IsNotNull(responseFactory, nameof(responseFactory));
            _supplierLogRepository = Ensure.IsNotNull(supplierLogRepository, nameof(supplierLogRepository));
            _bookingRepository = Ensure.IsNotNull(bookingRepository, nameof(bookingRepository));
        }

        /// <inheritdoc/>
        public async Task<Response> BookAsync(Request bookRequest)
        {
            Response response = null!;
            bool requestValid = true;
            bool success = false;
            var transferDetails = new TransferDetails();
            var bookDateAndTime = DateTime.Now;

            try
            {
                transferDetails = await _transferDetailsFactory.CreateAsync(bookRequest);

                if (transferDetails.Warnings.Any())
                {
                    response = new Response()
                    {
                        Warnings = transferDetails.Warnings.Select(w => w.Text).ToList()
                    };

                    requestValid = false;
                }
                else
                {
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                                        transferDetails.Source,
                                        bookRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == transferDetails.Source));

                    if (thirdParty != null)
                    {
                        bookDateAndTime = DateTime.Now;

                        requestValid = thirdParty.ValidateSettings(transferDetails);
                        if (requestValid)
                        {
                            var supplierReference = await thirdParty.BookAsync(transferDetails);
                            transferDetails.SupplierReference = supplierReference;
                            success = supplierReference.ToLower() != "failed";
                        }
                        if (success)
                        {
                            response = _responseFactory.Create(transferDetails);
                        }
                        else
                        {
                            response = new Response()
                            {
                                Warnings = transferDetails.Warnings.Select(w => $"{w.Title}, {w.Text}").ToList()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Warnings.Add(ex.ToString());
            }
            finally
            {
                int transferBookingId = await _bookingRepository.StoreBookingAsync(transferDetails, requestValid, success);
                transferDetails.TransferBookingID = transferBookingId;
                bookRequest.BookingID = transferBookingId;

                await _logRepository.LogBookAsync(bookRequest, response!, success);
                await _supplierLogRepository.LogBookRequestsAsync(transferDetails);

                if (requestValid && !success)
                {
                    response.Warnings = new List<string>() { "Supplier book failed" };
                }
                else if (requestValid)
                {
                    response.Warnings = null!;
                }
            }

            return response!;
        }
    }
}