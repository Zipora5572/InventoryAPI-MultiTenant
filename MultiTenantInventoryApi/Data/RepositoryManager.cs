using MultiTenantInventoryApi.Contracts;

namespace MultiTenantInventoryApi.Data;

public class RepositoryManager(
    IDataContext _context,
    IItemRepository itemRepository
) : IRepositoryManager
{
    public IItemRepository Items { get; } = itemRepository;

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
