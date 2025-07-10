namespace MultiTenantInventoryApi.Services;


public interface IItemService
{
    Task<ItemResponse> CreateItemAsync(string tenantId, CreateItemRequest request);
    Task<List<ItemResponse>> GetItemsAsync(string tenantId);
    Task<ItemResponse?> CheckoutItemAsync(string tenantId, int itemId, CheckoutItemRequest request, TenantSettings settings);
    Task<ItemResponse?> CheckinItemAsync(string tenantId, int itemId, CheckinItemRequest request);
    Task<bool> SoftDeleteItemAsync(string tenantId, int itemId);
}