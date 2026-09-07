using Clinic_Management_System.Models;
using Clinic_Management_System.Models.Enums;
using System;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IDashboardRepository
    {
        Task<int> CountPatientsAsync();
        Task<List<Appointment>> GetAppointmentsForDateAsync(DateTime today);
        Task<List<Package>> GetActivePackagesAsync();
        Task<decimal> SumPackagesAmountPaidAsync();
        Task<int> CountAppointmentsOnDateAsync(DateTime targetDate);
        Task<int> CountPackagesByTypeAsync(PackageType type);
        Task<List<Package>> GetCurrentYearPackagesAsync(int currentYear);
        Task<List<Package>> GetTodayPackagesAsync(DateTime today);
        Task<List<Check>> GetTodayChecksAsync(DateTime today);
        Task<int> CountNewPatientsTodayAsync(DateTime today);
        Task<List<Patient>> GetRecentPatientsAsync();
        Task<int> CountActiveInternDoctorsAsync();
        Task<int> CountActiveOrganizationsAsync();
        Task<int> CountActiveReceptionistsAsync();
        Task<List<Package>> GetAllPackagesAsync();
    }
}