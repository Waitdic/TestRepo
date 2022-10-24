using System.Diagnostics;

namespace iVectorOne_Admin_Api.Infrastructure.Extensions
{
    public static class ActivityExtensions
    {
        public static string GetTraceId(this Activity activity)
        {
            ArgumentNullException.ThrowIfNull(activity);


            var traceId = activity.IdFormat switch
            {
                ActivityIdFormat.Hierarchical => activity.RootId,
                ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
                ActivityIdFormat.Unknown => null,
                _ => null,
            };

            return traceId ?? string.Empty;
        }
    }
}