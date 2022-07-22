namespace iVectorOne.CSSuppliers.ExpediaRapid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.CSSuppliers.ExpediaRapid.ExceptionMessages;


    public class ExpediaRapidOccupancy
    {

        /// <remarks>
        /// This constructor seems to exist only for testing purposes.
        /// There is no point in testing this if it isn't used.
        /// </remarks>

        public ExpediaRapidOccupancy(string occupancy)
        {

            if (string.IsNullOrWhiteSpace(occupancy))
                throw new ArgumentNullException(nameof(occupancy));

            var matches = Regex.Match(occupancy, @"^(?<adults>\d+)(-?)(?<childrenCsv>\d+(,\d+)*)?$").Groups;

            var adultsGroup = matches["adults"];
            var childCsvGroup = matches["childrenCsv"];

            if (!adultsGroup.Success)
                throw new ArgumentException(OccupancyExceptions.InvalidOccupancyString);

            int adults = adultsGroup.Value.ToSafeInt();

            if (adults == 0)
                throw new ArgumentException(OccupancyExceptions.NoAdults);

            Adults = adults;

            if (childCsvGroup.Success)
                ChildAges.AddRange(childCsvGroup.Value.Split(',').Select(int.Parse));
        }

        public ExpediaRapidOccupancy(int adults, List<int> childAges, int infants)
        {

            if (adults == 0)
                throw new ArgumentException(OccupancyExceptions.NoAdults);

            Adults = adults;
            if (childAges is not null)
                ChildAges.AddRange(childAges);
            ChildAges.AddRange(Enumerable.Range(0, infants).Select(i => 0));
        }

        public int Adults { get; set; }
        public List<int> ChildAges { get; set; } = new List<int>();

        public string GetExpediaRapidOccupancy()
        {
            var occupancyValue = new StringBuilder();
            occupancyValue.Append(Adults);

            if (ChildAges.Any())
                occupancyValue.Append("-" + string.Join(",", ChildAges));

            return occupancyValue.ToString();
        }

    }

}