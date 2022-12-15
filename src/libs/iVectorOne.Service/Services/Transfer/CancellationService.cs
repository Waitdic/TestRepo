namespace iVectorOne.Services.Transfer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Models;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Repositories;
    using Cancel = SDK.V2.TransferCancel;
    using Precancel = SDK.V2.TransferPrecancel;

    /// <summary>Cancellation service responsible for cancelling third party bookings.</summary>
    public class CancellationService : ICancellationService
    {
        /// <summary>Factory used for building a transfer details from the request</summary>
        private readonly ITransferDetailsFactory _transferDetailsFactory;

        /// <summary>Repository responsible for logging</summary>
        private readonly ITransferAPILogRepository _logRepository;

        /// <summary>Factory responsible for creating the correct Third party class</summary>
        private readonly ITransferThirdPartyFactory _thirdPartyFactory;

        /// <summary>Builds the cancellation response</summary>
        private readonly ICancelTransferResponseFactory _responseFactory;

        /// <summary>The reference validator</summary>
        ////private readonly ISuppierReferenceValidator _referenceValidator;

        /// <summary>The supplier log repository</summary>
        private readonly ITransferSupplierLogRepository _supplierLogRepository;

        /// <summary>Initializes a new instance of the <see cref="CancellationService" /> class.</summary>
        /// <param name="transferDetailsFactory">Factory used for building a transfer details from the request</param>
        /// <param name="logRepository">Repository responsible for logging</param>
        /// <param name="thirdPartyFactory">Factory responsible for creating the correct Third party class.</param>
        /// <param name="responseFactory">Factory responsible for creating the response.</param>
        /// <param name="referenceValidator">Validates if the right supplier references have been sent for the supplier</param>
        /// <param name="supplierLogRepository">Repository for saving supplier logs to the database</param>
        public CancellationService(
            ITransferDetailsFactory transferDetailsFactory,
            ITransferAPILogRepository logRepository,
            ITransferThirdPartyFactory thirdPartyFactory,
            ICancelTransferResponseFactory responseFactory,
            ITransferSupplierLogRepository supplierLogRepository)
        {
            _transferDetailsFactory = Ensure.IsNotNull(transferDetailsFactory, nameof(transferDetailsFactory));
            _logRepository = Ensure.IsNotNull(logRepository, nameof(logRepository));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _responseFactory = Ensure.IsNotNull(responseFactory, nameof(responseFactory));
            _supplierLogRepository = Ensure.IsNotNull(supplierLogRepository, nameof(supplierLogRepository));
        }

        /// <inheritdoc/>
        public async Task<Cancel.Response> CancelAsync(Cancel.Request cancelRequest)
        {
            Cancel.Response response = null!;
            bool requestValid = true;
            bool success = false;
            var transferDetails = new TransferDetails();

            try
            {
                transferDetails = await _transferDetailsFactory.CreateAsync(cancelRequest);
                //_referenceValidator.ValidateCancel(transferDetails);

                if (transferDetails.Warnings.Any())
                {
                    response = new Cancel.Response()
                    {
                        Warnings = transferDetails.Warnings.Select(w => w.Text).ToList()
                    };

                    requestValid = false;
                }
                else
                {
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                        transferDetails.Source,
                        cancelRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == transferDetails.Source));

                    if (thirdParty != null)
                    {
                        ThirdPartyCancellationResponse thirdPartyReponse = new ThirdPartyCancellationResponse();
                       
                        requestValid = thirdParty.ValidateSettings(transferDetails);
                        
                        if (requestValid)
                        {
                            thirdPartyReponse = await thirdParty.CancelBookingAsync(transferDetails);
                            success = thirdPartyReponse?.Success ?? false;
                        }
                        if (success)
                        {
                            response = _responseFactory.Create(thirdPartyReponse!);
                        }
                        else
                        {
                            response = new Cancel.Response()
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
                await _logRepository.LogCancelAsync(cancelRequest, response!, success);
                await _supplierLogRepository.LogBookRequestsAsync(transferDetails);

                if (requestValid && !success)
                {
                    response.Warnings = new List<string>() { "Supplier cancellation failed" };
                }
                else if (requestValid)
                {
                    response.Warnings = null!;
                }
            }

            return response!;
        }

        /// <inheritdoc/>
        public async Task<Precancel.Response> GetCancellationFeesAsync(Precancel.Request preCancelRequest)
        {
            Precancel.Response response = null!;
            bool requestValid = true;
            bool success = false;
            var transferDetails = new TransferDetails();

            try
            {
                transferDetails = await _transferDetailsFactory.CreateAsync(preCancelRequest);
                //_referenceValidator.ValidateCancel(transferDetails);

                if (transferDetails.Warnings.Any())
                {
                    response = new Precancel.Response()
                    {
                        Warnings = transferDetails.Warnings.Select(w => w.Text).ToList()
                    };

                    requestValid = false;
                }
                else
                {
                    var thirdParty = _thirdPartyFactory.CreateFromSource(
                        transferDetails.Source,
                        preCancelRequest.Account.Configurations.FirstOrDefault(c => c.Supplier == transferDetails.Source));

                    if (thirdParty != null)
                    {
                        requestValid = thirdParty.ValidateSettings(transferDetails);

                        ThirdPartyCancellationFeeResult cancellationFees = new ThirdPartyCancellationFeeResult();
                        if (requestValid)
                        {
                            cancellationFees = await thirdParty.GetCancellationCostAsync(transferDetails);
                            success = !transferDetails.Warnings.Any();
                        }

                        if (success)
                        {
                            response = await _responseFactory.CreateFeesResponseAsync(cancellationFees, transferDetails);
                        }
                        else
                        {
                            response = new Precancel.Response()
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
                await _logRepository.LogPrecancelAsync(preCancelRequest, response!, success);
                await _supplierLogRepository.LogBookRequestsAsync(transferDetails);

                if (requestValid && !success)
                {
                    response.Warnings = new List<string>() { "Supplier pre-cancellation failed" };
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