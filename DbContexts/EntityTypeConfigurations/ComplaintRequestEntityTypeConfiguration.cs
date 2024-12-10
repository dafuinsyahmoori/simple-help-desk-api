using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleHelpDeskAPI.Entities;

namespace SimpleHelpDeskAPI.DbContexts.EntityTypeConfigurations
{
    public class ComplaintRequestEntityTypeConfiguration : IEntityTypeConfiguration<ComplaintRequest>
    {
        public void Configure(EntityTypeBuilder<ComplaintRequest> builder)
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
                .Property(e => e.ImageOrVideoUrl)
                .HasMaxLength(20)
                .HasColumnName("image_or_video_url");

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
                .Property("UserId")
                .HasColumnName("user_id");



            builder
                .HasOne(e => e.ComplaintResponse)
                .WithOne(e => e.ComplaintRequest)
                .HasForeignKey<ComplaintResponse>("ComplaintRequestId");



            builder.ToTable("complaint_requests");
        }
    }
}