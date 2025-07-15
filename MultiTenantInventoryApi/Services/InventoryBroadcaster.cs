
namespace MultiTenantInventoryApi.Services;

public class InventoryBroadcaster(IHubContext<InventoryHub> hubContext)
{
    public async Task BroadcastItemAdded(ItemResponse item)
    {
        await hubContext.Clients.All.SendAsync("itemAdded", item);
    }

    public async Task BroadcastItemUpdated(ItemResponse item)
    {
        await hubContext.Clients.All.SendAsync("itemUpdated", item);
    }

    public async Task BroadcastItemDeleted(int itemId, string tenantId)
    {
        await hubContext.Clients.All.SendAsync("itemDeleted", new { id = itemId, tenantId });
    }
}
