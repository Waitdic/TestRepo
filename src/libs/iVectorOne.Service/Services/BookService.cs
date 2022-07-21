namespace ThirdParty.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ThirdParty;
    using ThirdParty.Factories;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Repositories;
    using ThirdParty.SDK.V2.PropertyBook;

    /// <summary>Service responsible for talking to the third party and handling the book, and returning book responses.</summary>
    public class BookService : IBookService
    {
        /// <summary>The property details factory</summary>
        private readonly IPropertyDetailsFactory propertyDetailsFactory;

        /// <summary>The log repository</summary>
        private readonly IBookingLogRepository logRepository;

        /// <summary>Factory that creates the third party class</summary>
        private readonly IThirdPartyFactory thirdPartyFactory;

        /// <summary>Creates a book response using information from the property details</summary>
        private readonly IPropertyBookResponseFactory responseFactory;

        /// <summary>The reference validator</summary>
        private readonly ISuppierReferenceValidator referenceValidator;

        /// <summary>Initializes a new instance of the <see cref="BookService" /> class.</summary>
        /// <param name="propertyDetailsFactory">The property details factory.</param>
        /// <param name="logRepository">Repository for saving pre book logs to the database</param>
        /// <param name="thirdPartyFactory">Factory that creates the correct third party class</param>
        /// <param name="responseFactory">Creates a book response using information from the property details</param>
        /// <param name="referenceValidator">Validates if the right supplier references have been sent for the supplier</param>
        public BookService(
            IPropertyDetailsFactory propertyDetailsFactory,
            IBookingLogRepository logRepository,
            IThirdPartyFactory thirdPartyFactory,
            IPropertyBookResponseFactory responseFactory,
            ISuppierReferenceValidator referenceValidator)
        {
            this.propertyDetailsFactory = propertyDetailsFactory;
            this.logRepository = logRepository;
            this.thirdPartyFactory = thirdPartyFactory;
            this.responseFactory = responseFactory;
            this.referenceValidator = referenceValidator;
        }

        /// <inheritdoc/>
        public async Task<Response> BookAsync(Request bookRequest)
        {
            Response response = null!;
            var exceptionString = string.Empty;
            bool success = true;
            var propertyDetails = new PropertyDetails();

            try
            {
                propertyDetails = await this.propertyDetailsFactory.CreateAsync(bookRequest);
                this.referenceValidator.ValidateBook(propertyDetails, bookRequest);

                if (propertyDetails.Warnings.Any())
                {
                    response = new Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };

                    success = false;
                }
                else
                {
                    var thirdParty = this.thirdPartyFactory.CreateFromSource(
                                        propertyDetails.Source,
                                        bookRequest.User.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        var supplierReference = await thirdParty.BookAsync(propertyDetails);
                        propertyDetails.SupplierSourceReference = supplierReference;
                        success = supplierReference.ToLower() != "failed";
                    }

                    if (success)
                    {
                        response = this.responseFactory.Create(propertyDetails);
                    }
                    else
                    {
                        exceptionString = "Suppplier book failed";
                        response = new Response()
                        {
                            Warnings = new System.Collections.Generic.List<string>() { exceptionString }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionString = ex.ToString();
            }
            finally
            {
                if (!success && propertyDetails.Warnings.Any())
                {
                    exceptionString += string.Join(Environment.NewLine, propertyDetails.Warnings);
                }

                await this.logRepository.LogBookAsync(bookRequest, response!, bookRequest.User, exceptionString);
            }

            return response!;
        }
    }
}