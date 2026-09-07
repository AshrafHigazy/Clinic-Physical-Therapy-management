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

        public void AddReceptionist(Receptionist receptionist)
        {
            _context.Add(receptionist);
        }

        public async Task<Receptionist?> FindReceptionistAsync(int id)
        {
            return await _context.Receptionist.FindAsync(id);
        }

        public void UpdateReceptionist(Receptionist receptionist)
        {
            _context.Update(receptionist);
        }

        public bool ReceptionistExists(int id)
        {
            return _context.Receptionist.Any(e => e.Id == id);
        }

        public void RemoveReceptionist(Receptionist receptionist)
        {
            _context.Receptionist.Remove(receptionist);
        }
    }
}