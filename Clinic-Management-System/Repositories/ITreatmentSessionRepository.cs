using Clinic_Management_System.Models;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface ITreatmentSessionRepository
    {
        Task<Patient?> FindPatientAsync(int patientId);
        Task<Package?> GetActivePackageByPatientAsync(int patientId);
        void AddTreatmentSession(TreatmentSession session);
        Task<TreatmentSession?> GetSessionByIdAsync(int id);
        Task<TreatmentSession?> FindSessionAsync(int id);
        void UpdateSession(TreatmentSession session);
        Task<TreatmentSession?> GetSessionWithPackageAsync(int id);
        Task<Package?> FindPackageAsync(int id);
        void UpdatePackage(Package package);
        void RemoveSession(TreatmentSession session);
    }
}