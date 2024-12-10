using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleHelpDeskAPI.Entities;

namespace SimpleHelpDeskAPI.DbContexts.EntityTypeConfigurations
{
    public class ComplaintResponseEntityTypeConfiguration : IEntityTypeConfiguration<ComplaintResponse>
    {
        public void Configure(EntityTypeBuilder<ComplaintResponse> builder)
        {
            builder
                .Property(e => e.Id)
                .HasColumnName("id");

            builder
                .Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title")
                .IsRequired();

            builder
                .Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content")
                .IsRequired();

            builder
                .Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(now())")
                .IsRequired();

            builder
                .Property("AdministratorId")
                .HasColumnName("administrator_id");

            builder
                .Property<int>("ComplaintRequestId")
                .HasColumnName("complaint_request_id")
                .IsRequired();



            builder.ToTable("complaint_responses");
        }
    }
}