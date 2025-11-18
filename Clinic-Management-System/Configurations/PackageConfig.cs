using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Clinic_Management_System.Models;


namespace Clinic_Management_System.Configurations
{
    public class PackageConfig : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.ToTable("Packages");


            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Patient)
                   .WithMany(pat => pat.Packages)
                   .HasForeignKey(p => p.PatientId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Organization)
                   .WithMany()
                   .HasForeignKey(p => p.OrganizationId)


                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Check)
                   .WithMany(c => c.Packages)
                   .HasForeignKey(p => p.CheckId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Type)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(p => p.Status)
                   .HasMaxLength(50);

            builder.Property(p => p.NumOfSessions)
                   .IsRequired();

            builder.Property(p => p.AmountPaid)
                   .HasColumnType("decimal(18,2)");
        }
    }
}