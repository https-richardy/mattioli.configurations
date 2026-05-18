using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Mattioli.Configurations.Middlewares
{
    public class RequestCorrelationIdEnrichmentMiddleware(RequestDelegate next)
    {
        private const string CorrelationIdHeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next = next;

        public Task Invoke(HttpContext context)
        {
            string correlationId = GetCorrelationId(context);

            context.Request.Headers[CorrelationIdHeaderName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                return _next.Invoke(context);
            }
        }

        private static string GetCorrelationId(HttpContext context)
        {
            context.Request.Headers.TryGetValue(
                CorrelationIdHeaderName, out StringValues correlationId);

            return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
        }
    }
}
