namespace iVectorOne.Models.Property.Booking
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   <para>A class representing the room details</para>
    /// </summary>
    public class RoomDetails
    {
        /// <summary> Gets or sets The passengers</summary>
        public Passengers Passengers { get; set; } = new();

        /// <summary> Gets or sets The third party reference</summary>
        public string ThirdPartyReference { get; set; } = string.Empty;

        /// <summary>
        ///  Gets or sets The property room booking identifier
        /// </summary>
        public int PropertyRoomBookingID { get; set; }

        /// <summary>
        ///  Gets or sets The meal basis
        /// </summary>
        public string MealBasis { get; set; } = string.Empty;

        /// <summary>
        ///  Gets or sets The meal basis code
        /// </summary>
        public string MealBasisCode { get; set; } = string.Empty;

        /// <summary>
        ///  Gets or sets The room type
        /// </summary>
        public string RoomType { get; set; } = string.Empty;

        /// <summary>
        ///  Gets or sets The room type code
        /// </summary>
        public string RoomTypeCode { get; set; } = string.Empty;

        /// <summary>
        ///  Gets or sets The rate code identifier
        /// </summary>
        public int RateCodeID { get; set; }

        /// <summary>
        ///  Gets or sets The booking questions
        /// </summary>
        public List<BookingQuestion> BookingQuestions { get; set; } = new();

        /// <summary>
        ///  Gets or sets a value indicating whether The room is on request
        /// </summary>
        public bool OnRequest { get; set; } = false;

        /// <summary>
        ///  Gets or sets The optional supplements
        /// </summary>
        public List<OptionalSupplement> OptionalSupplements { get; set; } = new();

        /// <summary>
        /// Gets the adults.
        /// </summary>
        public int Adults => Passengers.TotalAdults;

        /// <summary>
        /// Gets the children.
        /// </summary>
        public int Children => Passengers.TotalChildren;

        /// <summary>
        /// Gets the infants.
        /// </summary>
        public int Infants => Passengers.TotalInfants;

        /// <summary>
        /// Gets the number of passengers.
        /// </summary>
        public int TotalPassengers => Passengers.TotalAdults + Passengers.TotalChildren + Passengers.TotalInfants;

        /// <summary>
        ///  Gets or sets The local cost
        /// </summary>
        public decimal LocalCost { get; set; }

        /// <summary>
        ///  Gets or sets The gross cost
        /// </summary>
        public decimal GrossCost { get; set; }

        /// <summary>
        ///  Gets or sets The total cost
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        ///  Gets or sets The currency
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Gets the child ages.
        /// </summary>
        public List<int> ChildAges => Passengers.ChildAges;

        /// <summary>
        /// Gets the child and infant ages.
        /// </summary>
        public List<int> ChildAndInfantAges
        {
            get
            {
                return Passengers.Where(o => o.PassengerType != PassengerType.Adult).Select(o => o.Age).ToList();
            }
        }

        /// <summary>
        /// The number of adults over the given ag.
        /// </summary>
        /// <param name="age">The i age.</param>
        /// <returns>The number of adults over the given age</returns>
        public int AdultsSetAgeOrOver(int age = 12)
        {
            return this.Adults + Passengers.TotalChildrenSetAgeOrOver(age);
        }

        /// <summary>
        /// Returns The number of Children over the given age.
        /// </summary>
        /// <param name="age">The i age.</param>
        /// <returns>The number of Children over the given age</returns>
        public int ChildrenSetAgeOrOver(int age = 12)
        {
            return Passengers.TotalChildrenSetAgeOrOver(age);
        }

        /// <summary>
        /// The number of Children under the given age.
        /// </summary>
        /// <param name="age">The i age.</param>
        /// <returns>The number of Children under the given age</returns>
        public int ChildrenUnderSetAge(int age = 12)
        {
            return Passengers.TotalChildrenUnderSetAge(age);
        }

        /// <summary>
        /// Gets the child age CSV.
        /// </summary>
        /// <returns>a comma separated list of the child ages</returns>
        public string GetChildAgeCsv()
        {
            return string.Join(",", Passengers.Where(o => o.PassengerType == PassengerType.Child).Select(o => o.Age.ToString()).ToList());
        }

        /// <summary>
        /// Gets and sets the special request.
        /// </summary>
        /// <value>
        /// The special request.
        /// </value>
        public string SpecialRequest { get; set; } = string.Empty;
    }
}