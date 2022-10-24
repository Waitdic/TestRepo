namespace iVectorOne.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.Runtime.Internal.Util;
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.PropertyPrebook;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///   <para>The service responsible for handling the pre book</para>
    /// </summary>
    public class PrebookService : IPrebookService
    {
        private readonly ILogger<PrebookService> _logger;

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
            ISupplierLogRepository supplierLogRepository,
            ILogger<PrebookService> logger)
        {
            _propertyDetailsFactory = Ensure.IsNotNull(propertyDetailsFactory, nameof(propertyDetailsFactory));
            _logRepository = Ensure.IsNotNull(logRepository, nameof(logRepository));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _responseFactory = Ensure.IsNotNull(responseFactory, nameof(responseFactory));
            _supplierLogRepository = Ensure.IsNotNull(supplierLogRepository, nameof(supplierLogRepository));
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Response> PrebookAsync(Request prebookRequest)
        {
            Response response = null!;
            bool requestValid = true;
            bool success = false;
            var propertyDetails = new PropertyDetails();
            var prebookDateAndTime = DateTime.Now;

            _logger.LogInformation("** PrebookAsync Start");

            try
            {
                _logger.LogInformation("** PrebookAsync before propertyDetails");
                propertyDetails = await _propertyDetailsFactory.CreateAsync(prebookRequest);
                _logger.LogInformation("** PrebookAsync after propertyDetails");
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
                    _logger.LogInformation("** PrebookAsync before CreateFromSource");
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                        propertyDetails.Source,
                        prebookRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));
                    _logger.LogInformation("** PrebookAsync after CreateFromSource");
                    if (thirdParty != null)
                    {
                        prebookDateAndTime = DateTime.Now;
                        _logger.LogInformation("** PrebookAsync before PreBookAsync");
                        success = await thirdParty.PreBookAsync(propertyDetails);

                        _logger.LogInformation("** PrebookAsync after PreBookAsync");


                        if (success)
                        {
                            _logger.LogInformation("** PrebookAsync before CreateAsync");
                            response = await _responseFactory.CreateAsync(propertyDetails);
                            _logger.LogInformation("** PrebookAsync after CreateAsync");
                        }
                        else
                        {
                            _logger.LogInformation("** PrebookAsync before success false");
                            response = new Response()
                            {
                                Warnings = propertyDetails.Warnings.Select(w => $"{w.Title}, {w.Text}").ToList()
                            };
                            _logger.LogInformation("** PrebookAsync after success true");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Warnings.Add(ex.ToString());
                _logger.LogInformation($"** PrebookAsync Exception {ex.Message}");
            }
            finally
            {
                _logger.LogInformation("** PrebookAsync Finally Start");

                await _logRepository.LogPrebookAsync(prebookRequest, response!, success);
                await _supplierLogRepository.LogPrebookRequestsAsync(propertyDetails);

                _logger.LogInformation("** PrebookAsync Finally End");

                if (requestValid && !success)
                {
                    response.Warnings = new List<string>() { "Suppplier prebook failed" };
                }
                else if (requestValid)
                {
                    response.Warnings = null!;
                }
            }

            _logger.LogInformation("** PrebookAsync End");

            return response;
        }
    }
}