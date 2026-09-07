using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinic_Management_System.Models
{
    public class Package
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب اختيار المريض")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "يجب اختيار الطبيب")]
        public int InternDoctorId { get; set; }

        [Required(ErrorMessage = "يجب اختيار الفحص")]
        public int CheckId { get; set; }

        public int? OrganizationId { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "يجب اختيار نوع الباقة")]
        public Clinic_Management_System.Models.Enums.PackageType Type { get; set; }

        [Required(ErrorMessage = "عدد الجلسات مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "عدد الجلسات يجب أن يكون رقم أكبر من صفر")]
        public int NumOfSessions { get; set; }

        public int SessionsCount { get; set; } = 0;

        [Required(ErrorMessage = "يجب إدخال المبلغ المدفوع")]
        public decimal AmountPaid { get; set; }

        public string Status { get; set; } = "Active";

        public Organization? Organization { get; set; }
        public Check? Check { get; set; }
        public Patient? Patient { get; set; }
        public InternDoctor? InternDoctor { get; set; }

        public ICollection<TreatmentSession>? TreatmentSessions { get; set; }

        public int UsedSessions => SessionsCount;

        public int RemainingSessions => Math.Max(0, NumOfSessions - SessionsCount);

        public bool IsFinished => RemainingSessions <= 0;
    }
}
