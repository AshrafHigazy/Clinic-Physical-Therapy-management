using System.ComponentModel.DataAnnotations;

namespace Clinic_Management_System.Models
{
    public class InternDoctor
    {
        public int InternDoctorId { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم لا يمكن أن يتعدى 100 حرف")]
        public string FullName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "رقم الهاتف يجب أن يكون 11 رقم بالضبط")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ✅ نعمل Initialize للقوائم عشان مايبقوش null
        public ICollection<InternDoctorAttendance> Attendances { get; set; } = new List<InternDoctorAttendance>();
        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
