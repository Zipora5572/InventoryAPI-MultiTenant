
namespace MultiTenantInventoryApi.Services;

public class InventoryBroadcaster(IHubContext<InventoryHub> hubContext) : IInventoryBroadcaster
{
    public async Task BroadcastItemAdded(ItemResponse item, string? senderConnectionId = null)
    {
        await hubContext.Clients.GroupExcept(item.TenantId, senderConnectionId).SendAsync("itemAdded", item);
    }

    public async Task BroadcastItemUpdated(ItemResponse item, string? senderConnectionId = null)
    {
        await hubContext.Clients.GroupExcept(item.TenantId, senderConnectionId).SendAsync("itemUpdated", item);
    }

    public async Task BroadcastItemDeleted(int itemId, string tenantId, string? senderConnectionId = null)
    {
        await hubContext.Clients.GroupExcept(tenantId, senderConnectionId).SendAsync("itemDeleted", new { id = itemId, tenantId });
    }
}
