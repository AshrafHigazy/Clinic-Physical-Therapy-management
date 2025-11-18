using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic_Management_System.Configurations
{
    public class InternDoctorConfig : IEntityTypeConfiguration<InternDoctor>
    {
        public void Configure(EntityTypeBuilder<InternDoctor> builder)
        {
            builder.HasKey(d => d.InternDoctorId);

            builder.Property(d => d.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.Phone)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(d => d.Notes)
                   .HasMaxLength(500);

            builder.Property(d => d.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.HasMany(d => d.Attendances)
                   .WithOne(a => a.InternDoctor)
                   .HasForeignKey(a => a.InternDoctorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Packages)
                   .WithOne(p => p.InternDoctor)
                   .HasForeignKey(p => p.InternDoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
