
namespace MultiTenantInventoryApi.Data;

public class RepositoryManager : IRepositoryManager
{
    private readonly IDataContext _context;

    public IItemRepository Items { get; }

    public RepositoryManager(
    IDataContext context,
        IItemRepository itemRepository
       )
    {
        _context = context;
        Items = itemRepository;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
