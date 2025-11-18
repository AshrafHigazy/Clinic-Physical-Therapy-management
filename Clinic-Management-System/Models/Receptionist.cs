using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_Management_System.Models
{
    public class Receptionist
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Hiring date is required")]
        [DataType(DataType.Date)]
        public DateTime HiringDate { get; set; }= DateTime.Now;

        [Required(ErrorMessage = "Salary is required")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Shift is required")]
        [StringLength(50, ErrorMessage = "Shift cannot exceed 50 characters")]
        public string Shift { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15)]
        public string Phone { get; set; }

        public bool IsActive { get; set; } = true;

        //[Required(ErrorMessage = "Password is required")]
        //[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }


        [Required(ErrorMessage = "Role is required")]
        [StringLength(50)]
        public string Role { get; set; }

        //[EmailAddress(ErrorMessage = "Invalid email format")]
        //[StringLength(100)]
        //public string Email { get; set; }

        // 🔹 Navigation Properties
        public ICollection<ReceptionistAttendance>? Attendances { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
        [Required]
        public ICollection<AppointmentByPatientOrReceptionist> AppointmentLinks { get; set; } = new List<AppointmentByPatientOrReceptionist>();
    }
}