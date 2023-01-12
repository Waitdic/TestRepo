﻿namespace iVectorOne.SDK.V2.TransferCancel
{
    using MediatR;

    public record Request : TransferRequestBase, IRequest<Response>
    {
        /// <summary>Gets or sets the supplier booking reference.</summary>
        public string SupplierBookingReference { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference1.</summary>
        public string SupplierReference { get; set; } = string.Empty;

    }
}