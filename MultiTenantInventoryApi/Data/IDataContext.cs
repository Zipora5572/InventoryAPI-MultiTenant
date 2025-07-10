namespace MultiTenantInventoryApi.Data;

public interface IDataContext
{
    DbSet<Item> Items { get; }

    DbSet<T> Set<T>() where T : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
