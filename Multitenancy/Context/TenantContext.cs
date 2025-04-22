using Microsoft.EntityFrameworkCore;
using Multitenancy.Finbuckle_Multitenant;
using Multitenancy.Models;

namespace Multitenancy.Context
{
    public class TenantContext : DbContext
    {
        public TenantContext(DbContextOptions<TenantContext> options) : base(options) { }
        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<LoginTenant> LoginTenant { get; set; } = null!;
    }
}
