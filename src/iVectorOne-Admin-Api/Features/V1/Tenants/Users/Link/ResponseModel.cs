﻿namespace iVectorOne_Admin_Api.Features.V1.Tenants.Users.Link
{
    public record ResponseModel : ResponseModelBase
    {
        public int TenantId { get; set; }
    }
}