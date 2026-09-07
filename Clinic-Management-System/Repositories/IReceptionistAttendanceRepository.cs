using Clinic_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public interface IReceptionistAttendanceRepository
    {
        List<Receptionist> GetReceptionistsForSelect();
        List<ReceptionistCurrentShift> GetCurrentShifts();
        Task<ReceptionistCurrentShift?> GetCurrentShiftByReceptionistAsync(int receptionistId);
        Task<ReceptionistAttendance?> GetAttendanceByReceptionistAndDateAsync(int receptionistId, DateTime date);
        Task AddCurrentShiftAsync(ReceptionistCurrentShift shift);
        Task<ReceptionistCurrentShift?> GetCurrentShiftWithReceptionistAsync(int receptionistId);
        Task<bool> AttendanceAlreadyEndedAsync(int receptionistId, DateTime date);
        Task AddAttendanceAndRemoveShiftAsync(ReceptionistAttendance attendance, ReceptionistCurrentShift shift);
        Task<List<ReceptionistAttendance>> GetAttendanceHistoryAsync(int? receptionistId);
        Task<List<ReceptionistAttendance>> GetAllAttendancesAsync();
    }
}