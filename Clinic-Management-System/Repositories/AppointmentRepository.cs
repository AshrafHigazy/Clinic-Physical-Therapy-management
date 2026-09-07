using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AppointmentCalendarDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _context.Appointment
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

            return appointments;
        }

        public async Task<List<Receptionist>> GetReceptionistsAsync()
        {
            return await _context.Receptionist
                .OrderBy(r => r.FullName)
                .ToListAsync();
        }

        public async Task<Receptionist?> FindReceptionistAsync(int id)
        {
            return await _context.Receptionist.FindAsync(id);
        }

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

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            _context.Appointment.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<Appointment?> GetAppointmentDetailsAsync(int id)
        {
            return await _context.Appointment
                .Include(a => a.Patient)
                .Include(a => a.Receptionist)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Appointment?> FindAppointmentAsync(int id)
        {
            return await _context.Appointment.FindAsync(id);
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointment.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAppointmentAsync(Appointment appointment)
        {
            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }
}