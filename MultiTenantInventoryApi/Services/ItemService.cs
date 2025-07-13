namespace MultiTenantInventoryApi.Services;

public class ItemService(
    IRepositoryManager _repo,
    IMapper _mapper,
    ILogger<ItemService> _logger
) : IItemService
{
    public async Task<ItemResponse> CreateItemAsync(string tenantId, CreateItemRequest request)
    {
        try
        {
            var item = _mapper.Map<Item>(request) with { TenantId = tenantId };

            await _repo.Items.AddAsync(item);
            await _repo.SaveAsync();

            return _mapper.Map<ItemResponse>(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateItemAsync failed for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<List<ItemResponse>> GetItemsAsync(string tenantId)
    {
        try
        {
            var items = await _repo.Items
                .GetAll()
                .Where(i => i.TenantId == tenantId && i.IsActive)
                .ToListAsync();

            return _mapper.Map<List<ItemResponse>>(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetItemsAsync failed for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<ItemResponse?> CheckoutItemAsync(string tenantId, int itemId, CheckoutItemRequest request, TenantSettings settings)
    {
        try
        {
            if (!settings.EnableCheckout)
            {
                _logger.LogWarning("Checkout validation failed: feature disabled for tenant {TenantId}", tenantId);
                return null;
            }

            var item = await _repo.Items.GetByIdAsync(itemId);
            if (item == null)
            {
                _logger.LogWarning("Checkout validation failed: item {ItemId} not found (tenant {TenantId})", itemId, tenantId);
                return null;
            }

            if (item.TenantId != tenantId)
            {
                _logger.LogWarning("Checkout validation failed: tenant mismatch for item {ItemId}. Expected {TenantId}, found {ActualTenantId}", itemId, tenantId, item.TenantId);
                return null;
            }

            if (!item.IsActive)
            {
                _logger.LogWarning("Checkout validation failed: item {ItemId} is not active (tenant {TenantId})", itemId, tenantId);
                return null;
            }

            if (item.IsCheckedOut)
            {
                _logger.LogWarning("Checkout validation failed: item {ItemId} already checked out (tenant {TenantId})", itemId, tenantId);
                return null;
            }

            if (!settings.AllowedItemCategories.Contains(item.Category))
            {
                _logger.LogWarning("Checkout validation failed: category '{Category}' not allowed for tenant {TenantId}", item.Category, tenantId);
                return null;
            }

            var userItemsCount = await _repo.Items
                .GetAll()
                .CountAsync(i => i.TenantId == tenantId && i.CheckedOutBy == request.Username && i.IsCheckedOut);

            if (userItemsCount >= settings.MaxItemsPerUser)
            {
                _logger.LogWarning("Checkout validation failed: user '{Username}' exceeded max items ({Max}) (tenant {TenantId})",
                    request.Username, settings.MaxItemsPerUser, tenantId);
                return null;
            }

            var updatedItem = item with { IsCheckedOut = true, CheckedOutBy = request.Username };

            await _repo.Items.UpdateAsync(itemId, updatedItem);
            await _repo.SaveAsync();

            _logger.LogInformation("Checkout success: item {ItemId} checked out by {Username} (tenant {TenantId})", itemId, request.Username, tenantId);
            return _mapper.Map<ItemResponse>(updatedItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CheckoutItemAsync failed for item {ItemId}, tenant {TenantId}", itemId, tenantId);
            throw;
        }
    }

    public async Task<ItemResponse?> CheckinItemAsync(string tenantId, int itemId, CheckinItemRequest request)
    {
        try
        {
            var item = await _repo.Items.GetByIdAsync(itemId);
            if (item == null || item.TenantId != tenantId || !item.IsActive)
            {
                _logger.LogWarning("Checkin validation failed: item {ItemId} not found or inactive (tenant {TenantId})", itemId, tenantId);
                return null;
            }

            if (!item.IsCheckedOut || item.CheckedOutBy != request.Username)
            {
                _logger.LogWarning("Checkin validation failed: item {ItemId} not checked out by user {Username} (tenant {TenantId})", itemId, request.Username, tenantId);
                return null;
            }

            var updatedItem = item with { IsCheckedOut = false, CheckedOutBy = null };

            await _repo.Items.UpdateAsync(itemId, updatedItem);
            await _repo.SaveAsync();

            _logger.LogInformation("Checkin success: item {ItemId} returned by {Username} (tenant {TenantId})", itemId, request.Username, tenantId);
            return _mapper.Map<ItemResponse>(updatedItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CheckinItemAsync failed for item {ItemId}, tenant {TenantId}", itemId, tenantId);
            throw;
        }
    }

    public async Task<bool> SoftDeleteItemAsync(string tenantId, int itemId)
    {
        try
        {
            var item = await _repo.Items.GetByIdAsync(itemId);

            if (item == null)
            {
                _logger.LogWarning("SoftDelete validation failed: item {ItemId} not found (tenant {TenantId})", itemId, tenantId);
                return false;
            }

            if (item.TenantId != tenantId)
            {
                _logger.LogWarning("SoftDelete validation failed: tenant mismatch for item {ItemId}. Expected {TenantId}, found {ActualTenantId}",
                    itemId, tenantId, item.TenantId);
                return false;
            }

            var updatedItem = item with { IsActive = false };
            await _repo.Items.UpdateAsync(itemId, updatedItem);
            await _repo.SaveAsync();

            _logger.LogInformation("SoftDelete success: item {ItemId} marked inactive (tenant {TenantId})", itemId, tenantId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SoftDeleteItemAsync failed for item {ItemId}, tenant {TenantId}", itemId, tenantId);
            return false;
        }
    }
}
