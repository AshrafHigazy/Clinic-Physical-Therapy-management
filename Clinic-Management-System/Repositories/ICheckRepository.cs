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
        void AddCheck(Check check);
        Task<Check?> GetCheckForEditAsync(int? id);
        void UpdateCheck(Check check);
        bool CheckExists(int id);
        Task<Check?> FindCheckAsync(int id);
        void RemoveCheck(Check check);
    }
}