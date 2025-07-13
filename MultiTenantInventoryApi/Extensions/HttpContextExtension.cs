namespace MultiTenantInventoryApi.Extensions;

public static class HttpContextExtension
{
    public static string GetTenantId(this HttpContext context)
    {
        return context.Items["TenantId"] as string
            ?? throw new InvalidOperationException("TenantId not resolved");
    }

    public static TenantSettings GetTenantSettings(this HttpContext context)
    {
        return context.Items["TenantSettings"] as TenantSettings
            ?? throw new InvalidOperationException("TenantSettings not resolved");
    }
}
