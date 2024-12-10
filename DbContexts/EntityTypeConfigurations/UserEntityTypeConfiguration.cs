using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleHelpDeskAPI.Entities;

namespace SimpleHelpDeskAPI.DbContexts.EntityTypeConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
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
                .Property(e => e.MiddleName)
                .HasMaxLength(15)
                .HasColumnName("middle_name");

            builder
                .Property(e => e.LastName)
                .HasMaxLength(15)
                .HasColumnName("last_name");

            builder
                .Property(e => e.BirthDate)
                .HasColumnName("birth_date")
                .IsRequired();

            builder
                .Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");

            builder
                .Property(e => e.Email)
                .HasMaxLength(60)
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



            builder.ToTable("users");
        }
    }
}