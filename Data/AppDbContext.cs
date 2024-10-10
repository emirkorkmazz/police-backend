using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Police.Data
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(Configuration.GetConnectionString("PoliceDatabase"));
        }

        public DbSet<PoliciesModel> Policies { get; set; }
        public DbSet<CustomersModel> Customers { get; set; }
        public DbSet<CompaniesModel> Companies { get; set; }
        public DbSet<ReferencesModel> References { get; set; }
        public DbSet<IncomeModel> Incomes { get; set; }
        public DbSet<ProductsModel> Products { get; set; }
        public DbSet<UserModel> Users { get; set; }

    }
}
