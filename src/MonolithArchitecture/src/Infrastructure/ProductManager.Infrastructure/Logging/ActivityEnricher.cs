/*
 * https://github.com/serilog/serilog-aspnetcore/issues/207
 *
 */
using System.Diagnostics;
namespace ProductManager.Infrastructure.Logging;

internal static class ActivityExtensions
{
    public static string GetSpanId(this Activity activity) => activity.IdFormat switch
    {
        ActivityIdFormat.Hierarchical => activity.Id,
        ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
        _ => null
    } ?? string.Empty;

    public static string GetTraceId(this Activity activity) => activity.IdFormat switch
    {
        ActivityIdFormat.Hierarchical => activity.RootId,
        ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
        _ => null
    } ?? string.Empty;

    public static string GetParentId(this Activity activity) => activity.IdFormat switch
    {
        ActivityIdFormat.Hierarchical => activity.ParentId,
        ActivityIdFormat.W3C => activity.ParentSpanId.ToHexString(),
        _ => null
    } ?? string.Empty;
}
