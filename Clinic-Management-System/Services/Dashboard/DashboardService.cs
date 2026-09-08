using Clinic_Management_System.Models.Enums;
using Clinic_Management_System.Models.ViewModels;
using Clinic_Management_System.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardViewModel> GetDashboardMetricsAsync()
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Saturday)) % 7;
            var saturday = today.AddDays(-1 * diff).Date;
            var currentYear = today.Year;

            var model = new DashboardViewModel();
            model.TotalPatients = await _unitOfWork.Dashboard.CountPatientsAsync();

            var appointmentsToday = await _unitOfWork.Dashboard.GetAppointmentsForDateAsync(today);

            model.TodayAppointmentsCount = appointmentsToday.Count;
            model.TodayAttended = appointmentsToday.Count(a => a.IsAttended);
            model.TodayCanceled = appointmentsToday.Count(a => a.IsCanceled);

            var activePackages = await _unitOfWork.Dashboard.GetActivePackagesAsync();
            model.ActivePackagesCount = activePackages.Count;
            model.TotalRevenue = await _unitOfWork.Dashboard.SumPackagesAmountPaidAsync();

            var weeklyAppointments = new int[7];
            for (int i = 0; i < 7; i++)
            {
                var targetDate = saturday.AddDays(i);
                weeklyAppointments[i] = await _unitOfWork.Dashboard.CountAppointmentsOnDateAsync(targetDate);
            }
            model.WeeklyAppointments = weeklyAppointments;

            var distribution = new int[5];
            distribution[0] = await _unitOfWork.Dashboard.CountPackagesByTypeAsync(PackageType.ذهبي);
            distribution[1] = await _unitOfWork.Dashboard.CountPackagesByTypeAsync(PackageType.فضي);
            distribution[2] = await _unitOfWork.Dashboard.CountPackagesByTypeAsync(PackageType.الماسي);
            distribution[3] = await _unitOfWork.Dashboard.CountPackagesByTypeAsync(PackageType.منزلي);
            distribution[4] = await _unitOfWork.Dashboard.CountPackagesByTypeAsync(PackageType.جلسة);
            model.PackageDistribution = distribution;

            var monthlyCounts = new int[12];
            var monthlyRevs = new decimal[12];
            var yearPackages = await _unitOfWork.Dashboard.GetCurrentYearPackagesAsync(currentYear);

            for (int month = 1; month <= 12; month++)
            {
                var monthPackages = yearPackages.Where(p => p.StartDate.Month == month).ToList();
                monthlyCounts[month - 1] = monthPackages.Count;
                monthlyRevs[month - 1] = monthPackages.Sum(p => p.AmountPaid);
            }
            model.MonthlyDataCounts = monthlyCounts;
            model.MonthlyDataRevenues = monthlyRevs;
            model.CurrentMonthIndex = today.Month - 1;

            var todayPackages = await _unitOfWork.Dashboard.GetTodayPackagesAsync(today);
            var todayChecks = await _unitOfWork.Dashboard.GetTodayChecksAsync(today);
            var newPatientsToday = await _unitOfWork.Dashboard.CountNewPatientsTodayAsync(today);

            model.TodayPackagesSum = todayPackages.Sum(p => p.AmountPaid);
            model.TodayPackagesCount = todayPackages.Count;
            model.TodayChecksCount = todayChecks.Count;
            model.TodayNewPatientsCount = newPatientsToday;
            model.TodayPackagesList = todayPackages;
            model.TodayChecksList = todayChecks;
            model.TodayTotalEarnings = todayPackages.Sum(p => p.AmountPaid);

            model.RecentPatients = await _unitOfWork.Dashboard.GetRecentPatientsAsync();
            model.TodayAppointmentsList = appointmentsToday.OrderBy(a => a.StartTime).ToList();

            model.ActiveDoctors = await _unitOfWork.Dashboard.CountActiveInternDoctorsAsync();
            model.ActiveOrganizations = await _unitOfWork.Dashboard.CountActiveOrganizationsAsync();
            model.ActiveReceptionists = await _unitOfWork.Dashboard.CountActiveReceptionistsAsync();

            var allPackages = await _unitOfWork.Dashboard.GetAllPackagesAsync();
            model.ActivePackagesCountStat = allPackages.Count(p => p.Status == "Active");
            model.ExpiredPackagesCount = allPackages.Count(p => p.Status == "Ended" || string.Equals(p.Status, "Ended", StringComparison.OrdinalIgnoreCase) || p.SessionsCount >= p.NumOfSessions);
            model.RemainingSessionsOverall = allPackages.Sum(p => Math.Max(0, p.NumOfSessions - p.SessionsCount));

            return model;
        }
    }
}
