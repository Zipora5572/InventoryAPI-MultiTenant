namespace MultiTenantInventoryApi.Services;

public interface IConnectionProvider
{
    string? ConnectionId { get; }
}
