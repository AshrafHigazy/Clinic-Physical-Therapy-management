using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_Management_System.Models
{
    public class Organization

    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        [NotMapped]
        public List<string> TyppeOfContract { get; set; } = new();
        public DateTime CreateAt { get; set; }

        public bool IsActive { get; set; } = true;

        public string TyppeOfContractSerialized
        {
            get => string.Join(",", TyppeOfContract);
            set => TyppeOfContract = string.IsNullOrEmpty(value)
                ? new()
                : value.Split(',').ToList();
        }
    }
}