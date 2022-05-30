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

        /// <summary>Books the specified book request.</summary>
        /// <param name="bookRequest">The book request.</param>
        /// <param name="user">The user.</param>
        /// <returns>
        /// A book response
        /// </returns>
        public async Task<Response> BookAsync(Request bookRequest, User user)
        {
            Response response = null!;
            var exceptionString = string.Empty;
            var success = true;
            var propertyDetails = new PropertyDetails();

            try
            {
                propertyDetails = await this.propertyDetailsFactory.CreateAsync(bookRequest, user);
                this.referenceValidator.ValidateBook(propertyDetails, bookRequest);

                if (propertyDetails.Warnings.Any())
                {
                    response = new Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };
                }
                else
                {
                    var thirdParty = this.thirdPartyFactory.CreateFromSource(
                                        propertyDetails.Source,
                                        user.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        var supplierReference = thirdParty.Book(propertyDetails);
                        propertyDetails.SupplierSourceReference = supplierReference;
                        success = supplierReference.ToLower() != "failed";
                    }

                    if (success)
                    {
                        response = this.responseFactory.Create(propertyDetails);
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

                this.logRepository.LogBook(bookRequest, response!, user, exceptionString);
            }

            return response!;
        }
    }
}