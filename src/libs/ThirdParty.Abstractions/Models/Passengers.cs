namespace ThirdParty.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A list of passenger
    /// </summary>
    /// <seealso cref="List{Passenger}" />
    public class Passengers : List<Passenger>
    {
        /// <summary>
        /// Gets or sets a value indicating whether [automatic add names and ages].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic add names and ages]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoAddNamesAndAges { get; set; } = true;

        /// <summary>
        /// Gets the total adults.
        /// </summary>
        /// <value>
        /// The total adults.
        /// </value>
        public int TotalAdults
        {
            get
            {
                return this.Where(o => o.PassengerType == PassengerType.Adult).Count();
            }
        }

        /// <summary>
        /// Gets the total children.
        /// </summary>
        /// <value>
        /// The total children.
        /// </value>
        public int TotalChildren
        {
            get
            {
                return this.Where(o => o.PassengerType == PassengerType.Child).Count();
            }
        }

        /// <summary>
        /// Gets the total infants.
        /// </summary>
        /// <value>
        /// The total infants.
        /// </value>
        public int TotalInfants
        {
            get
            {
                return this.Where(o => o.PassengerType == PassengerType.Infant).Count();
            }
        }

        /// <summary>
        /// Gets the child ages CSV.
        /// </summary>
        /// <value>
        /// The child ages CSV.
        /// </value>
        public string ChildAgesCSV
        {
            get
            {
                return string.Join(",", this.Where(o => o.PassengerType == PassengerType.Child).Select(o => o.Age.ToString()).ToList());
            }
        }

        /// <summary>
        /// Gets the child ages.
        /// </summary>
        /// <value>
        /// The child ages.
        /// </value>
        public List<int> ChildAges
        {
            get
            {
                return this.Where(o => o.PassengerType == PassengerType.Child).Select(o => o.Age).ToList();
            }
        }

        /// <summary>
        /// Totals the children set age or over.
        /// </summary>
        /// <param name="age">The i age.</param>
        /// <returns>Totals the children set age or over</returns>
        public int TotalChildrenSetAgeOrOver(int age = 12)
        {
            return this.Where(o => o.PassengerType == PassengerType.Child && o.Age >= age).Count();
        }

        /// <summary>
        /// Totals the children under set age.
        /// </summary>
        /// <param name="age">The i age.</param>
        /// <returns>Totals the children set age or over</returns>
        public int TotalChildrenUnderSetAge(int age = 12)
        {
            return this.Where(o => o.PassengerType == PassengerType.Child && o.Age < age).Count();
        } 

        /// <summary>
        /// Childs the ages set age or over.
        /// </summary>
        /// <param name="age">The age.</param>
        /// <returns>The Childs the ages set age or over.</returns>
        public List<int> ChildAgesSetAgeOrOver(int age = 12)
        {
            return this.ChildAges.Where(i => i >= age).ToList();
        }

        /// <summary>
        /// Childs the ages under set age.
        /// </summary>
        /// <param name="age">The i age.</param>
        /// <returns>The Childs the ages under set age.</returns>
        public List<int> ChildAgesUnderSetAge(int age = 12)
        {
            return this.ChildAges.Where(i => i < age).ToList();
        }
    }
}
