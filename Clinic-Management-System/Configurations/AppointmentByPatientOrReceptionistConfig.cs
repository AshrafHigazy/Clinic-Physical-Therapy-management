using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic_Management_System.Configurations
{
    public class AppointmentByPatientOrReceptionistConfig :
        IEntityTypeConfiguration<AppointmentByPatientOrReceptionist>
    {
        public void Configure(EntityTypeBuilder<AppointmentByPatientOrReceptionist> builder)
        {
            builder.HasKey(a => new
            {
                a.AppointmentId,
                a.PatientId,
                a.ReceptionistId
            });

            builder.HasOne(a => a.Appointment)
                .WithOne(ap => ap.AppointmentLink)
                .HasForeignKey<AppointmentByPatientOrReceptionist>(a => a.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // <== يمنع التكرار في المسارات

            builder.HasOne(a => a.Receptionist)
                .WithMany()
                .HasForeignKey(a => a.ReceptionistId)
                .OnDelete(DeleteBehavior.Restrict); // <== نفس الشيء هنا
        }
    }
}
