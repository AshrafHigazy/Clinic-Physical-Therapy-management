using Clinic_Management_System.Models;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface ICheckRepository
    {
        Task<List<Check>> GetChecksAsync();
        Task<Check?> GetCheckDetailsAsync(int? id);
        Patient? GetPatientById(int id);
        List<Patient> GetPatientsForSelect();
        Task AddCheckAsync(Check check);
        Task<Check?> GetCheckForEditAsync(int? id);
        Task UpdateCheckAsync(Check check);
        bool CheckExists(int id);
        Task<Check?> FindCheckAsync(int id);
        Task RemoveCheckAsync(Check check);
    }
}