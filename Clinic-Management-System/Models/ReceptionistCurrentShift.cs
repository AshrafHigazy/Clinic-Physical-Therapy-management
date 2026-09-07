using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_Management_System.Models
{
    public class ReceptionistCurrentShift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ReceptionistId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        public Receptionist? Receptionist { get; set; }
    }
}
