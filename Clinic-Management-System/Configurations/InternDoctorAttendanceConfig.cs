    using Clinic_Management_System.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    namespace Clinic_Management_System.Configurations
    {
        public class InternDoctorAttendanceConfig : IEntityTypeConfiguration<InternDoctorAttendance>
        {
            public void Configure(EntityTypeBuilder<InternDoctorAttendance> builder)
            {
                builder.HasKey(a => new { a.InternDoctorId, a.Date });

                // نخلي Hours تتحسب أوتوماتيك داخل SQL Server
                
            }
        }
    }
