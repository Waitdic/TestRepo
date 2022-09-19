namespace iVectorOne.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.PropertyBook;

    /// <summary>Service responsible for talking to the third party and handling the book, and returning book responses.</summary>
    public class BookService : IBookService
    {
        /// <summary>The property details factory</summary>
        private readonly IPropertyDetailsFactory _propertyDetailsFactory;

        /// <summary>The log repository</summary>
        private readonly IBookingLogRepository _logRepository;

        /// <summary>Factory that creates the third party class</summary>
        private readonly IThirdPartyFactory _thirdPartyFactory;

        /// <summary>Creates a book response using information from the property details</summary>
        private readonly IPropertyBookResponseFactory _responseFactory;

        /// <summary>The reference validator</summary>
        private readonly ISuppierReferenceValidator _referenceValidator;

        /// <summary>The supplier log repository</summary>
        private readonly ISupplierLogRepository _supplierLogRepository;

        /// <summary>Initializes a new instance of the <see cref="BookService" /> class.</summary>
        /// <param name="propertyDetailsFactory">The property details factory.</param>
        /// <param name="logRepository">Repository for saving pre book logs to the database</param>
        /// <param name="thirdPartyFactory">Factory that creates the correct third party class</param>
        /// <param name="responseFactory">Creates a book response using information from the property details</param>
        /// <param name="referenceValidator">Validates if the right supplier references have been sent for the supplier</param>
        /// <param name="supplierLogRepository">Repository for saving supplier pre book logs to the database</param>
        public BookService(
            IPropertyDetailsFactory propertyDetailsFactory,
            IBookingLogRepository logRepository,
            IThirdPartyFactory thirdPartyFactory,
            IPropertyBookResponseFactory responseFactory,
            ISuppierReferenceValidator referenceValidator,
            ISupplierLogRepository supplierLogRepository)
        {
            _propertyDetailsFactory = Ensure.IsNotNull(propertyDetailsFactory, nameof(propertyDetailsFactory));
            _logRepository = Ensure.IsNotNull(logRepository, nameof(logRepository));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _responseFactory = Ensure.IsNotNull(responseFactory, nameof(responseFactory));
            _referenceValidator = Ensure.IsNotNull(referenceValidator, nameof(referenceValidator));
            _supplierLogRepository = Ensure.IsNotNull(supplierLogRepository, nameof(supplierLogRepository));
        }

        /// <inheritdoc/>
        public async Task<Response> BookAsync(Request bookRequest)
        {
            Response response = null!;
            bool requestValid = true;
            bool success = false;
            var propertyDetails = new PropertyDetails();
            var bookDateAndTime = DateTime.Now;

            try
            {
                propertyDetails = await _propertyDetailsFactory.CreateAsync(bookRequest);
                _referenceValidator.ValidateBook(propertyDetails, bookRequest);

                if (propertyDetails.Warnings.Any())
                {
                    response = new Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };

                    requestValid = false;
                }
                else
                {
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                                        propertyDetails.Source,
                                        bookRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        bookDateAndTime = DateTime.Now;
                        var supplierReference = await thirdParty.BookAsync(propertyDetails);
                        propertyDetails.SupplierSourceReference = supplierReference;
                        success = supplierReference.ToLower() != "failed";

                        if (success)
                        {
                            response = _responseFactory.Create(propertyDetails);
                        }
                        else
                        {
                            response = new Response()
                            {
                                Warnings = propertyDetails.Warnings.Select(w => $"{w.Title}, {w.Text}").ToList()
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
                await _logRepository.LogBookAsync(bookRequest, response!, success);
                await _supplierLogRepository.LogBookAsync(propertyDetails.SupplierLogs, bookRequest.Account, bookDateAndTime, propertyDetails, response, success);

                if (requestValid && !success)
                {
                    response.Warnings = new List<string>() { "Suppplier book failed" };
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