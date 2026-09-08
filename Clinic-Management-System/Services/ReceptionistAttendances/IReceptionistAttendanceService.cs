using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management_System.Services.ReceptionistAttendances
{
    public interface IReceptionistAttendanceService
    {
        (List<Receptionist> receptionists, List<ReceptionistCurrentShift> currentShifts) GetStartShiftData();
        Task<ServiceResult> StartShiftAsync(int receptionistId);
        Task<ServiceResult> EndShiftAsync(int receptionistId);
        Task<List<ReceptionistAttendance>> GetAttendanceHistoryAsync(int? receptionistId);
        Task<List<ReceptionistAttendance>> GetAllAttendancesAsync();
    }
}
