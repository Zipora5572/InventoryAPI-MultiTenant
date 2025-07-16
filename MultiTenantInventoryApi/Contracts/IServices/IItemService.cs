namespace MultiTenantInventoryApi.Contracts.IServices;

public interface IItemService
{
    Task<ItemResponse> CreateItemAsync(string tenantId, CreateItemRequest request, string? connectionId = null);
    Task<List<ItemResponse>> GetItemsAsync(string tenantId);
    Task<ItemResponse?> UpdateItemAsync(string tenantId, int itemId, UpdateItemRequest request, string? connectionId = null);
    Task<ItemResponse?> CheckoutItemAsync(string tenantId, int itemId, CheckoutItemRequest request, TenantSettings settings, string? connectionId = null);
    Task<ItemResponse?> CheckinItemAsync(string tenantId, int itemId, CheckinItemRequest request, string? connectionId = null);
    Task<bool> SoftDeleteItemAsync(string tenantId, int itemId, string? connectionId = null);
}
