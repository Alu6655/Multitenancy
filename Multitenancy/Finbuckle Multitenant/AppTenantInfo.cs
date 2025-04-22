using Finbuckle.MultiTenant;

namespace Multitenancy.Finbuckle_Multitenant
{
    public class AppTenantInfo :TenantInfo
    {
        public string? ConnectionString { get; set; }
    }
}
