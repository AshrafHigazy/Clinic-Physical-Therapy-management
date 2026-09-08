using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<AppointmentCalendarDto>> GetAllAppointmentsAsync()
        {
            return await _context.Appointment
                .Include(a => a.Patient)
                .Include(a => a.Receptionist)
                .Select(a => new AppointmentCalendarDto
                {
                    id = a.Id,
                    title = a.Patient.FullName,
                    start = a.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = a.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    isAttended = a.IsAttended,
                    isCanceled = a.IsCanceled,
                    receptionist = a.Receptionist != null ? (a.Receptionist.FullName ?? a.Receptionist.Id.ToString()) : "غير محدد",
                    description = a.IsCanceled ? " غاب" : (a.IsAttended ? " حضر" : " لم يتأكد"),
                    color = a.IsCanceled ? "#dc3545" : "#28a745"
                })
                .ToListAsync();
        }

        public async Task<List<Receptionist>> GetReceptionistsAsync()
        {
            return await _context.Receptionist
                .OrderBy(r => r.FullName)
                .ToListAsync();
        }

        public async Task<Receptionist?> FindReceptionistAsync(int id)
            => await _context.Receptionist.FindAsync(id);

        public async Task<bool> PatientHasOverlappingAppointmentAsync(int patientId, System.DateTime startTime, System.DateTime endTime)
        {
            return await _context.Appointment.AnyAsync(a =>
                a.PatientId == patientId &&
                a.StartTime < endTime &&
                a.EndTime > startTime &&
                !a.IsCanceled
            );
        }

        public async Task<int> CountOverlappingAppointmentsAsync(System.DateTime startTime, System.DateTime endTime)
        {
            return await _context.Appointment.CountAsync(a =>
                a.StartTime < endTime &&
                a.EndTime > startTime &&
                !a.IsCanceled
            );
        }

        public void AddAppointment(Appointment appointment) => Add(appointment);

        public async Task<Appointment?> GetAppointmentDetailsAsync(int id)
        {
            return await _context.Appointment
                .Include(a => a.Patient)
                .Include(a => a.Receptionist)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Appointment?> FindAppointmentAsync(int id)
            => await GetByIdAsync(id);

        public void UpdateAppointment(Appointment appointment) => Update(appointment);

        public void RemoveAppointment(Appointment appointment) => Remove(appointment);
    }
}