using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class CheckRepository : Repository<Check>, ICheckRepository
    {
        public CheckRepository(ApplicationDbContext context) : base(context) { }

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
            => _context.Patient.FirstOrDefault(p => p.Id == id);

        public List<Patient> GetPatientsForSelect()
            => _context.Patient.ToList();

        public void AddCheck(Check check) => Add(check);

        public async Task<Check?> GetCheckForEditAsync(int? id)
        {
            return await _context.Checks
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(c => c.CheckId == id);
        }

        public void UpdateCheck(Check check) => Update(check);

        public bool CheckExists(int id) => Exists(e => e.CheckId == id);

        public async Task<Check?> FindCheckAsync(int id)
            => await _context.Checks.FindAsync(id);

        public void RemoveCheck(Check check) => Remove(check);
    }
}