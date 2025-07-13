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

app.UseCors();

app.UseErrorHandlingMiddleware();

app.UseTenantResolutionMiddleware();

app.MapItemRoutes();

app.Run();


