namespace MultiTenantInventoryApi.Contracts.Responses;

public record ItemListResponse(
    List<ItemResponse> Items
);
