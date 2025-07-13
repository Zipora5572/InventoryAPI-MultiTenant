namespace MultiTenantInventoryApi.Data.Repository;

public class ItemRepository(DataContext _context)
    : Repository<Item>(_context), IItemRepository
{
}
