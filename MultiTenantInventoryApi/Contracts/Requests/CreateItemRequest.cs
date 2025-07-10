namespace MultiTenantInventoryApi.Contracts.Requests;

public record CreateItemRequest(
    string Name,
    string Category
);
