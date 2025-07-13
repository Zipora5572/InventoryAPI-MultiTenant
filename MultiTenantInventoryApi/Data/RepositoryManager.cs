namespace MultiTenantInventoryApi.Data;

public class RepositoryManager(
    DataContext _context,
    IItemRepository itemRepository
) : IRepositoryManager
{
    public IItemRepository Items { get; } = itemRepository;

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
