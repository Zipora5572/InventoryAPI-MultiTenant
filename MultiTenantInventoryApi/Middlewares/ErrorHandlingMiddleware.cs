namespace MultiTenantInventoryApi.Middleware;

public class ErrorHandlingMiddleware(
    RequestDelegate _next,
    ILogger<ErrorHandlingMiddleware> _logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var tenantId = context.Request.Headers["X-Tenant-ID"].FirstOrDefault() ?? "Unknown";

            _logger.LogError(ex,
                "Unhandled exception for tenant {TenantId}. Path: {Path}, TraceId: {TraceId}",
                tenantId,
                context.Request.Path,
                context.TraceIdentifier);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                message = "An unexpected error occurred.",
                traceId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
