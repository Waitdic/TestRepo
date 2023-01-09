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
    using iVectorOne.SDK.V2.PropertyPrebook;

    /// <summary>
    ///   <para>The service responsible for handling the pre book</para>
    /// </summary>
    public class PrebookService : IPrebookService
    {
        /// <summary>The property details factory</summary>
        private readonly IPropertyDetailsFactory _propertyDetailsFactory;

        /// <summary>The log repository</summary>
        private readonly IAPILogRepository _logRepository;

        /// <summary>Factory that creates the third party class</summary>
        private readonly IThirdPartyFactory _thirdPartyFactory;

        /// <summary>The factory responsible for building the pre book response</summary>
        private readonly IPropertyPrebookResponseFactory _responseFactory;

        /// <summary>The supplier log repository</summary>
        private readonly ISupplierLogRepository _supplierLogRepository;

        /// <summary>Initializes a new instance of the <see cref="PrebookService" /> class.</summary>
        /// <param name="propertyDetailsFactory">The property details factory.</param>
        /// <param name="logRepository">Repository for saving pre book logs to the database</param>
        /// <param name="thirdPartyFactory">Factory that creates the correct third party class</param>
        /// <param name="responseFactory">The factory responsible for building the pre book response</param>
        /// <param name="supplierLogRepository">Repository for saving supplier logs to the database</param>
        public PrebookService(
            IPropertyDetailsFactory propertyDetailsFactory,
            IAPILogRepository logRepository,
            IThirdPartyFactory thirdPartyFactory,
            IPropertyPrebookResponseFactory responseFactory,
            ISupplierLogRepository supplierLogRepository)
        {
            _propertyDetailsFactory = Ensure.IsNotNull(propertyDetailsFactory, nameof(propertyDetailsFactory));
            _logRepository = Ensure.IsNotNull(logRepository, nameof(logRepository));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _responseFactory = Ensure.IsNotNull(responseFactory, nameof(responseFactory));
            _supplierLogRepository = Ensure.IsNotNull(supplierLogRepository, nameof(supplierLogRepository));
        }

        /// <inheritdoc/>
        public async Task<Response> PrebookAsync(Request prebookRequest)
        {
            Response response = null!;
            bool requestValid = true;
            bool success = false;
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

                    requestValid = false;
                }
                else
                {
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                        propertyDetails.Source,
                        prebookRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        success = await thirdParty.PreBookAsync(propertyDetails);

                        if (success)
                        {
                            response = await _responseFactory.CreateAsync(propertyDetails);
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
                await _logRepository.LogPrebookAsync(prebookRequest, response!, success);
                await _supplierLogRepository.LogPrebookRequestsAsync(propertyDetails);

                if (requestValid && !success)
                {
                    response.Warnings = new List<string>() { "Supplier prebook failed" };
                }
                else if (requestValid)
                {
                    response.Warnings = null!;
                }
            }

            return response;
        }
    }
}