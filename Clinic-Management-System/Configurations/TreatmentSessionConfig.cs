using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic_Management_System.Configurations
{
    public class TreatmentSessionConfig : IEntityTypeConfiguration<TreatmentSession>
    {
        public void Configure(EntityTypeBuilder<TreatmentSession> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Package)
                   .WithMany(p => p.TreatmentSessions)
                   .HasForeignKey(t => t.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.Prognosis)
                   .HasMaxLength(200)
                   .IsRequired();
            builder.Property(t => t.SessionDate)
       .HasDefaultValueSql("GETDATE()");
        }
    }

}
