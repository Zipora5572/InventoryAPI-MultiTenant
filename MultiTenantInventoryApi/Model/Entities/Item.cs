namespace MultiTenantInventoryApi.Model.Entities;
public record Item
{
    public int Id { get; init; }
    public string TenantId { get; init; }
    public string Name { get; init; }
    public string Category { get; init; }
    public bool IsCheckedOut { get; init; }
    public string? CheckedOutBy { get; init; }
    public bool IsActive { get; init; }

    public Item() { }
}
