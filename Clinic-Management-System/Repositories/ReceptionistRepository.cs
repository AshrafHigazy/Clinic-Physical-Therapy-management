using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class ReceptionistRepository : IReceptionistRepository
    {
        private readonly ApplicationDbContext _context;

        public ReceptionistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Receptionist>> GetReceptionistsAsync()
        {
            return await _context.Receptionist
                .OrderByDescending(r => r.IsActive)
                .ThenByDescending(r => r.HiringDate)
                .ToListAsync();
        }

        public async Task<Receptionist?> GetReceptionistByIdAsync(int? id)
        {
            return await _context.Receptionist
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddReceptionistAsync(Receptionist receptionist)
        {
            _context.Add(receptionist);
            await _context.SaveChangesAsync();
        }

        public async Task<Receptionist?> FindReceptionistAsync(int id)
        {
            return await _context.Receptionist.FindAsync(id);
        }

        public async Task UpdateReceptionistAsync(Receptionist receptionist)
        {
            _context.Update(receptionist);
            await _context.SaveChangesAsync();
        }

        public bool ReceptionistExists(int id)
        {
            return _context.Receptionist.Any(e => e.Id == id);
        }

        public async Task RemoveReceptionistAsync(Receptionist receptionist)
        {
            _context.Receptionist.Remove(receptionist);
            await _context.SaveChangesAsync();
        }
    }
}