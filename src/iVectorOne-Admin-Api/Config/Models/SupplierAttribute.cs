﻿using System;
using System.Text.Json.Serialization;

namespace iVectorOne_Admin_Api.Config.Models
{
    public partial class SupplierAttribute
    {
        public SupplierAttribute()
        {
            SupplierSubscriptionAttributes = new HashSet<SupplierSubscriptionAttribute>();
        }

        public int SupplierAttributeId { get; set; }
        public short SupplierId { get; set; }
        public int AttributeId { get; set; }

        public virtual Attribute Attribute { get; set; } = null!;
        
        [JsonIgnore]
        public virtual Supplier Supplier { get; set; } = null!;

        public virtual ICollection<SupplierSubscriptionAttribute> SupplierSubscriptionAttributes { get; set; }
    }
}
