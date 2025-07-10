namespace MultiTenantInventoryApi.Contracts.Responses;

public record ItemResponse(
    int Id,
    string Name,
    string Category,
    bool IsCheckedOut,
    string? CheckedOutBy
);
