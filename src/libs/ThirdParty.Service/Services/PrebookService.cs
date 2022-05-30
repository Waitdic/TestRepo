﻿namespace ThirdParty.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ThirdParty;
    using ThirdParty.Factories;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Repositories;
    using ThirdParty.SDK.V2.PropertyPrebook;

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

        /// <summary>
        ///   <para>
        /// Takes in a pre book Request, talks to the third parties and returns a response</para>
        /// </summary>
        /// <param name="prebookRequest">The pre book request.</param>
        /// <param name="user">The user.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<Response> PrebookAsync(Request prebookRequest, User user)
        {
            Response response = null!;
            string exceptionString = string.Empty;
            bool success = true;
            var propertyDetails = new PropertyDetails();

            try
            {
                propertyDetails = await this.propertyDetailsFactory.CreateAsync(prebookRequest, user);

                if (propertyDetails.Warnings.Any())
                {
                    response = new Response()
                    {
                        Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
                    };
                }
                else
                {
                    IThirdParty thirdParty = this.thirdPartyFactory.CreateFromSource(
                        propertyDetails.Source,
                        user.Configurations.FirstOrDefault(c => c.Supplier == propertyDetails.Source));

                    if (thirdParty != null)
                    {
                        success = thirdParty.PreBook(propertyDetails);
                    }

                    if (success)
                    {
                        response = await this.responseFactory.CreateAsync(propertyDetails);
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

                this.logRepository.LogPrebook(prebookRequest, response!, user, exceptionString);
            }

            return response;
        }
    }
}