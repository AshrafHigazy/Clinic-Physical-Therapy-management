using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Clinic_Management_System.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountPatientsAsync()
        {
            return await _context.Patient.CountAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsForDateAsync(DateTime today)
        {
            var startOfDay = today.Date;
            var endOfDay = startOfDay.AddDays(1);
            return await _context.Appointment
                .Include(a => a.Patient)
                .Include(a => a.Receptionist)
                .Where(a => a.StartTime >= startOfDay && a.StartTime < endOfDay)
                .ToListAsync();
        }

        public async Task<List<Package>> GetActivePackagesAsync()
        {
            return await _context.Packages.Where(p => p.Status == "Active").ToListAsync();
        }

        public async Task<decimal> SumPackagesAmountPaidAsync()
        {
            return await _context.Packages.SumAsync(p => p.AmountPaid);
        }

        public async Task<int> CountAppointmentsOnDateAsync(DateTime targetDate)
        {
            var start = targetDate.Date;
            var end = start.AddDays(1);
            return await _context.Appointment
                .Where(a => a.StartTime >= start && a.StartTime < end)
                .CountAsync();
        }

        public async Task<int> CountPackagesByTypeAsync(PackageType type)
        {
            return await _context.Packages.CountAsync(p => p.Type == type);
        }

        public async Task<List<Package>> GetCurrentYearPackagesAsync(int currentYear)
        {
            return await _context.Packages
                .Where(p => p.StartDate.Year == currentYear)
                .ToListAsync();
        }

        public async Task<List<Package>> GetTodayPackagesAsync(DateTime today)
        {
            var start = today.Date;
            var end = start.AddDays(1);
            return await _context.Packages
                .Include(p => p.Patient)
                .Include(p => p.Check)
                .Where(p => p.StartDate >= start && p.StartDate < end)
                .ToListAsync();
        }

        public async Task<List<Check>> GetTodayChecksAsync(DateTime today)
        {
            var start = today.Date;
            var end = start.AddDays(1);
            return await _context.Checks
                .Include(c => c.Patient)
                .Where(c => c.CreatedAt >= start && c.CreatedAt < end)
                .ToListAsync();
        }

        public async Task<int> CountNewPatientsTodayAsync(DateTime today)
        {
            var start = today.Date;
            var end = start.AddDays(1);
            return await _context.Patient
                .Where(p => p.CreateAt >= start && p.CreateAt < end)
                .CountAsync();
        }

        public async Task<List<Patient>> GetRecentPatientsAsync()
        {
            return await _context.Patient
                .OrderByDescending(p => p.CreateAt)
                .Take(10)
                .ToListAsync();
        }

        public async Task<int> CountActiveInternDoctorsAsync()
        {
            return await _context.InternDoctors.CountAsync(d => d.IsActive);
        }

        public async Task<int> CountActiveOrganizationsAsync()
        {
            return await _context.Organizations.CountAsync(o => o.IsActive);
        }

        public async Task<int> CountActiveReceptionistsAsync()
        {
            return await _context.Receptionist.CountAsync(r => r.IsActive);
        }

        public async Task<List<Package>> GetAllPackagesAsync()
        {
            return await _context.Packages.ToListAsync();
        }
    }
}