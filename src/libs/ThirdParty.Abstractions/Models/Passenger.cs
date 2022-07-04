namespace ThirdParty.Models
{
    using System;

    /// <summary>
    /// a passenger
    /// </summary>
    public class Passenger
    {
        /// <summary>
        /// Gets or sets the passenger identifier.
        /// </summary>
        /// <value>
        /// The passenger identifier.
        /// </value>
        public int PassengerID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the type of the passenger.
        /// </summary>
        /// <value>
        /// The type of the passenger.
        /// </value>
        public PassengerType PassengerType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is lead guest.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is lead guest; otherwise, <c>false</c>.
        /// </value>
        public bool IsLeadGuest { get; set; } = false;

        // todo - populate from prebook / book requests
        /// <summary>
        /// Gets or sets the nationality identifier.
        /// </summary>
        /// <value>
        /// The nationality identifier.
        /// </value>
        public string NationalityCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public Address Address { get; set; } = new Address();

        /// <summary>
        /// Gets or sets the third party identifier.
        /// </summary>
        /// <value>
        /// The third party identifier.
        /// </value>
        public string TPID { get; set; } = null!;

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; } = null!;

        /// <summary>
        /// Gets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public Gender Gender
        {
            get
            {
                switch (this.Title)
                {
                    case "Mrs":
                        {
                            return Gender.Female;
                        }

                    case "Miss":
                        {
                            return Gender.Female;
                        }

                    case "Ms":
                        {
                            return Gender.Female;
                        }

                    default:
                        {
                            return Gender.Male;
                        }
                }
            }
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName
        {
            get
            {
                return this.Title + " " + this.FirstName + " " + this.LastName;
            }
        }
    }
}