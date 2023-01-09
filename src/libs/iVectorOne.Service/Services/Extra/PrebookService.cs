namespace iVectorOne.Services.Extra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Models.Extra;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.ExtraPrebook;

    /// <summary>
    ///   <para>The service responsible for handling the pre book</para>
    /// </summary>
    public class PrebookService : IPrebookService
    {
        /// <summary>The extra details factory</summary>
        private readonly IExtraDetailsFactory _extraDetailsFactory;

        /// <summary>The log repository</summary>
        private readonly IExtraAPILogRepository _logRepository;

        /// <summary>Factory that creates the third party class</summary>
        private readonly IExtraThirdPartyFactory _thirdPartyFactory;

        /// <summary>The factory responsible for building the pre book response</summary>
        private readonly IExtraPrebookResponseFactory _responseFactory;

        /// <summary>The supplier log repository</summary>
        private readonly IExtraSupplierLogRepository _supplierLogRepository;

        /// <summary>Initializes a new instance of the <see cref="PrebookService" /> class.</summary>
        /// <param name="extraDetailsFactory">The extra details factory.</param>
        /// <param name="logRepository">Repository for saving pre book logs to the database</param>
        /// <param name="thirdPartyFactory">Factory that creates the correct third party class</param>
        /// <param name="responseFactory">The factory responsible for building the pre book response</param>
        /// <param name="supplierLogRepository">Repository for saving supplier logs to the database</param>
        public PrebookService(
            IExtraDetailsFactory extraDetailsFactory,
            IExtraAPILogRepository logRepository,
            IExtraThirdPartyFactory thirdPartyFactory,
            IExtraPrebookResponseFactory responseFactory,
            IExtraSupplierLogRepository supplierLogRepository)
        {
            _extraDetailsFactory = Ensure.IsNotNull(extraDetailsFactory, nameof(extraDetailsFactory));
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
            var extraDetails = new ExtraDetails();
            var prebookDateAndTime = DateTime.Now;

            try
            {
                extraDetails = await _extraDetailsFactory.CreateAsync(prebookRequest);

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
                        prebookRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == extraDetails.Source));

                    if (thirdParty != null)
                    {
                        prebookDateAndTime = DateTime.Now;

                        requestValid = thirdParty.ValidateSettings(extraDetails);

                        if (requestValid)
                        {
                            success = await thirdParty.PreBookAsync(extraDetails);
                        }

                        if (success)
                        {
                            response = await _responseFactory.CreateAsync(extraDetails);
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
                await _logRepository.LogPrebookAsync(prebookRequest, response!, success);
                await _supplierLogRepository.LogPrebookRequestsAsync(extraDetails);

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