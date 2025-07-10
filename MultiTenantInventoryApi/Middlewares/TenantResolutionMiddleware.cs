namespace MultiTenantInventoryApi.Middlewares;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;
    private readonly IOptions<Dictionary<string, TenantSettings>> _tenantOptions;

    public TenantResolutionMiddleware(
        RequestDelegate next,
        ILogger<TenantResolutionMiddleware> logger,
        IOptions<Dictionary<string, TenantSettings>> tenantOptions)
    {
        _next = next;
        _logger = logger;
        _tenantOptions = tenantOptions;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantIdValues))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Missing X-Tenant-ID header.");
            return;
        }

        var tenantId = tenantIdValues.ToString();

        if (string.IsNullOrWhiteSpace(tenantId) || !_tenantOptions.Value.ContainsKey(tenantId))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Invalid or unknown tenant.");
            return;
        }

        httpContext.Items["TenantId"] = tenantId;
        httpContext.Items["TenantSettings"] = _tenantOptions.Value[tenantId];

        _logger.LogInformation("Tenant '{TenantId}' accessed {Path}", tenantId, httpContext.Request.Path);

        await _next(httpContext);
    }
}

public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolutionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantResolutionMiddleware>();
    }
}
