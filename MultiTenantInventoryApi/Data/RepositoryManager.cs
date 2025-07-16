namespace MultiTenantInventoryApi.Data;

public class RepositoryManager(
    DataContext _context,
    IItemRepository itemRepository,
    ITenantRepository tenantRepository
) : IRepositoryManager
{
    public IItemRepository Items { get; } = itemRepository;

    public ITenantRepository Tenants { get; } = tenantRepository;

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
