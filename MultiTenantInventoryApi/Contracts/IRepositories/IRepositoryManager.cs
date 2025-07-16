namespace MultiTenantInventoryApi.Contracts.IRepositories;

public interface IRepositoryManager
{
    IItemRepository Items { get; }

    ITenantRepository Tenants { get; }

    Task SaveAsync();
}
