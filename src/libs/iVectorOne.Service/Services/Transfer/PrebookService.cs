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
    using iVectorOne.SDK.V2.TransferPrebook;

    /// <summary>
    ///   <para>The service responsible for handling the pre book</para>
    /// </summary>
    public class PrebookService : IPrebookService
    {
        /// <summary>The transfer details factory</summary>
        private readonly ITransferDetailsFactory _transferDetailsFactory;

        /// <summary>The log repository</summary>
        private readonly ITransferAPILogRepository _logRepository;

        /// <summary>Factory that creates the third party class</summary>
        private readonly ITransferThirdPartyFactory _thirdPartyFactory;

        /// <summary>The factory responsible for building the pre book response</summary>
        private readonly ITransferPrebookResponseFactory _responseFactory;

        /// <summary>The supplier log repository</summary>
        private readonly ITransferSupplierLogRepository _supplierLogRepository;

        /// <summary>Initializes a new instance of the <see cref="PrebookService" /> class.</summary>
        /// <param name="transferDetailsFactory">The transfer details factory.</param>
        /// <param name="logRepository">Repository for saving pre book logs to the database</param>
        /// <param name="thirdPartyFactory">Factory that creates the correct third party class</param>
        /// <param name="responseFactory">The factory responsible for building the pre book response</param>
        /// <param name="supplierLogRepository">Repository for saving supplier logs to the database</param>
        public PrebookService(
            ITransferDetailsFactory transferDetailsFactory,
            ITransferAPILogRepository logRepository,
            ITransferThirdPartyFactory thirdPartyFactory,
            ITransferPrebookResponseFactory responseFactory,
            ITransferSupplierLogRepository supplierLogRepository)
        {
            _transferDetailsFactory = Ensure.IsNotNull(transferDetailsFactory, nameof(transferDetailsFactory));
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
            var transferDetails = new TransferDetails();
            var prebookDateAndTime = DateTime.Now;

            try
            {
                transferDetails = await _transferDetailsFactory.CreateAsync(prebookRequest);

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
                        prebookRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == transferDetails.Source));

                    if (thirdParty != null)
                    {
                        prebookDateAndTime = DateTime.Now;

                        success = await thirdParty.PreBookAsync(transferDetails);

                        if (success)
                        {
                            response = await _responseFactory.CreateAsync(transferDetails);
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
                await _logRepository.LogPrebookAsync(prebookRequest, response!, success);
                await _supplierLogRepository.LogPrebookRequestsAsync(transferDetails);

                if (requestValid && !success)
                {
                    response.Warnings = new List<string>() { "Suppplier prebook failed" };
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