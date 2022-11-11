namespace iVectorOne.SDK.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.SDK.V2;
    using iVectorOne.SDK.V2.PropertyBook;
    using iVectorOne.Tests.Models;

    public class LeadCustomerTest
    {
        [Fact]
        public void Validate_Should_AddCorrectWarning_When_EmailFormatIsInvalid()
        {
            //Arange + Act
            var validationResults = GetEmailValidationResults(LeadCustomerRes.InvalidEmailFormats);

            // Assert 
            Assert.False(validationResults.Where(v => v).Any());
        }

        [Fact]
        public void Validate_ShouldNot_AddWarning_When__EmailFormatIsValid()
        {
            //Arange + Act
            var validationResults = GetEmailValidationResults(LeadCustomerRes.ValidEmailFormats);

            // Assert 
            Assert.False(validationResults.Where(v => !v).Any());
        }

        #region "Helpers"

        public List<bool> GetEmailValidationResults(string emailsFile)
        {
            //Arange
            string[] emails = emailsFile.Split(Environment.NewLine, StringSplitOptions.None);
            var validationResults = new List<bool>();
            var validator = new Validator();

            // Act
            foreach (string email in emails)
            {
                if (!email.Contains("//"))
                {
                    var request = new Request()
                    {
                        LeadCustomer = CreateLeadCustomer(email.Trim()),
                    };
                    var results = validator.Validate(request).Errors.Select(e => e.ErrorMessage);

                    validationResults.Add(!results.Any(r => r == WarningMessages.InvalidCustomerEmail));
                }
            }

            return validationResults;
        }

        public LeadCustomer CreateLeadCustomer(
            string email = "example@gmail.com",
            string title = "Mr",
            string firstName = "Test",
            string lastName = "Tester",
            string dob = "1969-12-22T00:00:00Z",
            string addres1 = "1 Test Street",
            string city = "Testville",
            string country = "Testshire",
            string postcode = "IV30 4JS",
            string bookingCountryCode = "GB",
            string phone = "7879440485",
            string mobile = "7879440485")
        {
            return new LeadCustomer
            {
                CustomerTitle = title,
                CustomerFirstName = firstName,
                CustomerLastName = lastName,
                DateOfBirth = dob.ToSafeDate(),
                CustomerAddress1 = addres1,
                CustomerTownCity = city,
                CustomerCounty = country,
                CustomerPostcode = postcode,
                CustomerBookingCountryCode = bookingCountryCode,
                CustomerPhone = phone,
                CustomerMobile = mobile,
                CustomerEmail = email
            };
        }

        #endregion
    }
}