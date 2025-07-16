namespace MultiTenantInventoryApi.Contracts;
public interface IConnectionManager
{
    void MapConnection(string connectionId, string tenantId);
    string? GetTenantId(string connectionId);
    void UnmapConnection(string connectionId);
}
