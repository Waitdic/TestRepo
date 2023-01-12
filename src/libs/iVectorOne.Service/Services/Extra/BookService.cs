namespace iVectorOne.Services.Extra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Models.Extra;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.ExtraBook;

    /// <summary>Service responsible for talking to the third party and handling the book, and returning book responses.</summary>
    public class BookService : IBookService
    {
        /// <summary>The extra details factory</summary>
        private readonly IExtraDetailsFactory _extraDetailsFactory;

        /// <summary>The log repository</summary>
        private readonly IExtraAPILogRepository _logRepository;

        /// <summary>Factory that creates the third party class</summary>
        private readonly IExtraThirdPartyFactory _thirdPartyFactory;

        /// <summary>Creates a book response using information from the extra details</summary>
        private readonly IExtraBookResponseFactory _responseFactory;

        /// <summary>The supplier log repository</summary>
        private readonly IExtraSupplierLogRepository _supplierLogRepository;

        /// <summary>The extra booking repository</summary>
        private readonly IExtraBookingRepository _bookingRepository;

        /// <summary>Initializes a new instance of the <see cref="BookService" /> class.</summary>
        /// <param name="extraDetailsFactory">The extra details factory.</param>
        /// <param name="logRepository">Repository for saving pre book logs to the database</param>
        /// <param name="thirdPartyFactory">Factory that creates the correct third party class</param>
        /// <param name="responseFactory">Creates a book response using information from the extra details</param>
        /// <param name="supplierLogRepository">Repository for saving supplier logs to the database</param>
        /// <param name="bookingRepository">Repository for saving the booking to the database</param>
        public BookService(
            IExtraDetailsFactory extraDetailsFactory,
            IExtraAPILogRepository logRepository,
            IExtraThirdPartyFactory thirdPartyFactory,
            IExtraBookResponseFactory responseFactory,
            IExtraSupplierLogRepository supplierLogRepository,
            IExtraBookingRepository bookingRepository)
        {
            _extraDetailsFactory = Ensure.IsNotNull(extraDetailsFactory, nameof(extraDetailsFactory));
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
            var extraDetails = new ExtraDetails();
            var bookDateAndTime = DateTime.Now;

            try
            {
                extraDetails = await _extraDetailsFactory.CreateAsync(bookRequest);

                if (extraDetails.Warnings.Any())
                {
                    response = new Response()
                    {
                        Warnings = extraDetails.Warnings.Select(w => w.Text).ToList()
                    };

                    requestValid = false;
                }
                else
                {
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                                        extraDetails.Source,
                                        bookRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == extraDetails.Source));

                    if (thirdParty != null)
                    {
                        bookDateAndTime = DateTime.Now;

                        requestValid = thirdParty.ValidateSettings(extraDetails);
                        if (requestValid)
                        {
                            var supplierReference = await thirdParty.BookAsync(extraDetails);
                            extraDetails.SupplierReference = supplierReference;
                            success = supplierReference.ToLower() != "failed";
                        }
                        if (success)
                        {
                            response = _responseFactory.Create(extraDetails);
                        }
                        else
                        {
                            response = new Response()
                            {
                                Warnings = extraDetails.Warnings.Select(w => $"{w.Title}, {w.Text}").ToList()
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
                int transferBookingId = await _bookingRepository.StoreBookingAsync(extraDetails, requestValid, success);
                extraDetails.TransferBookingID = transferBookingId;
                bookRequest.BookingID = transferBookingId;

                await _logRepository.LogBookAsync(bookRequest, response!, success);
                await _supplierLogRepository.LogBookRequestsAsync(extraDetails);

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