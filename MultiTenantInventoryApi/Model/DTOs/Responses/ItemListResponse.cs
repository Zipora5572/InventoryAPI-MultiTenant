namespace MultiTenantInventoryApi.Model.DTOs.Responses;

public record ItemListResponse(
    List<ItemResponse> Items
);
