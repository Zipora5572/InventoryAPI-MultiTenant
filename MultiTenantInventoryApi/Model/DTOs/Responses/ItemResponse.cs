namespace MultiTenantInventoryApi.Model.DTOs.Responses;

public record ItemResponse(
    int Id,
    string Name,
    string Category,
    bool IsCheckedOut,
    string? CheckedOutBy
);
