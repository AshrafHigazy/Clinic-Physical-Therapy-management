using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class ReceptionistRepository : Repository<Receptionist>, IReceptionistRepository
    {
        public ReceptionistRepository(ApplicationDbContext context) : base(context) { }

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

        public void AddReceptionist(Receptionist receptionist) => Add(receptionist);

        public async Task<Receptionist?> FindReceptionistAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public void UpdateReceptionist(Receptionist receptionist) => Update(receptionist);

        public bool ReceptionistExists(int id) => Exists(e => e.Id == id);

        public void RemoveReceptionist(Receptionist receptionist) => Remove(receptionist);
    }
}