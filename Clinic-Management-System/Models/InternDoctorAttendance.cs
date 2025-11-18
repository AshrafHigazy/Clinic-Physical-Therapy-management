using System.ComponentModel.DataAnnotations;

namespace Clinic_Management_System.Models
{
    public class InternDoctorAttendance
    {
        public int InternDoctorId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public double? Hours { get; set; }

        // Navigation
        [Required]
        public InternDoctor InternDoctor { get; set; } = null!;
    }
}
