using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class InternDoctorRepository : Repository<InternDoctor>, IInternDoctorRepository
    {
        public InternDoctorRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<InternDoctor>> GetDoctorsAsync(string? search)
        {
            var doctors = from d in _context.InternDoctors select d;

            if (!string.IsNullOrEmpty(search))
                doctors = doctors.Where(d => d.FullName.Contains(search) || d.Phone.Contains(search));

            doctors = doctors.OrderByDescending(d => d.IsActive).ThenBy(d => d.FullName);

            return await doctors.ToListAsync();
        }

        public void AddDoctor(InternDoctor doctor) => Add(doctor);

        public InternDoctor? GetDoctorWithAttendances(int id)
        {
            return _context.InternDoctors
                .Include(d => d.Attendances)
                .FirstOrDefault(m => m.InternDoctorId == id);
        }

        public async Task<InternDoctor?> GetDoctorByFilterAsync(int id)
            => await _context.InternDoctors.FirstOrDefaultAsync(d => d.InternDoctorId == id);

        public async Task<InternDoctor?> FindDoctorAsync(int id)
            => await _context.InternDoctors.FindAsync(id);

        public void UpdateDoctor(InternDoctor doctor) => Update(doctor);

        public void RemoveDoctor(InternDoctor doctor) => Remove(doctor);

        public InternDoctor? GetActiveInternDoctor(int id)
            => _context.InternDoctors.FirstOrDefault(d => d.InternDoctorId == id && d.IsActive);

        public InternDoctorAttendance? GetTodayAttendance(int doctorId, DateTime today, DateTime tomorrow)
        {
            return _context.InternDoctorAttendances
                .FirstOrDefault(a => a.InternDoctorId == doctorId && a.Date >= today && a.Date < tomorrow);
        }
    }
}