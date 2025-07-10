namespace MultiTenantInventoryApi.Contracts.IRepositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();

    Task<T?> GetByIdAsync(int id);

    Task<T> AddAsync(T entity);

    Task<T> UpdateAsync(int id, T entity);

    Task DeleteAsync(T entity);
}
