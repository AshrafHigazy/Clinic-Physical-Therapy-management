using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Appointments
{
    public interface IAppointmentService
    {
        Task<List<AppointmentCalendarDto>> GetAllAppointmentsAsync();
        Task<(List<Receptionist> receptionists, string formattedDate)> GetConfirmBookingDataAsync(int patientId, DateTime date);
        Task<ServiceResult> CreateAppointmentAsync(int patientId, string start, int receptionistId);
        Task<Appointment?> GetAppointmentDetailsAsync(int id);
        Task<Appointment?> FindAppointmentAsync(int id);
        Task<ServiceResult> CancelAppointmentAsync(int id);
        Task<ServiceResult> UpdateAppointmentStatusAsync(int id, bool isAttended, bool isCanceled);
        Task<ServiceResult> DeleteAppointmentAsync(int id);
    }
}
