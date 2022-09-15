﻿namespace iVectorOne.SDK.V2.PropertySearch
{
    using System;
    using System.Collections.Generic;
    using iVectorOne.Models;
    using MediatR;

    public record Request : RequestBase, IRequest<Response>
    {
        public DateTime? ArrivalDate { get; set; }

        public int Duration { get; set; }

        public List<int> Properties { get; set; } = new();

        public List<RoomRequest> RoomRequests { get; set; } = new();

        public string NationalityID { get; set; } = "";

        public string CurrencyCode { get; set; } = "";

        public bool OpaqueRates { get; set; }

        public string SellingCountry { get; set; } = "";

        public List<string> Suppliers { get; set; } = new();

        public string EmailLogsToAddress { get; set; } = string.Empty;

        public DedupeMethod DedupeMethod { get; set; } = DedupeMethod.cheapestleadin;
    }
}