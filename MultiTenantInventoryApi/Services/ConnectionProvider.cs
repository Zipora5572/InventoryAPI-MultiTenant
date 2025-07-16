namespace MultiTenantInventoryApi.Services;

public class ConnectionProvider(IHttpContextAccessor accessor) : IConnectionProvider
{
    public string? ConnectionId =>
        accessor.HttpContext?.Request.Headers["X-Connection-Id"].FirstOrDefault();
}
