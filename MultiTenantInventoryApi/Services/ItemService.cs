namespace MultiTenantInventoryApi.Services;


public class ItemService : IItemService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<ItemService> _logger; 

    public ItemService(IRepositoryManager repo, IMapper mapper, ILogger<ItemService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;

    }

    public async Task<ItemResponse> CreateItemAsync(string tenantId, CreateItemRequest request)
    {
        var item = _mapper.Map<Item>(request) with
        {
            TenantId = tenantId
        };

        await _repo.Items.AddAsync(item);
        await _repo.SaveAsync();

        return _mapper.Map<ItemResponse>(item);
    }

    public async Task<List<ItemResponse>> GetItemsAsync(string tenantId)
    {
        var items = _repo.Items
            .GetAll()
            .Where(i => i.TenantId == tenantId && i.IsActive)
            .ToList();

        return _mapper.Map<List<ItemResponse>>(items);
    }

    public async Task<ItemResponse?> CheckoutItemAsync(string tenantId, int itemId, CheckoutItemRequest request, TenantSettings settings)
    {
        if (!settings.EnableCheckout)
        {
            _logger.LogWarning($"Checkout disabled for tenant '{tenantId}'");
            return null;
        }

        var item = await _repo.Items.GetByIdAsync(itemId);
        if (item == null)
        {
            _logger.LogWarning($"Item {itemId} not found");
            return null;
        }

        if (item.TenantId != tenantId)
        {
            _logger.LogWarning($"Item tenant mismatch: expected '{tenantId}', but was '{item.TenantId}'");
            return null;
        }

        if (!item.IsActive)
        {
            _logger.LogWarning($"Item {itemId} is not active");
            return null;
        }

        if (item.IsCheckedOut)
        {
            _logger.LogWarning($"Item {itemId} already checked out");
            return null;
        }

        if (!settings.AllowedItemCategories.Contains(item.Category))
        {
            _logger.LogWarning($"Item category '{item.Category}' not allowed for tenant '{tenantId}'");
            return null;
        }

        var userItemsCount = _repo.Items
            .GetAll()
            .Count(i => i.TenantId == tenantId && i.CheckedOutBy == request.Username && i.IsCheckedOut);

        if (userItemsCount >= settings.MaxItemsPerUser)
        {
            _logger.LogWarning($"User '{request.Username}' exceeded max items limit ({settings.MaxItemsPerUser})");
            return null;
        }

        var updatedItem = item with
        {
            IsCheckedOut = true,
            CheckedOutBy = request.Username
        };

        await _repo.Items.UpdateAsync(itemId, updatedItem);
        await _repo.SaveAsync();

        _logger.LogInformation($"Item {itemId} checked out successfully by user '{request.Username}' for tenant '{tenantId}'");

        return _mapper.Map<ItemResponse>(updatedItem);
    }
    public async Task<ItemResponse?> CheckinItemAsync(string tenantId, int itemId, CheckinItemRequest request)
    {
        var item = await _repo.Items.GetByIdAsync(itemId);
        if (item == null || item.TenantId != tenantId || !item.IsActive)
            return null;

        if (!item.IsCheckedOut || item.CheckedOutBy != request.Username)
            return null;

        var updatedItem = item with
        {
            IsCheckedOut = false,
            CheckedOutBy = null
        };

        await _repo.Items.UpdateAsync(itemId, updatedItem);
        await _repo.SaveAsync();

        return _mapper.Map<ItemResponse>(updatedItem);
    }

    public async Task<bool> SoftDeleteItemAsync(string tenantId, int itemId)
    {
        var item = await _repo.Items.GetByIdAsync(itemId);
        if (item == null || item.TenantId != tenantId)
            return false;

        var updatedItem = item with { IsActive = false };
        await _repo.Items.UpdateAsync(itemId, updatedItem);
        await _repo.SaveAsync();

        return true;
    }
}