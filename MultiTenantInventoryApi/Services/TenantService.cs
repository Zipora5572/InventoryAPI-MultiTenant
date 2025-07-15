namespace MultiTenantInventoryApi.Services;

public class TenantService(
    IRepositoryManager _repo,
    IMapper _mapper,
    ILogger<TenantService> _logger
) : ITenantService
{
    public async Task<Tenant> CreateTenantAsync(Tenant request)
    {
        try
        {
            var tenant = new Tenant(0, request.Name);


            await _repo.Tenants.AddAsync(tenant);
            await _repo.SaveAsync();

            _logger.LogInformation("Tenant created successfully: {TenantName}", tenant.Name);
            return _mapper.Map<Tenant>(tenant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateTenantAsync failed for name {TenantName}", request.Name);
            throw;
        }
    }

    public async Task<List<Tenant>> GetAllTenantsAsync()
    {
        try
        {
            var tenants = await _repo.Tenants.GetAll().ToListAsync();
            return _mapper.Map<List<Tenant>>(tenants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllTenantsAsync failed");
            throw;
        }
    }

    public async Task<bool> DeleteTenantAsync(int tenantId)
    {
        try
        {
            var tenant = await _repo.Tenants.GetByIdAsync(tenantId);
            if (tenant == null)
            {
                _logger.LogWarning("Delete failed: tenant {TenantId} not found", tenantId);
                return false;
            }

            await _repo.Tenants.DeleteAsync(tenant);
            await _repo.SaveAsync();

            _logger.LogInformation("Tenant {TenantId} deleted successfully", tenantId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteTenantAsync failed for tenant {TenantId}", tenantId);
            return false;
        }
    }

}
