using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class ReceptionistAttendanceRepository : Repository<ReceptionistAttendance>, IReceptionistAttendanceRepository
    {
        public ReceptionistAttendanceRepository(ApplicationDbContext context) : base(context) { }

        public List<Receptionist> GetReceptionistsForSelect()
        {
            return _context.Receptionist.ToList();
        }

        public List<ReceptionistCurrentShift> GetCurrentShifts()
        {
            return _context.ReceptionistCurrentShifts
                .Include(c => c.Receptionist)
                .ToList();
        }

        public async Task<ReceptionistCurrentShift?> GetCurrentShiftByReceptionistAsync(int receptionistId)
        {
            return await _context.ReceptionistCurrentShifts
                .FirstOrDefaultAsync(c => c.ReceptionistId == receptionistId);
        }

        public async Task<ReceptionistAttendance?> GetAttendanceByReceptionistAndDateAsync(int receptionistId, DateTime date)
        {
            return await _context.ReceptionistAttendance
                .FirstOrDefaultAsync(a => a.ReceptionistId == receptionistId && a.Date == date);
        }

        public void AddCurrentShift(ReceptionistCurrentShift shift)
        {
            _context.ReceptionistCurrentShifts.Add(shift);
        }

        public async Task<ReceptionistCurrentShift?> GetCurrentShiftWithReceptionistAsync(int receptionistId)
        {
            return await _context.ReceptionistCurrentShifts
                .Include(c => c.Receptionist)
                .FirstOrDefaultAsync(c => c.ReceptionistId == receptionistId);
        }

        public async Task<bool> AttendanceAlreadyEndedAsync(int receptionistId, DateTime date)
        {
            return await _context.ReceptionistAttendance
                .AnyAsync(a => a.ReceptionistId == receptionistId && a.Date == date);
        }

        public void AddAttendanceAndRemoveShift(ReceptionistAttendance attendance, ReceptionistCurrentShift shift)
        {
            Add(attendance);
            _context.ReceptionistCurrentShifts.Remove(shift);
        }

        public async Task<List<ReceptionistAttendance>> GetAttendanceHistoryAsync(int? receptionistId)
        {
            return await _context.ReceptionistAttendance
                .Where(a => a.ReceptionistId == receptionistId)
                .Include(a => a.Receptionist)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<List<ReceptionistAttendance>> GetAllAttendancesAsync()
        {
            var attendances = _context.ReceptionistAttendance
                .Include(r => r.Receptionist)
                .OrderByDescending(a => a.Date);

            return await attendances.ToListAsync();
        }
    }
}