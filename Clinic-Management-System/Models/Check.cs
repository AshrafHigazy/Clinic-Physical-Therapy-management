using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Clinic_Management_System.Models
{
    public class Check
    { 
        public int CheckId { get; set; }
        public int PatientId { get; set; }
        [Required(ErrorMessage = "حقل (مين رشحنا ليك) مطلوب.")]
        public string Sugestion { get; set; }
        [Required(ErrorMessage = "حقل (التقييم السريري) مطلوب.")]

        public string ClinicAssessment { get; set; }
        [Required(ErrorMessage = "حقل (التشخيص) مطلوب.")]

        public string Diagnosis { get; set; }
        [Required(ErrorMessage = "حقل (خطة العلاج) مطلوب.")]

        public string PlaneOfTreatment { get; set; }
        [Required(ErrorMessage = "حقل (طرق العلاج) مطلوب.")]

        public string MethodsOfTreatment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; 


        // Navigation(mandatory)
        public Patient? Patient { get; set; }
        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
