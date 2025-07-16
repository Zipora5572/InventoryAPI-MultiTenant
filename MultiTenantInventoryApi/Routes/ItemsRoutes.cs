
namespace MultiTenantInventoryApi.Routes;

public static class ItemRoutes
{
    public static IEndpointRouteBuilder MapItemRoutes(this IEndpointRouteBuilder app)
    {
        var itemsGroup = app.MapGroup("/api/items");

        itemsGroup.MapGet("", GetAll);
        itemsGroup.MapPost("", Create);
        itemsGroup.MapPut("{id}", Update);
        itemsGroup.MapPost("{id}/checkout", Checkout);
        itemsGroup.MapPost("{id}/checkin", Checkin);
        itemsGroup.MapDelete("{id}", SoftDelete);
        itemsGroup.MapGet("SpecialReport", SpecialReport);

        return app;
    }

    static async Task<Ok<List<ItemResponse>>> GetAll(
        IItemService itemService,
        ITenantProvider tenantProvider)
    {
        var tenantId = tenantProvider.TenantId;
        var items = await itemService.GetItemsAsync(tenantId);
        return TypedResults.Ok(items);
    }

    static async Task<Created<ItemResponse>> Create(
        CreateItemRequest request,
        IItemService itemService,
        ITenantProvider tenantProvider,
        IConnectionProvider connectionProvider)
    {
        var tenantId = tenantProvider.TenantId;
        var connectionId = connectionProvider.ConnectionId;

        var response = await itemService.CreateItemAsync(tenantId, request, connectionId);
        return TypedResults.Created($"/api/items/{response.Id}", response);
    }

    static async Task<Results<Ok<ItemResponse>, NotFound>> Update(
        int id,
        UpdateItemRequest request,
        IItemService itemService,
        ITenantProvider tenantProvider,
        IConnectionProvider connectionProvider)
    {
        var tenantId = tenantProvider.TenantId;
        var connectionId = connectionProvider.ConnectionId;

        var updated = await itemService.UpdateItemAsync(tenantId, id, request, connectionId);

        return updated is not null
            ? TypedResults.Ok(updated)
            : TypedResults.NotFound();
    }

    static async Task<Results<Ok<ItemResponse>, BadRequest<string>>> Checkin(
        int id,
        CheckinItemRequest request,
        IItemService itemService,
        ITenantProvider tenantProvider,
        IConnectionProvider connectionProvider)
    {
        var tenantId = tenantProvider.TenantId;
        var connectionId = connectionProvider.ConnectionId;

        var result = await itemService.CheckinItemAsync(tenantId, id, request, connectionId);

        return result is not null
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest("Checkin not allowed or validation failed.");
    }

    static async Task<Results<Ok<ItemResponse>, BadRequest<string>>> Checkout(
        int id,
        CheckoutItemRequest request,
        IItemService itemService,
        ITenantProvider tenantProvider,
        IConnectionProvider connectionProvider)
    {
        var tenantId = tenantProvider.TenantId;
        var settings = tenantProvider.Settings;
        var connectionId = connectionProvider.ConnectionId;

        var result = await itemService.CheckoutItemAsync(tenantId, id, request, settings, connectionId);

        return result is not null
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest("Checkout not allowed or validation failed.");
    }

    static async Task<Results<NoContent, NotFound>> SoftDelete(
        int id,
        IItemService itemService,
        ITenantProvider tenantProvider,
        IConnectionProvider connectionProvider)
    {
        var tenantId = tenantProvider.TenantId;
        var connectionId = connectionProvider.ConnectionId;

        var deleted = await itemService.SoftDeleteItemAsync(tenantId, id, connectionId);
        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }

    static Task<Results<Ok<string>, BadRequest<string>>> SpecialReport(
        ITenantProvider tenantProvider)
    {
        var tenantId = tenantProvider.TenantId;
        var settings = tenantProvider.Settings;

        if (settings.Features?.SpecialReport != true)
        {
            return Task.FromResult<Results<Ok<string>, BadRequest<string>>>(
                TypedResults.BadRequest("SpecialReport feature is not enabled for this tenant.")
            );
        }

        var report = $"Special report for tenant {tenantId} generated at {DateTime.UtcNow}";

        return Task.FromResult<Results<Ok<string>, BadRequest<string>>>(
            TypedResults.Ok(report)
        );
    }
}
