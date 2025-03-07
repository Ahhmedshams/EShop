using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace  EShop.Shared.Observability;

public static class Extensions
{
    private const string CorrelationIdKey = "X-Correlation-Id";

    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app) =>
        app.Use(async (ctx, next) =>
        {
            if (!ctx.Request.Headers.TryGetValue(CorrelationIdKey, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString("N");
            }
            CorrelationContext.CorrelationId = correlationId.ToString();
            ctx.Items[CorrelationIdKey] = correlationId.ToString();
            await next();
        });

    public static string? GetCorrelationId(this HttpContext context)
        => context.Items.TryGetValue(CorrelationIdKey, out var correlationId) ? correlationId as string : null;

    public static void SetCorrelationId(this HttpRequestHeaders context, string correlationId)
    => context.TryAddWithoutValidation(CorrelationIdKey, correlationId);
}

