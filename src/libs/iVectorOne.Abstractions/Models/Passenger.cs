namespace iVectorOne.Models
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
        public int PassengerID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the type of the passenger.
        /// </summary>
        public PassengerType PassengerType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is lead guest.
        /// </summary>
        public bool IsLeadGuest { get; set; } = false;

        // todo - populate from prebook / book requests
        /// <summary>
        /// Gets or sets the nationality identifier.
        /// </summary>
        public string NationalityCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public Address Address { get; set; } = new();

        /// <summary>
        /// Gets or sets the third party identifier.
        /// </summary>
        public string TPID { get; set; } = null!;

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// Gets the gender.
        /// </summary>
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
        public string FullName
        {
            get
            {
                return this.Title + " " + this.FirstName + " " + this.LastName;
            }
        }
    }
}