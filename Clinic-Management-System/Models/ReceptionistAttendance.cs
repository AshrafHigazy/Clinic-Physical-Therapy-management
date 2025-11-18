using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_Management_System.Models
{
    public class ReceptionistAttendance
    {

        [Required]
        [ForeignKey(nameof(Receptionist))]
        public int ReceptionistId { get; set; }

        [Key]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }  

        [DataType(DataType.DateTime)]
        public DateTime? CheckIn { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CheckOut { get; set; }

        [Range(0, 24, ErrorMessage = "Hours must be between 0 and 24")]
        public int? Hours { get; set; }

        // 🔹 Navigation Property
        public Receptionist? Receptionist { get; set; }
    }
}
