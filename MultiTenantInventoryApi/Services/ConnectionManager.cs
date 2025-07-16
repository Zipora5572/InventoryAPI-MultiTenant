
namespace MultiTenantInventoryApi.Services;

public class ConnectionManager : IConnectionManager
{
    private readonly ConcurrentDictionary<string, string> _map = new();

    public void MapConnection(string connectionId, string tenantId)
    {
        _map[connectionId] = tenantId;
    }

    public string? GetTenantId(string connectionId)
    {
        return _map.TryGetValue(connectionId, out var tenantId) ? tenantId : null;
    }

    public void UnmapConnection(string connectionId)
    {
        _map.TryRemove(connectionId, out _);
    }
}
