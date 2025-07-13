namespace MultiTenantInventoryApi.Model.DTOs.Requests;

public record CreateItemRequest(
    string Name,
    string Category
);
