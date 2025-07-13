namespace MultiTenantInventoryApi.Contracts.IServices;


public interface ITenantService
{
    Task<Tenant> CreateTenantAsync(Tenant request);
    Task<List<Tenant>> GetAllTenantsAsync();

    Task<bool> DeleteTenantAsync(int tenantId);

}
