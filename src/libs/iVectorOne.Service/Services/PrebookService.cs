namespace iVectorOne.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.PropertyPrebook;

    /// <summary>
    ///   <para>The service responsible for handling the pre book</para>
    /// </summary>
    public class PrebookService : IPrebookService
    {
        /// <summary>The property details factory</summary>
        private readonly IPropertyDetailsFactory _propertyDetailsFactory;

        /// <summary>The log repository</summary>
        private readonly IBookingLogRepository _logRepository;

        /// <summary>Factory that creates the third party class</summary>
        private readonly IThirdPartyFactory _thirdPartyFactory;

        /// <summary>The factory responsible for building the pre book response</summary>
        private readonly IPropertyPrebookResponseFactory _responseFactory;

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
            _propertyDetailsFactory = Ensure.IsNotNull(propertyDetailsFactory, nameof(propertyDetailsFactory));
            _logRepository = Ensure.IsNotNull(logRepository, nameof(logRepository));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _responseFactory = Ensure.IsNotNull(responseFactory, nameof(responseFactory));
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
                propertyDetails = await _propertyDetailsFactory.CreateAsync(prebookRequest);

                if (propertyDetails.Warnings.Any())
                {
                    response = new Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };
                }
                else
                {
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                        propertyDetails.Source,
                        prebookRequest.User.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        success = await thirdParty.PreBookAsync(propertyDetails);
                    }

                    if (success)
                    {
                        response = await _responseFactory.CreateAsync(propertyDetails);
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

                await _logRepository.LogPrebookAsync(prebookRequest, response!, prebookRequest.User, exceptionString);
            }

            return response;
        }
    }
}
