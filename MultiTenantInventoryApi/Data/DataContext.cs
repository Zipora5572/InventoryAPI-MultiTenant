

namespace MultiTenantInventoryApi.Data;

public class DataContext:DbContext, IDataContext
{
    public DbSet<Item> Items => Set<Item>();

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
}
