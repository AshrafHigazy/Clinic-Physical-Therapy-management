using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Clinic_Management_System.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Saturday)) % 7;
            var saturday = today.AddDays(-1 * diff).Date;
            var currentYear = today.Year;

            ViewBag.TotalPatients = await _context.Patient.CountAsync();

            var appointmentsToday = await _context.Appointment
                .Include(a => a.Patient)
                .Include(a => a.Receptionist)
                .Where(a => a.StartTime.Date == today)
                .ToListAsync();

            ViewBag.TodayAppointmentsCount = appointmentsToday.Count;
            ViewBag.TodayAttended = appointmentsToday.Count(a => a.IsAttended);
            ViewBag.TodayCanceled = appointmentsToday.Count(a => a.IsCanceled);

            var activePackages = await _context.Packages.Where(p => p.Status == "Active").ToListAsync();
            ViewBag.ActivePackagesCount = activePackages.Count;
            ViewBag.TotalRevenue = await _context.Packages.SumAsync(p => p.AmountPaid);

            var weeklyAppointments = new int[7];
            for (int i = 0; i < 7; i++)
            {
                var targetDate = saturday.AddDays(i);
                weeklyAppointments[i] = await _context.Appointment
                    .Where(a => a.StartTime.Date == targetDate.Date)
                    .CountAsync();
            }
            ViewBag.WeeklyAppointments = weeklyAppointments;

            var distribution = new int[5];
            distribution[0] = await _context.Packages.CountAsync(p => p.Type == PackageType.ذهبي);
            distribution[1] = await _context.Packages.CountAsync(p => p.Type == PackageType.فضي);
            distribution[2] = await _context.Packages.CountAsync(p => p.Type == PackageType.الماسي);
            distribution[3] = await _context.Packages.CountAsync(p => p.Type == PackageType.منزلي);
            distribution[4] = await _context.Packages.CountAsync(p => p.Type == PackageType.جلسة);
            ViewBag.PackageDistribution = distribution;

            var monthlyData = new { Counts = new int[12], Revenues = new decimal[12] };
            var yearPackages = await _context.Packages
                .Where(p => p.StartDate.Year == currentYear)
                .ToListAsync();

            for (int month = 1; month <= 12; month++)
            {
                var monthPackages = yearPackages.Where(p => p.StartDate.Month == month).ToList();
                monthlyData.Counts[month - 1] = monthPackages.Count;
                monthlyData.Revenues[month - 1] = monthPackages.Sum(p => p.AmountPaid);
            }
            ViewBag.MonthlyDataCounts = monthlyData.Counts;
            ViewBag.MonthlyDataRevenues = monthlyData.Revenues;
            ViewBag.CurrentMonthIndex = today.Month - 1;

            var todayPackages = await _context.Packages
                .Include(p => p.Patient)
                .Include(p => p.Check)
                .Where(p => p.StartDate.Date == today)
                .ToListAsync();
            var todayChecks = await _context.Checks
                .Include(c => c.Patient)
                .Where(c => c.CreatedAt.Date == today)
                .ToListAsync();
            var newPatientsToday = await _context.Patient
                .Where(p => p.CreateAt.Date == today)
                .CountAsync();

            ViewBag.TodayPackagesSum = todayPackages.Sum(p => p.AmountPaid);
            ViewBag.TodayPackagesCount = todayPackages.Count;
            ViewBag.TodayChecksCount = todayChecks.Count;
            ViewBag.TodayNewPatientsCount = newPatientsToday;
            ViewBag.TodayPackagesList = todayPackages;
            ViewBag.TodayChecksList = todayChecks;
            ViewBag.TodayTotalEarnings = todayPackages.Sum(p => p.AmountPaid);

            ViewBag.RecentPatients = await _context.Patient
                .OrderByDescending(p => p.CreateAt)
                .Take(10)
                .ToListAsync();
            ViewBag.TodayAppointmentsList = appointmentsToday.OrderBy(a => a.StartTime).ToList();

            ViewBag.ActiveDoctors = await _context.InternDoctors.CountAsync(d => d.IsActive);
            ViewBag.ActiveOrganizations = await _context.Organizations.CountAsync(o => o.IsActive);
            ViewBag.ActiveReceptionists = await _context.Receptionist.CountAsync(r => r.IsActive);

            var allPackages = await _context.Packages.ToListAsync();
            ViewBag.ActivePackagesCountStat = allPackages.Count(p => p.Status == "Active");
            ViewBag.ExpiredPackagesCount = allPackages.Count(p => p.Status == "Ended" || string.Equals(p.Status, "Ended", StringComparison.OrdinalIgnoreCase) || p.SessionsCount >= p.NumOfSessions);
            ViewBag.RemainingSessionsOverall = allPackages.Sum(p => Math.Max(0, p.NumOfSessions - p.SessionsCount));

            return View();
        }
    }
}
