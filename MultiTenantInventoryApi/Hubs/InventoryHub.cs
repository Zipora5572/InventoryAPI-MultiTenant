namespace MultiTenantInventoryApi.Hubs;

public class InventoryHub(IConnectionManager connectionManager) : Hub
{
    public async Task<string> RegisterToGroup(string tenantId)
    {
        var connectionId = Context.ConnectionId;
        var previousTenant = connectionManager.GetTenantId(connectionId);

        if (!string.IsNullOrEmpty(previousTenant) && previousTenant != tenantId)
            await Groups.RemoveFromGroupAsync(connectionId, previousTenant);

        connectionManager.MapConnection(connectionId, tenantId);
        await Groups.AddToGroupAsync(connectionId, tenantId);
        return connectionId;
    }



    public override Task OnDisconnectedAsync(Exception? exception)
    {
        connectionManager.UnmapConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}

