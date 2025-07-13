namespace MultiTenantInventoryApi.Services;

public class TenantProvider(IHttpContextAccessor _accessor) : ITenantProvider
{
    public string TenantId =>
        _accessor.HttpContext?.Items["TenantId"] as string
        ?? throw new InvalidOperationException("TenantId not found");

    public TenantSettings Settings =>
        _accessor.HttpContext?.Items["TenantSettings"] as TenantSettings
        ?? throw new InvalidOperationException("TenantSettings not found");
}
