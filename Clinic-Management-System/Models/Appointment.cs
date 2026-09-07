using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_Management_System.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }

        [Required]
        [ForeignKey(nameof(Receptionist))]
        public int ReceptionistId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }

        public bool IsAttended { get; set; } = false;
        public bool IsCanceled { get; set; } = false;

        public bool IsWithMainDoctor { get; set; }

        [Required]
        public Patient Patient { get; set; } = null!;
        [Required]
        public Receptionist Receptionist { get; set; } = null!;
        [Required]
        public AppointmentByPatientOrReceptionist AppointmentLink { get; set; } = null!;
    }
}
