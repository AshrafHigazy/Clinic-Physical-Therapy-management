using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class InternDoctorRepository : IInternDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public InternDoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<InternDoctor>> GetDoctorsAsync(string? search)
        {
            var doctors = from d in _context.InternDoctors
                          select d;

            if (!string.IsNullOrEmpty(search))
                doctors = doctors.Where(d => d.FullName.Contains(search) || d.Phone.Contains(search));

            doctors = doctors.OrderByDescending(d => d.IsActive).ThenBy(d => d.FullName);

            return await doctors.ToListAsync();
        }

        public async Task AddDoctorAsync(InternDoctor doctor)
        {
            _context.Add(doctor);
            await _context.SaveChangesAsync();
        }

        public InternDoctor? GetDoctorWithAttendances(int id)
        {
            return _context.InternDoctors
                .Include(d => d.Attendances)
                .FirstOrDefault(m => m.InternDoctorId == id);
        }

        public async Task<InternDoctor?> GetDoctorByFilterAsync(int id)
        {
            return await _context.InternDoctors
                .FirstOrDefaultAsync(d => d.InternDoctorId == id);
        }

        public async Task<InternDoctor?> FindDoctorAsync(int id)
        {
            return await _context.InternDoctors.FindAsync(id);
        }

        public async Task UpdateDoctorAsync(InternDoctor doctor)
        {
            _context.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDoctorAsync(InternDoctor doctor)
        {
            _context.InternDoctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }

        public InternDoctor? GetActiveInternDoctor(int id)
        {
            return _context.InternDoctors
                .FirstOrDefault(d => d.InternDoctorId == id && d.IsActive);
        }

        public InternDoctorAttendance? GetTodayAttendance(int doctorId, DateTime today, DateTime tomorrow)
        {
            return _context.InternDoctorAttendances
                .FirstOrDefault(a => a.InternDoctorId == doctorId && a.Date >= today && a.Date < tomorrow);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}