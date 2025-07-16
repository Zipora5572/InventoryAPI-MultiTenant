namespace MultiTenantInventoryApi.Contracts;

public interface IInventoryBroadcaster
{
    Task BroadcastItemAdded(ItemResponse item, string ConnectionId = null);
    Task BroadcastItemUpdated(ItemResponse item, string ConnectionId = null);
    Task BroadcastItemDeleted(int itemId, string tenantId, string ConnectionId = null);
}

