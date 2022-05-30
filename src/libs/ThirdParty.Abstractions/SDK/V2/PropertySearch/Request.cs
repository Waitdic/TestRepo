namespace ThirdParty.SDK.V2.PropertySearch
{
    using System;
    using System.Collections.Generic;
    using MediatR;

    public record Request : RequestBase, IRequest<Response>
    {
        public DateTime? ArrivalDate { get; set; }

        public int Duration { get; set; }

        public List<int> Properties { get; set; } = new();

        public List<RoomRequest> RoomRequests { get; set; } = new();

        public string NaionalityID { get; set; } = "";

        public string CurrencyCode { get; set; } = "";

        public bool OpaqueRates { get; set; }

        public string SellingCountry { get; set; } = "";
    }
}