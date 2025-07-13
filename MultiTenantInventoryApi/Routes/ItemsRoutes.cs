using MultiTenantInventoryApi.Contracts;
using MultiTenantInventoryApi.Model.DTOs.Requests;
using MultiTenantInventoryApi.Model.DTOs.Responses;

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

    static async Task<Ok<List<ItemResponse>>> GetAll(
        HttpContext httpContext,
        IItemService itemService)
    {
        var tenantId = httpContext.GetTenantId();
        var items = await itemService.GetItemsAsync(tenantId);
        return TypedResults.Ok(items);
    }

    static async Task<Created<ItemResponse>> Create(
        CreateItemRequest request,
        HttpContext httpContext,
        IItemService itemService)
    {
        var tenantId = httpContext.GetTenantId();
        var itemResponse = await itemService.CreateItemAsync(tenantId, request);
        return TypedResults.Created($"/api/items/{itemResponse.Id}", itemResponse);
    }

    static async Task<Results<Ok<ItemResponse>, BadRequest<string>>> Checkout(
        int id,
        CheckoutItemRequest request,
        HttpContext httpContext,
        IItemService itemService)
    {
        var tenantId = httpContext.GetTenantId();
        var settings = httpContext.GetTenantSettings();

        var result = await itemService.CheckoutItemAsync(tenantId, id, request, settings);
        if (result == null)
            return TypedResults.BadRequest("Checkout not allowed or validation failed.");

        return TypedResults.Ok(result);
    }

    static async Task<Results<Ok<ItemResponse>, BadRequest<string>>> Checkin(
        int id,
        CheckinItemRequest request,
        HttpContext httpContext,
        IItemService itemService)
    {
        var tenantId = httpContext.GetTenantId();

        var result = await itemService.CheckinItemAsync(tenantId, id, request);
        if (result == null)
            return TypedResults.BadRequest("Checkin not allowed or validation failed.");

        return TypedResults.Ok(result);
    }

    static async Task<Results<NoContent, NotFound>> SoftDelete(
        int id,
        HttpContext httpContext,
        IItemService itemService)
    {
        var tenantId = httpContext.GetTenantId();

        var deleted = await itemService.SoftDeleteItemAsync(tenantId, id);
        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }

    static async Task<Results<Ok<string>, BadRequest<string>>> SpecialReport(
        HttpContext httpContext)
    {
        var tenantId = httpContext.GetTenantId();
        var settings = httpContext.GetTenantSettings();

        if (settings.Features?.SpecialReport != true)
            return TypedResults.BadRequest("SpecialReport feature is not enabled for this tenant.");

        var report = $"Special report for tenant {tenantId} generated at {DateTime.UtcNow}";
        return TypedResults.Ok(report);
    }
}
