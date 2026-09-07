using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Clinic_Management_System.Repositories
{
    public class InternDoctorAttendanceRepository : IInternDoctorAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public InternDoctorAttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public System.Collections.Generic.List<InternDoctorAttendance> GetAttendancesByDate(DateTime selectedDate)
        {
            return _context.InternDoctorAttendances
                .Include(a => a.InternDoctor)
                .Where(a => a.Date.Date == selectedDate.Date)
                .ToList();
        }

        public System.Collections.Generic.List<InternDoctor> GetActiveInternDoctors()
        {
            return _context.InternDoctors
                .Where(d => d.IsActive)
                .ToList();
        }

        public InternDoctor? GetActiveInternDoctor(int id)
        {
            return _context.InternDoctors
                .FirstOrDefault(d => d.InternDoctorId == id && d.IsActive);
        }

        public bool AttendanceExists(int doctorId, DateTime date)
        {
            return _context.InternDoctorAttendances
                .Any(a => a.InternDoctorId == doctorId && a.Date.Date == date.Date);
        }

        public void AddAttendance(InternDoctorAttendance attendance)
        {
            _context.InternDoctorAttendances.Add(attendance);
            _context.SaveChanges();
        }

        public InternDoctor? GetInternDoctorWithAttendances(int id)
        {
            return _context.InternDoctors
                .Include(d => d.Attendances)
                .FirstOrDefault(d => d.InternDoctorId == id);
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