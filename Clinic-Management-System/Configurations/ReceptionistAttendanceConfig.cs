using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_System.Configurations
{
    public class ReceptionistAttendanceConfig :
    IEntityTypeConfiguration<ReceptionistAttendance>
    {
        public void Configure(EntityTypeBuilder<ReceptionistAttendance>builder)
        {
        builder.HasKey(a => new { a.ReceptionistId, a.Date });
            builder.Property(a => a.Hours)
           .HasComputedColumnSql("DATEDIFF(HOUR, [CheckIn], [CheckOut])", stored: true);
        }
    }

}
