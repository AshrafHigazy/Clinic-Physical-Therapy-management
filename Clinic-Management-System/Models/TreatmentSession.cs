using System;

namespace Clinic_Management_System.Models
{
    public class TreatmentSession
    {
        public int Id { get; set; }
        public int PackageId { get; set; }

        public DateTime SessionDate { get; set; }

        public string Prognosis { get; set; } = null!;

        public Package Package { get; set; } = null!;
    }
}
