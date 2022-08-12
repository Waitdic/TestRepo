﻿using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Create
{
    public record Response : ResponseBase
    {
        public List<string> Warnings { get; set; } = new();

        public void Default(ResponseModelBase model)
        {
            Result = Results.Ok(model);
        }

        public void NotFound()
        {
            Result = Results.NotFound();
        }

        public void BadRequest()
        {
            Result = Results.BadRequest();
        }
    }
}