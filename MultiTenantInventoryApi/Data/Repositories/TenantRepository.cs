namespace MultiTenantInventoryApi.Data.Repository;

public class TenantRepository(DataContext _context)
    : Repository<Tenant>(_context), ITenantRepository
{
}
