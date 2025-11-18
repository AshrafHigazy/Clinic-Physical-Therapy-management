using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic_Management_System.Configurations
{
    public class CheckConfiguration : IEntityTypeConfiguration<Check>
    {
        public void Configure(EntityTypeBuilder<Check> builder)
        {
            builder.ToTable("Checks");

            builder.HasKey(c => c.CheckId);

            builder.Property(c => c.Sugestion)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(c => c.ClinicAssessment)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(c => c.Diagnosis)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(c => c.PlaneOfTreatment)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(c => c.MethodsOfTreatment)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne(c => c.Patient)
                   .WithMany(p => p.Checks)
                   .HasForeignKey(c => c.PatientId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Packages)
                   .WithOne(p => p.Check)
                   .HasForeignKey(p => p.CheckId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            // ✅ هنا التعديل المهم
            builder.Property(c => c.CreatedAt)
                   .HasColumnType("datetime2") // أكثر دقة من datetime
                   .HasDefaultValueSql("GETDATE()") // يُنشأ تلقائيًا في SQL
                   .ValueGeneratedOnAdd(); // يتولد تلقائيًا عند الإنشاء
        }
    }
}
