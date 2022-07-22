namespace iVectorOne.Models
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
        /// Gets the total adults.
        /// </summary>
        public int TotalAdults => this.Count(o => o.PassengerType == PassengerType.Adult);

        /// <summary>
        /// Gets the total children.
        /// </summary>
        public int TotalChildren => this.Count(o => o.PassengerType == PassengerType.Child);

        /// <summary>
        /// Gets the total infants.
        /// </summary>
        public int TotalInfants => this.Count(o => o.PassengerType == PassengerType.Infant);

        /// <summary>
        /// Gets the child ages CSV.
        /// </summary>
        public string ChildAgesCSV => string.Join(",", ChildAges);

        /// <summary>
        /// Gets the child ages.
        /// </summary>
        public List<int> ChildAges
            => this.Where(o => o.PassengerType == PassengerType.Child).Select(o => o.Age).ToList();

        /// <summary>
        /// Totals the children set age or over.
        /// </summary>
        /// <param name="age">The maximum child age.</param>
        /// <returns>Totals the children set age or over</returns>
        public int TotalChildrenSetAgeOrOver(int age = 12)
            => this.Count(o => o.PassengerType == PassengerType.Child && o.Age >= age);

        /// <summary>
        /// Totals the children under set age.
        /// </summary>
        /// <param name="age">The maximum child age.</param>
        /// <returns>Totals the children set age or over</returns>
        public int TotalChildrenUnderSetAge(int age = 12)
            => this.Count(o => o.PassengerType == PassengerType.Child && o.Age < age);

        /// <summary>
        /// Childs the ages set age or over.
        /// </summary>
        /// <param name="age">The age.</param>
        /// <returns>The Childs the ages set age or over.</returns>
        public List<int> ChildAgesSetAgeOrOver(int age = 12)
            => ChildAges.Where(i => i >= age).ToList();

        /// <summary>
        /// Childs the ages under set age.
        /// </summary>
        /// <param name="age">The i age.</param>
        /// <returns>The Childs the ages under set age.</returns>
        public List<int> ChildAgesUnderSetAge(int age = 12)
            => ChildAges.Where(i => i < age).ToList();
    }
}
