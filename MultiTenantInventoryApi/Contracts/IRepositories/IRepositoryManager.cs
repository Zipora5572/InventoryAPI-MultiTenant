namespace MultiTenantInventoryApi.Contracts.IRepositories;

public interface IRepositoryManager
{
    IItemRepository Items { get; }

    Task SaveAsync();
}
