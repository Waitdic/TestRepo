namespace iVectorOne.SDK.V2.PropertyContent
{
    using System.Collections.Generic;

    public class RoomType
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? RateName { get; set; }
        public string? RateId { get; set; }
        public List<string> Facilities { get; set; } = new List<string>();

        public RoomType()
        {

        }

        public RoomType(string? name, string? code, string? rateName, string? rateId, List<string> facilities)
        {
            this.Name = name;
            this.Code = code;
            this.RateName = rateName;
            this.RateId = rateId;
            this.Facilities = facilities;
        }
    }
}