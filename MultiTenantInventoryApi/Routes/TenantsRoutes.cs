
namespace MultiTenantInventoryApi.Routes;

public static class TenantsRoutes
{
    public static IEndpointRouteBuilder MapTenantRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tenants");

        group.MapGet("", GetAll);
        group.MapPost("", Create);
        group.MapDelete("{id:int}", Delete);

        return app;
    }

    static async Task<Ok<List<Tenant>>> GetAll(ITenantService tenantService)
    {
        var tenants = await tenantService.GetAllTenantsAsync();
        return TypedResults.Ok(tenants);
    }

    static async Task<Created<Tenant>> Create(
      Tenant request,
        ITenantService tenantService)
    {
        var tenant = await tenantService.CreateTenantAsync(request);
        return TypedResults.Created($"/api/tenants/{tenant.Id}", tenant);
    }

    static async Task<Results<NoContent, NotFound>> Delete(
        int id,
        ITenantService tenantService)
    {
        var success = await tenantService.DeleteTenantAsync(id);

        return success
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}
