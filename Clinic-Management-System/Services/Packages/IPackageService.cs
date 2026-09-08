using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Packages
{
    public class PackageFormData
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";
        public List<Check> Checks { get; set; } = new();
        public List<Organization> Organizations { get; set; } = new();
        public string InfoMessage { get; set; } = "";
    }

    public class PackageEditData
    {
        public Package Package { get; set; } = null!;
        public List<InternDoctor> Doctors { get; set; } = new();
        public List<Organization> Organizations { get; set; } = new();
        public List<Check> Checks { get; set; } = new();
    }

    public interface IPackageService
    {
        PackageFormData? GetCreateFormData(int patientId);
        List<DoctorSearchDto> SearchDoctors(string term);
        Task<ServiceResult> CreatePackageAsync(int patientId, Package package);
        Task<PackageEditData?> GetEditFormDataAsync(int id);
        Task<ServiceResult> UpdatePackageAsync(int id, Package package);
        Task<Package?> GetPackageDetailsAsync(int id);
        Task<List<Package>> GetAllPackagesSortedAsync();
        Task<(List<Package> packages, Patient? patient)> GetPatientPackagesAsync(int patientId);
        List<Organization> GetAllOrganizations();
        List<Check> GetChecksByPatient(int patientId);
        List<InternDoctor> GetActiveInternDoctors();
    }
}
