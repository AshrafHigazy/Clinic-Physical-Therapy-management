using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Checks
{
    public interface ICheckService
    {
        Task<List<Check>> GetChecksAsync();
        Task<Check?> GetCheckDetailsAsync(int? id);
        Patient? GetPatientById(int id);
        List<Patient> GetPatientsForSelect();
        Task<ServiceResult> CreateCheckAsync(Check check);
        Task<Check?> GetCheckForEditAsync(int? id);
        Task<ServiceResult> UpdateCheckAsync(int id, Check check);
        Task<ServiceResult> DeleteCheckAsync(int id);
    }
}
