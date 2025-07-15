namespace MultiTenantInventoryApi.Model.DTOs.Requests;

public record UpdateItemRequest(
    string? Name = null,
    string? Category = null
);
