using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_Management_System.Models
{
    public class AppointmentByPatientOrReceptionist
    {

        [Required]
        [ForeignKey(nameof(Appointment))]
        public int AppointmentId { get; set; }

        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }

        [Required]
        [ForeignKey(nameof(Receptionist))]
        public int ReceptionistId { get; set; }

        // Navigation Properties
        [Required]
        public Appointment Appointment { get; set; } = null!;
        [Required]
        public Patient Patient { get; set; } = null!;
        public Receptionist? Receptionist { get; set; }
    }
}
