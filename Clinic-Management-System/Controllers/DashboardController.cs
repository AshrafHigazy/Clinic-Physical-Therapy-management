using Clinic_Management_System.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _dashboardService.GetDashboardMetricsAsync();

            ViewBag.TotalPatients = model.TotalPatients;
            ViewBag.TodayAppointmentsCount = model.TodayAppointmentsCount;
            ViewBag.TodayAttended = model.TodayAttended;
            ViewBag.TodayCanceled = model.TodayCanceled;
            ViewBag.ActivePackagesCount = model.ActivePackagesCount;
            ViewBag.TotalRevenue = model.TotalRevenue;
            ViewBag.WeeklyAppointments = model.WeeklyAppointments;
            ViewBag.PackageDistribution = model.PackageDistribution;
            ViewBag.MonthlyDataCounts = model.MonthlyDataCounts;
            ViewBag.MonthlyDataRevenues = model.MonthlyDataRevenues;
            ViewBag.CurrentMonthIndex = model.CurrentMonthIndex;
            ViewBag.TodayPackagesSum = model.TodayPackagesSum;
            ViewBag.TodayPackagesCount = model.TodayPackagesCount;
            ViewBag.TodayChecksCount = model.TodayChecksCount;
            ViewBag.TodayNewPatientsCount = model.TodayNewPatientsCount;
            ViewBag.TodayPackagesList = model.TodayPackagesList;
            ViewBag.TodayChecksList = model.TodayChecksList;
            ViewBag.TodayTotalEarnings = model.TodayTotalEarnings;
            ViewBag.RecentPatients = model.RecentPatients;
            ViewBag.TodayAppointmentsList = model.TodayAppointmentsList;
            ViewBag.ActiveDoctors = model.ActiveDoctors;
            ViewBag.ActiveOrganizations = model.ActiveOrganizations;
            ViewBag.ActiveReceptionists = model.ActiveReceptionists;
            ViewBag.ActivePackagesCountStat = model.ActivePackagesCountStat;
            ViewBag.ExpiredPackagesCount = model.ExpiredPackagesCount;
            ViewBag.RemainingSessionsOverall = model.RemainingSessionsOverall;

            return View();
        }
    }
}