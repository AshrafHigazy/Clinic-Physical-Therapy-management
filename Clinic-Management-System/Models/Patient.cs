using System.ComponentModel.DataAnnotations;
using Clinic_Management_System.Models;
using Clinic_Management_System.Models.Enums;

public class Patient
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "الاسم بالكامل مطلوب")]
    public string FullName { get; set; }

    [StringLength(200, ErrorMessage = "العنوان لا يمكن أن يزيد عن 200 حرف")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [Phone(ErrorMessage = "رقم الهاتف غير صالح")]
    [StringLength(11, ErrorMessage = "رقم الهاتف لا يمكن أن يزيد عن 15 رقم")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "النوع مطلوب")]
    public Gender Gender { get; set; }

    [Required]
    [DataType(DataType.Date, ErrorMessage = "صيغة التاريخ غير صحيحة")]
    public DateTime CreateAt { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "العمر مطلوب")]
    public int Age { get; set; }

    // Navigation Properties
    public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<Appointment>? Appointments { get; set; }
    public ICollection<Package>? Packages { get; set; }

    public ICollection<AppointmentByPatientOrReceptionist>? AppointmentLinks { get; set; }
    public ICollection<Check> Checks { get; set; } = new List<Check>();

}