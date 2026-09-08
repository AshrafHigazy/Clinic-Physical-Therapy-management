using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Common;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.TreatmentSessions
{
    public interface ITreatmentSessionService
    {
        Task<ServiceResult<int>> CreateSessionAsync(int patientId);
        Task<TreatmentSession?> GetSessionByIdAsync(int id);
        Task<TreatmentSession?> GetSessionWithPackageAsync(int id);
        Task<TreatmentSession?> FindSessionAsync(int id);
        Task<ServiceResult<int>> EditPrognosisAsync(int id, string prognosis);
        Task<ServiceResult<int>> DeleteSessionAsync(int id);
    }
}
