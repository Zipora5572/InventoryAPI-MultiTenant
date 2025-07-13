namespace MultiTenantInventoryApi.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();
}
