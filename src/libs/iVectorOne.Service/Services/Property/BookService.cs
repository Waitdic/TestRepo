﻿namespace iVectorOne.Services
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
        private readonly IAPILogRepository _logRepository;

        /// <summary>Factory that creates the third party class</summary>
        private readonly IThirdPartyFactory _thirdPartyFactory;

        /// <summary>Creates a book response using information from the property details</summary>
        private readonly IPropertyBookResponseFactory _responseFactory;

        /// <summary>The reference validator</summary>
        private readonly ISuppierReferenceValidator _referenceValidator;

        /// <summary>The supplier log repository</summary>
        private readonly ISupplierLogRepository _supplierLogRepository;

        /// <summary>The supplier log repository</summary>
        private readonly IBookingRepository _bookingRepository;

        /// <summary>Initializes a new instance of the <see cref="BookService" /> class.</summary>
        /// <param name="propertyDetailsFactory">The property details factory.</param>
        /// <param name="logRepository">Repository for saving pre book logs to the database</param>
        /// <param name="thirdPartyFactory">Factory that creates the correct third party class</param>
        /// <param name="responseFactory">Creates a book response using information from the property details</param>
        /// <param name="referenceValidator">Validates if the right supplier references have been sent for the supplier</param>
        /// <param name="supplierLogRepository">Repository for saving supplier logs to the database</param>
        /// <param name="bookingRepository">Repository for saving the booking to the database</param>
        public BookService(
            IPropertyDetailsFactory propertyDetailsFactory,
            IAPILogRepository logRepository,
            IThirdPartyFactory thirdPartyFactory,
            IPropertyBookResponseFactory responseFactory,
            ISuppierReferenceValidator referenceValidator,
            ISupplierLogRepository supplierLogRepository,
            IBookingRepository bookingRepository)
        {
            _propertyDetailsFactory = Ensure.IsNotNull(propertyDetailsFactory, nameof(propertyDetailsFactory));
            _logRepository = Ensure.IsNotNull(logRepository, nameof(logRepository));
            _thirdPartyFactory = Ensure.IsNotNull(thirdPartyFactory, nameof(thirdPartyFactory));
            _responseFactory = Ensure.IsNotNull(responseFactory, nameof(responseFactory));
            _referenceValidator = Ensure.IsNotNull(referenceValidator, nameof(referenceValidator));
            _supplierLogRepository = Ensure.IsNotNull(supplierLogRepository, nameof(supplierLogRepository));
            _bookingRepository = Ensure.IsNotNull(bookingRepository, nameof(bookingRepository));
        }

        /// <inheritdoc/>
        public async Task<Response> BookAsync(Request bookRequest)
        {
            Response response = null!;
            bool requestValid = true;
            bool success = false;
            var propertyDetails = new PropertyDetails();

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
                int bookingId = await _bookingRepository.StoreBookingAsync(propertyDetails, requestValid, success);
                propertyDetails.BookingID = bookingId;
                bookRequest.BookingID = bookingId;

                await _logRepository.LogBookAsync(bookRequest, response!, success);
                await _supplierLogRepository.LogBookRequestsAsync(propertyDetails);

                if (requestValid && !success)
                {
                    response.Warnings = new List<string>() { "Supplier book failed" };
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