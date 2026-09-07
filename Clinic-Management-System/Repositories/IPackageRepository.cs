using Clinic_Management_System.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IPackageRepository
    {
        Patient? GetPatientWithPackages(int patientId);
        List<Check> GetChecksByPatient(int patientId);
        List<Organization> GetAllOrganizations();
        List<DoctorSearchDto> SearchDoctors(string term);
        void AddPackage(Package package);
        Task<Package?> GetPackageWithPatientAndOrgAsync(int id);
        List<InternDoctor> GetActiveInternDoctors();
        Task<Package?> GetPackageAsNoTrackingAsync(int id);
        void UpdatePackage(Package package);
        Task<Package?> GetPackageDetailsAsync(int id);
        Task<List<Package>> GetAllPackagesWithIncludesAsync();
        Task<List<Package>> GetPatientPackagesAsync(int patientId);
        Task<Patient?> FindPatientAsync(int patientId);
    }
}