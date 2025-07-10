
namespace MultiTenantInventoryApi.Data.Repository;

public class ItemRepository: Repository<Item>, IItemRepository
{
    readonly IDataContext _context;
    public ItemRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}
