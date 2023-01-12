namespace iVectorOne.Search.Results.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A transformed results comparer, compares transformed results
    /// </summary>
    /// <seealso cref="IEqualityComparer{TransformedResult}" />
    public class TransformedResultsComparer : IEqualityComparer<TransformedResult>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <seealso cref="TransformedResult" /> to compare.</param>
        /// <param name="y">The second object of type <seealso cref="TransformedResult" /> to compare.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(TransformedResult x, TransformedResult y)
        {
            return x.MasterID == y.MasterID && x.TPKey == y.TPKey && x.CurrencyCode == y.CurrencyCode && x.RoomTypeCode == y.RoomTypeCode && x.RoomType == y.RoomType && x.PropertyRoomBookingID == y.PropertyRoomBookingID && x.MealBasisCode == y.MealBasisCode && x.Amount == y.Amount;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(TransformedResult obj)
        {
            HashCodeNoOverflow hashCode;

            hashCode.Int64 = 17;
            hashCode.Int32 = 0;

            hashCode.Int64 = (System.Convert.ToInt64(hashCode.Int32) * 23) + obj.MasterID.GetHashCode();

            if (!string.IsNullOrEmpty(obj.TPKey))
            {
                hashCode.Int64 = (System.Convert.ToInt64(hashCode.Int32) * 23) + obj.TPKey.GetHashCode();
            }

            if (!string.IsNullOrEmpty(obj.RoomTypeCode))
            {
                hashCode.Int64 = (System.Convert.ToInt64(hashCode.Int32) * 23) + obj.RoomTypeCode.GetHashCode();
            }

            return hashCode.Int32;
        }

        /// <summary>
        /// Hash code no overflow
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct HashCodeNoOverflow
        {
            /// <summary>
            /// The integer 64
            /// </summary>
            [FieldOffset(0)]
            public long Int64;

            /// <summary>
            /// The integer 32
            /// </summary>
            [FieldOffset(0)]
            public int Int32;
        }
    }
}
