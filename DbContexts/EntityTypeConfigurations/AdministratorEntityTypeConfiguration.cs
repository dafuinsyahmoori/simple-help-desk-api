using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleHelpDeskAPI.Entities;

namespace SimpleHelpDeskAPI.DbContexts.EntityTypeConfigurations
{
    public class AdministratorEntityTypeConfiguration : IEntityTypeConfiguration<Administrator>
    {
        public void Configure(EntityTypeBuilder<Administrator> builder)
        {
            builder
                .Property(e => e.Id)
                .HasColumnName("id");

            builder
                .Property(e => e.FirstName)
                .HasMaxLength(15)
                .HasColumnName("first_name")
                .IsRequired();

            builder
                .Property(e => e.LastName)
                .HasMaxLength(15)
                .HasColumnName("last_name");

            builder
                .Property(e => e.Email)
                .HasMaxLength(45)
                .HasColumnName("email")
                .IsRequired();

            builder
                .Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password")
                .IsRequired();



            builder
                .HasIndex(e => e.Email)
                .IsUnique();



            builder.ToTable("administrators");
        }
    }
}