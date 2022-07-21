namespace iVectorOne.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using iVectorOne;
    using iVectorOne.Factories;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.PropertyPrebook;

    /// <summary>
    ///   <para>The service responsible for handling the pre book</para>
    /// </summary>
    public class PrebookService : IPrebookService
    {
        /// <summary>The property details factory</summary>
        private readonly IPropertyDetailsFactory propertyDetailsFactory;

        /// <summary>The log repository</summary>
        private readonly IBookingLogRepository logRepository;

        /// <summary>Factory that creates the third party class</summary>
        private readonly IThirdPartyFactory thirdPartyFactory;

        /// <summary>The factory responsible for building the pre book response</summary>
        private readonly IPropertyPrebookResponseFactory responseFactory;

        /// <summary>Initializes a new instance of the <see cref="PrebookService" /> class.</summary>
        /// <param name="propertyDetailsFactory">The property details factory.</param>
        /// <param name="logRepository">Repository for saving pre book logs to the database</param>
        /// <param name="thirdPartyFactory">Factory that creates the correct third party class</param>
        /// <param name="responseFactory">The factory responsible for building the pre book response</param>
        public PrebookService(
            IPropertyDetailsFactory propertyDetailsFactory,
            IBookingLogRepository logRepository,
            IThirdPartyFactory thirdPartyFactory,
            IPropertyPrebookResponseFactory responseFactory)
        {
            this.propertyDetailsFactory = propertyDetailsFactory;
            this.logRepository = logRepository;
            this.thirdPartyFactory = thirdPartyFactory;
            this.responseFactory = responseFactory;
        }

        /// <inheritdoc/>
        public async Task<Response> PrebookAsync(Request prebookRequest)
        {
            Response response = null!;
            string exceptionString = string.Empty;
            bool success = true;
            var propertyDetails = new PropertyDetails();

            try
            {
                propertyDetails = await this.propertyDetailsFactory.CreateAsync(prebookRequest);

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
                        prebookRequest.User.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        success = await thirdParty.PreBookAsync(propertyDetails);
                    }

                    if (success)
                    {
                        response = await this.responseFactory.CreateAsync(propertyDetails);
                    }
                    else
                    {
                        exceptionString = "Suppplier prebook failed";
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
                if(!success && propertyDetails.Warnings.Any())
                {
                    exceptionString += string.Join(Environment.NewLine, propertyDetails.Warnings);
                }

                await this.logRepository.LogPrebookAsync(prebookRequest, response!, prebookRequest.User, exceptionString);
            }

            return response;
        }
    }
}
