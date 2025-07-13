using MultiTenantInventoryApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddApplicationServices().
    AddApplicationConfiguration();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseErrorHandlingMiddleware();

app.UseTenantResolutionMiddleware();

app.UseCors();
app.MapItemRoutes();

app.Run();


