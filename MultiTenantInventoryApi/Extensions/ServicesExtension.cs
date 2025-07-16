namespace MultiTenantInventoryApi.Extensions;

public static class ServicesExtension
{
    public static WebApplicationBuilder AddApplicationConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<Dictionary<string, TenantSettings>>(
                 builder.Configuration.GetSection("TenantSettings"));
        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddCors(x => x.AddDefaultPolicy(o =>
            o.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
           .AllowCredentials()
        ));

        builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlite("Data Source=inventory.db"));

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ITenantProvider, TenantProvider>();
        builder.Services.AddScoped<IConnectionProvider, ConnectionProvider>();

        builder.Services.AddScoped<IItemService, ItemService>();
        builder.Services.AddScoped<ITenantService, TenantService>();
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<ITenantRepository, TenantRepository>();
        builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
        builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();

        builder.Services.AddScoped<IInventoryBroadcaster, InventoryBroadcaster>();


        builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        builder.Services.AddSignalR();

        return builder;
    }
}
