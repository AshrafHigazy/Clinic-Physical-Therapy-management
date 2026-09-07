using Clinic_Management_System.Models;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IAppointmentRepository
    {
        Task<List<AppointmentCalendarDto>> GetAllAppointmentsAsync();
        Task<List<Receptionist>> GetReceptionistsAsync();
        Task<Receptionist?> FindReceptionistAsync(int id);
        Task<bool> PatientHasOverlappingAppointmentAsync(int patientId, System.DateTime startTime, System.DateTime endTime);
        Task<int> CountOverlappingAppointmentsAsync(System.DateTime startTime, System.DateTime endTime);
        Task AddAppointmentAsync(Appointment appointment);
        Task<Appointment?> GetAppointmentDetailsAsync(int id);
        Task<Appointment?> FindAppointmentAsync(int id);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task RemoveAppointmentAsync(Appointment appointment);
    }
}