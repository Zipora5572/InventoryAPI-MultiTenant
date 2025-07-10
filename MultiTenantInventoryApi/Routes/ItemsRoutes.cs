namespace MultiTenantInventoryApi.Routes;

public static class ItemRoutes
{
    public static IEndpointRouteBuilder MapItemRoutes(this IEndpointRouteBuilder app)
    {
        var itemsGroup = app.MapGroup("/api/items");

        itemsGroup.MapGet("", GetAll);
        itemsGroup.MapPost("", Create);
        itemsGroup.MapPost("{id}/checkout", Checkout);
        itemsGroup.MapPost("{id}/checkin", Checkin);
        itemsGroup.MapDelete("{id}", SoftDelete);
        itemsGroup.MapGet("SpecialReport", SpecialReport);


        return app;
    }

    static async Task<Results<Ok<List<ItemResponse>>, UnauthorizedHttpResult>> GetAll(
        HttpContext httpContext,
        ILogger<Item> logger,
        IItemService itemService)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantId))
        {
            logger.LogWarning("GetAll failed: Missing X-Tenant-ID header");
            return TypedResults.Unauthorized();
        }

        var items = await itemService.GetItemsAsync(tenantId);
        logger.LogInformation("GetAll items for tenant {TenantId}", tenantId);
        return TypedResults.Ok(items);
    }

    static async Task<Results<Created<ItemResponse>, UnauthorizedHttpResult>> Create(
        CreateItemRequest request,
        HttpContext httpContext,
        ILogger<Item> logger,
        IItemService itemService)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantId))
        {
            logger.LogWarning("Create failed: Missing X-Tenant-ID header");
            return TypedResults.Unauthorized();
        }

        var itemResponse = await itemService.CreateItemAsync(tenantId, request);
        logger.LogInformation("Created item {ItemId} for tenant {TenantId}", itemResponse.Id, tenantId);
        return TypedResults.Created($"/api/items/{itemResponse.Id}", itemResponse);
    }

    static async Task<Results<Ok<ItemResponse>, BadRequest<string>, UnauthorizedHttpResult>> Checkout(
    int id,
    CheckoutItemRequest request,
    HttpContext httpContext,
    ILogger<Item> logger,
    IItemService itemService,
    IOptionsMonitor<Dictionary<string, TenantSettings>> tenantSettingsMonitor)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantId))
        {
            logger.LogWarning("Checkout failed: Missing X-Tenant-ID header");
            return TypedResults.Unauthorized();
        }

        var tenantSettingsMap = tenantSettingsMonitor.Get(Options.DefaultName);

        if (!tenantSettingsMap.TryGetValue(tenantId, out var settings))
        {
            logger.LogWarning("Checkout failed: No settings found for tenant {TenantId}", tenantId);
            return TypedResults.Unauthorized();
        }

        if (!settings.EnableCheckout)
        {
            logger.LogWarning("Checkout disabled for tenant '{TenantId}'", tenantId);
            return TypedResults.BadRequest("Checkout not allowed or validation failed.");
        }

        var result = await itemService.CheckoutItemAsync(tenantId, id, request, settings);
        if (result == null)
        {
            logger.LogWarning("Checkout validation failed for item {ItemId} tenant {TenantId}", id, tenantId);
            return TypedResults.BadRequest("Checkout not allowed or validation failed.");
        }

        logger.LogInformation("Item {ItemId} checked out by {Username} for tenant {TenantId}", id, request.Username, tenantId);
        return TypedResults.Ok(result);
    }


    static async Task<Results<Ok<ItemResponse>, BadRequest<string>, UnauthorizedHttpResult>> Checkin(
        int id,
        CheckinItemRequest request,
        HttpContext httpContext,
        ILogger<Item> logger,
        IItemService itemService)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantId))
        {
            logger.LogWarning("Checkin failed: Missing X-Tenant-ID header");
            return TypedResults.Unauthorized();
        }

        var result = await itemService.CheckinItemAsync(tenantId, id, request);
        if (result == null)
        {
            logger.LogWarning("Checkin validation failed for item {ItemId} tenant {TenantId}", id, tenantId);
            return TypedResults.BadRequest("Checkin not allowed or validation failed.");
        }

        logger.LogInformation("Item {ItemId} checked in by {Username} for tenant {TenantId}", id, request.Username, tenantId);
        return TypedResults.Ok(result);
    }

    static async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> SoftDelete(
        int id,
        HttpContext httpContext,
        ILogger<Item> logger,
        IItemService itemService)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantId))
        {
            logger.LogWarning("SoftDelete failed: Missing X-Tenant-ID header");
            return TypedResults.Unauthorized();
        }

        var deleted = await itemService.SoftDeleteItemAsync(tenantId, id);
        if (!deleted)
        {
            logger.LogWarning("SoftDelete failed: Item {ItemId} not found for tenant {TenantId}", id, tenantId);
            return TypedResults.NotFound();
        }

        logger.LogInformation("Item {ItemId} soft deleted for tenant {TenantId}", id, tenantId);
        return TypedResults.NoContent();
    }

    static async Task<Results<Ok<string>, BadRequest<string>, UnauthorizedHttpResult>> SpecialReport(
    HttpContext httpContext,
    ILogger<Item> logger,
    IItemService itemService,
    IOptionsMonitor<Dictionary<string, TenantSettings>> tenantSettingsMonitor)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantId))
        {
            logger.LogWarning("SpecialReport failed: Missing X-Tenant-ID header");
            return TypedResults.Unauthorized();
        }

        var tenantSettingsMap = tenantSettingsMonitor.Get(Options.DefaultName);

        if (!tenantSettingsMap.TryGetValue(tenantId, out var settings))
        {
            logger.LogWarning("SpecialReport failed: No settings found for tenant {TenantId}", tenantId);
            return TypedResults.Unauthorized();
        }

        if (settings.Features == null || !settings.Features.SpecialReport)
        {
            logger.LogWarning("SpecialReport feature disabled for tenant '{TenantId}'", tenantId);
            return TypedResults.BadRequest("SpecialReport feature is not enabled for this tenant.");
        }

        var report = $"Special report for tenant {tenantId} generated at {DateTime.UtcNow}";

        logger.LogInformation("SpecialReport generated for tenant {TenantId}", tenantId);
        return TypedResults.Ok(report);
    }


}
