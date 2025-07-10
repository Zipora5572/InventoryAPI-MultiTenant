namespace MultiTenantInventoryApi.Model.Config;
public class TenantSettings
{
    public bool EnableCheckout { get; set; }
    public int MaxItemsPerUser { get; set; }
    public List<string> AllowedItemCategories { get; set; } = new();
    public FeaturesSettings Features { get; set; } = new FeaturesSettings();
}

