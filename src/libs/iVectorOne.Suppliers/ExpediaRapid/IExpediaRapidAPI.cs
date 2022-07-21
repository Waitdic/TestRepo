﻿namespace iVectorOne.CSSuppliers.ExpediaRapid
{
    using System.Threading.Tasks;
    using Intuitive.Helpers.Net;
    using iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses;
    using iVectorOne.Models.Property.Booking;

    public interface IExpediaRapidAPI
    {
        Task<TResponse> GetDeserializedResponseAsync<TResponse>(PropertyDetails propertyDetails, Request request) where TResponse : IExpediaRapidResponse<TResponse>, new();
        Task<string> GetResponseAsync(PropertyDetails propertyDetails, Request request);
    }
}