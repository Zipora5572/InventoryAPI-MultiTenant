namespace MultiTenantInventoryApi.Data.Repository;

public class Repository<T>(DataContext _context) : IRepository<T> where T : class
{
    protected readonly DbSet<T> _dbSet = _context.Set<T>();

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T> UpdateAsync(int id, T updatedEntity)
    {
        var existingEntity = await _dbSet.FindAsync(id);
        if (existingEntity == null)
            return null;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name != "Id");

        foreach (var prop in properties)
        {
            var value = prop.GetValue(updatedEntity);
            if (value != null)
                prop.SetValue(existingEntity, value);
        }

        return existingEntity;
    }
}
