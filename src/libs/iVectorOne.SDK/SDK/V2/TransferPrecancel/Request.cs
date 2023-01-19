﻿namespace iVectorOne.SDK.V2.TransferPrecancel
{
    using MediatR;

    public record Request : ComponentRequestBase, IRequest<Response>
    {
        /// <summary>
        /// Gets or sets the supplier booking reference.
        /// </summary>
        public string SupplierBookingReference { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference.</summary>
        public string SupplierReference { get; set; } = string.Empty;
    }
}