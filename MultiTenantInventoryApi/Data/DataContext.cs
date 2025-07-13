namespace MultiTenantInventoryApi.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();

    public DbSet<Tenant> Tenants => Set<Tenant>();

}
