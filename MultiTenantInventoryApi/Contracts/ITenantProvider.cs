namespace MultiTenantInventoryApi.Contracts;

public interface ITenantProvider
{
    string TenantId { get; }
    TenantSettings Settings { get; }
}
