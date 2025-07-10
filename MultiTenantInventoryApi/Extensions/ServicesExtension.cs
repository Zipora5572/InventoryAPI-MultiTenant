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
        ));

        builder.Services.AddDbContext<IDataContext, DataContext>(options =>
                options.UseSqlite("Data Source=inventory.db"));


        builder.Services.AddScoped<IItemService, ItemService>();
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

        builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        return builder;
    }
}
