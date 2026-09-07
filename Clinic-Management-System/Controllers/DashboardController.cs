using Clinic_Management_System.Models.Enums;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor")]
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Saturday)) % 7;
            var saturday = today.AddDays(-1 * diff).Date;
            var currentYear = today.Year;

            ViewBag.TotalPatients = await _dashboardRepository.CountPatientsAsync();

            var appointmentsToday = await _dashboardRepository.GetAppointmentsForDateAsync(today);

            ViewBag.TodayAppointmentsCount = appointmentsToday.Count;
            ViewBag.TodayAttended = appointmentsToday.Count(a => a.IsAttended);
            ViewBag.TodayCanceled = appointmentsToday.Count(a => a.IsCanceled);

            var activePackages = await _dashboardRepository.GetActivePackagesAsync();
            ViewBag.ActivePackagesCount = activePackages.Count;
            ViewBag.TotalRevenue = await _dashboardRepository.SumPackagesAmountPaidAsync();

            var weeklyAppointments = new int[7];
            for (int i = 0; i < 7; i++)
            {
                var targetDate = saturday.AddDays(i);
                weeklyAppointments[i] = await _dashboardRepository.CountAppointmentsOnDateAsync(targetDate);
            }
            ViewBag.WeeklyAppointments = weeklyAppointments;

            var distribution = new int[5];
            distribution[0] = await _dashboardRepository.CountPackagesByTypeAsync(PackageType.ذهبي);
            distribution[1] = await _dashboardRepository.CountPackagesByTypeAsync(PackageType.فضي);
            distribution[2] = await _dashboardRepository.CountPackagesByTypeAsync(PackageType.الماسي);
            distribution[3] = await _dashboardRepository.CountPackagesByTypeAsync(PackageType.منزلي);
            distribution[4] = await _dashboardRepository.CountPackagesByTypeAsync(PackageType.جلسة);
            ViewBag.PackageDistribution = distribution;

            var monthlyData = new { Counts = new int[12], Revenues = new decimal[12] };
            var yearPackages = await _dashboardRepository.GetCurrentYearPackagesAsync(currentYear);

            for (int month = 1; month <= 12; month++)
            {
                var monthPackages = yearPackages.Where(p => p.StartDate.Month == month).ToList();
                monthlyData.Counts[month - 1] = monthPackages.Count;
                monthlyData.Revenues[month - 1] = monthPackages.Sum(p => p.AmountPaid);
            }
            ViewBag.MonthlyDataCounts = monthlyData.Counts;
            ViewBag.MonthlyDataRevenues = monthlyData.Revenues;
            ViewBag.CurrentMonthIndex = today.Month - 1;

            var todayPackages = await _dashboardRepository.GetTodayPackagesAsync(today);
            var todayChecks = await _dashboardRepository.GetTodayChecksAsync(today);
            var newPatientsToday = await _dashboardRepository.CountNewPatientsTodayAsync(today);

            ViewBag.TodayPackagesSum = todayPackages.Sum(p => p.AmountPaid);
            ViewBag.TodayPackagesCount = todayPackages.Count;
            ViewBag.TodayChecksCount = todayChecks.Count;
            ViewBag.TodayNewPatientsCount = newPatientsToday;
            ViewBag.TodayPackagesList = todayPackages;
            ViewBag.TodayChecksList = todayChecks;
            ViewBag.TodayTotalEarnings = todayPackages.Sum(p => p.AmountPaid);

            ViewBag.RecentPatients = await _dashboardRepository.GetRecentPatientsAsync();
            ViewBag.TodayAppointmentsList = appointmentsToday.OrderBy(a => a.StartTime).ToList();

            ViewBag.ActiveDoctors = await _dashboardRepository.CountActiveInternDoctorsAsync();
            ViewBag.ActiveOrganizations = await _dashboardRepository.CountActiveOrganizationsAsync();
            ViewBag.ActiveReceptionists = await _dashboardRepository.CountActiveReceptionistsAsync();

            var allPackages = await _dashboardRepository.GetAllPackagesAsync();
            ViewBag.ActivePackagesCountStat = allPackages.Count(p => p.Status == "Active");
            ViewBag.ExpiredPackagesCount = allPackages.Count(p => p.Status == "Ended" || string.Equals(p.Status, "Ended", StringComparison.OrdinalIgnoreCase) || p.SessionsCount >= p.NumOfSessions);
            ViewBag.RemainingSessionsOverall = allPackages.Sum(p => Math.Max(0, p.NumOfSessions - p.SessionsCount));

            return View();
        }
    }
}