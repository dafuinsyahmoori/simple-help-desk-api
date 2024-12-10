using Microsoft.EntityFrameworkCore;
using SimpleHelpDeskAPI.DbContexts.EntityTypeConfigurations;
using SimpleHelpDeskAPI.DbContexts.ValueComparers;
using SimpleHelpDeskAPI.DbContexts.ValueConverters;
using SimpleHelpDeskAPI.Entities;

namespace SimpleHelpDeskAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<ComplaintRequest> ComplaintRequests => Set<ComplaintRequest>();
        public DbSet<ComplaintResponse> ComplaintResponses => Set<ComplaintResponse>();
        public DbSet<Administrator> Administrators => Set<Administrator>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<DateOnly>()
                .HaveConversion<DateOnlyValueConverter, DateOnlyValueComparer>()
                .HaveColumnType("date");

            configurationBuilder
                .Properties<DateTime>()
                .HaveColumnType("datetime");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new UserEntityTypeConfiguration())
                .ApplyConfiguration(new ComplaintRequestEntityTypeConfiguration())
                .ApplyConfiguration(new ComplaintResponseEntityTypeConfiguration())
                .ApplyConfiguration(new AdministratorEntityTypeConfiguration());
        }
    }
}