using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Clinic_Management_System.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.Appointments
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        private const int OpeningHour = 8;
        private const int LastStartHour = 23;
        private const int MaxPerHour = 4;
        private static readonly TimeSpan AppointmentDuration = TimeSpan.FromHours(1);

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AppointmentCalendarDto>> GetAllAppointmentsAsync()
        {
            return await _unitOfWork.Appointments.GetAllAppointmentsAsync();
        }

        public async Task<(List<Receptionist> receptionists, string formattedDate)> GetConfirmBookingDataAsync(int patientId, DateTime date)
        {
            var receptionists = await _unitOfWork.Appointments.GetReceptionistsAsync();
            var formattedDate = date.ToString("yyyy-MM-ddTHH:mm");
            return (receptionists, formattedDate);
        }

        public async Task<ServiceResult> CreateAppointmentAsync(int patientId, string start, int receptionistId)
        {
            string[] formats = {
                "yyyy-MM-ddTHH:mm",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd"
            };

            if (!DateTime.TryParseExact(
                start,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime startTime))
            {
                if (!DateTime.TryParse(start, CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime) &&
                    !DateTime.TryParse(start, out startTime))
                {
                    return ServiceResult.Fail("❌ صيغة التاريخ غير صحيحة.");
                }
            }

            Receptionist? receptionist = null;
            if (receptionistId > 0)
            {
                receptionist = await _unitOfWork.Appointments.FindReceptionistAsync(receptionistId);
            }

            if (receptionist == null)
            {
                var allReceptionists = await _unitOfWork.Appointments.GetReceptionistsAsync();
                receptionist = allReceptionists.FirstOrDefault();
                if (receptionist != null)
                {
                    receptionistId = receptionist.Id;
                }
                else
                {
                    return ServiceResult.Fail("❌ لا يوجد موظف استقبال مسجل في النظام.");
                }
            }

            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 0);
            DateTime endTime = startTime.Add(AppointmentDuration);

            if (startTime.Hour < OpeningHour || startTime.Hour > LastStartHour)
            {
                return ServiceResult.Fail($"⚠ ساعات العمل من {OpeningHour}:00 إلى {LastStartHour}:00");
            }

            bool patientOverlap = await _unitOfWork.Appointments.PatientHasOverlappingAppointmentAsync(patientId, startTime, endTime);
            if (patientOverlap)
            {
                return ServiceResult.Fail("⚠ المريض لديه موعد متداخل في نفس الفترة.");
            }

            int overlappingCount = await _unitOfWork.Appointments.CountOverlappingAppointmentsAsync(startTime, endTime);
            if (overlappingCount >= MaxPerHour)
            {
                return ServiceResult.Fail("⚠ هذا التوقيت ممتلئ بالفعل (4 مرضى كحد أقصى).");
            }

            var appointment = new Appointment
            {
                PatientId = patientId,
                ReceptionistId = receptionistId,
                StartTime = startTime,
                EndTime = endTime,
                IsWithMainDoctor = true
            };

            _unitOfWork.Appointments.AddAppointment(appointment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok("✅ تم حجز الموعد بنجاح.");
        }

        public async Task<Appointment?> GetAppointmentDetailsAsync(int id)
        {
            return await _unitOfWork.Appointments.GetAppointmentDetailsAsync(id);
        }

        public async Task<Appointment?> FindAppointmentAsync(int id)
        {
            return await _unitOfWork.Appointments.FindAppointmentAsync(id);
        }

        public async Task<ServiceResult> CancelAppointmentAsync(int id)
        {
            var appt = await _unitOfWork.Appointments.FindAppointmentAsync(id);
            if (appt == null)
                return ServiceResult.Fail("الموعد غير موجود.");

            appt.IsCanceled = true;
            appt.IsAttended = false;
            _unitOfWork.Appointments.UpdateAppointment(appt);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> UpdateAppointmentStatusAsync(int id, bool isAttended, bool isCanceled)
        {
            var appt = await _unitOfWork.Appointments.FindAppointmentAsync(id);
            if (appt == null)
                return ServiceResult.Fail("الموعد غير موجود.");

            appt.IsAttended = isAttended;
            appt.IsCanceled = isCanceled;
            _unitOfWork.Appointments.UpdateAppointment(appt);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAppointmentAsync(int id)
        {
            var appt = await _unitOfWork.Appointments.FindAppointmentAsync(id);
            if (appt == null)
                return ServiceResult.Fail("الموعد غير موجود.");

            _unitOfWork.Appointments.RemoveAppointment(appt);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok();
        }
    }
}
