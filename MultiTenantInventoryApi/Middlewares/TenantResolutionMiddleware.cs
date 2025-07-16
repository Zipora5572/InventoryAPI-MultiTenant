namespace MultiTenantInventoryApi.Middlewares;

public class TenantResolutionMiddleware(
    RequestDelegate _next,
    ILogger<TenantResolutionMiddleware> _logger,
    IOptions<Dictionary<string, TenantSettings>> _tenantOptions)
{
    private const string TenantIdKey = "TenantId";
    private const string TenantSettingsKey = "TenantSettings";

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api/items", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context); 
            return;
        }
        if (!context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantIdValues))
        {
            _logger.LogWarning("Request missing X-Tenant-ID header. Path: {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing X-Tenant-ID header.");
            return;
        }

        var tenantId = tenantIdValues.ToString();

        if (string.IsNullOrWhiteSpace(tenantId) || !_tenantOptions.Value.TryGetValue(tenantId, out var settings))
        {
            _logger.LogWarning("Invalid or unknown tenant '{TenantId}'. Path: {Path}", tenantId, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid or unknown tenant.");
            return;
        }

        context.Items[TenantIdKey] = tenantId;
        context.Items[TenantSettingsKey] = settings;

        _logger.LogInformation("Tenant '{TenantId}' resolved for path {Path}", tenantId, context.Request.Path);
        await _next(context);
    }
}

public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolutionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantResolutionMiddleware>();
    }
}
