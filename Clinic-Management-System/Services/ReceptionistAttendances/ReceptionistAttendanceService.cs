using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Clinic_Management_System.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.ReceptionistAttendances
{
    public class ReceptionistAttendanceService : IReceptionistAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReceptionistAttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public (List<Receptionist> receptionists, List<ReceptionistCurrentShift> currentShifts) GetStartShiftData()
        {
            var receptionists = _unitOfWork.ReceptionistAttendances.GetReceptionistsForSelect();
            var currentShifts = _unitOfWork.ReceptionistAttendances.GetCurrentShifts();
            return (receptionists, currentShifts);
        }

        public async Task<ServiceResult> StartShiftAsync(int receptionistId)
        {
            if (receptionistId == 0)
            {
                return ServiceResult.Fail("معرف الموظف غير صالح.");
            }

            var existingShift = await _unitOfWork.ReceptionistAttendances.GetCurrentShiftByReceptionistAsync(receptionistId);
            if (existingShift != null)
            {
                return ServiceResult.Fail("❌ هذا الموظف بدأ عمله بالفعل.");
            }

            var today = DateTime.Today;
            var attendanceToday = await _unitOfWork.ReceptionistAttendances.GetAttendanceByReceptionistAndDateAsync(receptionistId, today);
            if (attendanceToday != null)
            {
                return ServiceResult.Fail("✅ هذا الموظف أنهى عمله اليوم.");
            }

            var shift = new ReceptionistCurrentShift
            {
                ReceptionistId = receptionistId,
                StartTime = DateTime.Now
            };

            _unitOfWork.ReceptionistAttendances.AddCurrentShift(shift);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok("✅ تم تسجيل بداية العمل بنجاح.");
        }

        public async Task<ServiceResult> EndShiftAsync(int receptionistId)
        {
            var shift = await _unitOfWork.ReceptionistAttendances.GetCurrentShiftWithReceptionistAsync(receptionistId);
            if (shift == null)
            {
                return ServiceResult.Fail("❌ هذا الموظف لم يبدأ عمله بعد.");
            }

            var today = DateTime.Today;
            var alreadyEnded = await _unitOfWork.ReceptionistAttendances.AttendanceAlreadyEndedAsync(receptionistId, today);
            if (alreadyEnded)
            {
                return ServiceResult.Fail("✅ هذا الموظف أنهى عمله اليوم بالفعل.");
            }

            var attendance = new ReceptionistAttendance
            {
                ReceptionistId = shift.ReceptionistId,
                Date = today,
                CheckIn = shift.StartTime,
                CheckOut = DateTime.Now,
                Hours = (int)(DateTime.Now - shift.StartTime).TotalHours
            };

            _unitOfWork.ReceptionistAttendances.AddAttendanceAndRemoveShift(attendance, shift);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Ok("✅ تم تسجيل إنهاء العمل بنجاح.");
        }

        public async Task<List<ReceptionistAttendance>> GetAttendanceHistoryAsync(int? receptionistId)
        {
            return await _unitOfWork.ReceptionistAttendances.GetAttendanceHistoryAsync(receptionistId);
        }

        public async Task<List<ReceptionistAttendance>> GetAllAttendancesAsync()
        {
            return await _unitOfWork.ReceptionistAttendances.GetAllAttendancesAsync();
        }
    }
}
