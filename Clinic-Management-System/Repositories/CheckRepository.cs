using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class CheckRepository : ICheckRepository
    {
        private readonly ApplicationDbContext _context;

        public CheckRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Check>> GetChecksAsync()
        {
            return await _context.Checks
                .Include(c => c.Patient)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Check?> GetCheckDetailsAsync(int? id)
        {
            return await _context.Checks
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(c => c.CheckId == id);
        }

        public Patient? GetPatientById(int id)
        {
            return _context.Patient.FirstOrDefault(p => p.Id == id);
        }

        public List<Patient> GetPatientsForSelect()
        {
            return _context.Patient.ToList();
        }

        public async Task AddCheckAsync(Check check)
        {
            _context.Checks.Add(check);
            await _context.SaveChangesAsync();
        }

        public async Task<Check?> GetCheckForEditAsync(int? id)
        {
            return await _context.Checks
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(c => c.CheckId == id);
        }

        public async Task UpdateCheckAsync(Check check)
        {
            _context.Update(check);
            await _context.SaveChangesAsync();
        }

        public bool CheckExists(int id)
        {
            return _context.Checks.Any(e => e.CheckId == id);
        }

        public async Task<Check?> FindCheckAsync(int id)
        {
            return await _context.Checks.FindAsync(id);
        }

        public async Task RemoveCheckAsync(Check check)
        {
            _context.Checks.Remove(check);
            await _context.SaveChangesAsync();
        }
    }
}