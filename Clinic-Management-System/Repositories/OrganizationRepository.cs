using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ApplicationDbContext _context;

        public OrganizationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Organization>> GetOrganizationsAsync()
        {
            return await _context.Organizations
                .OrderByDescending(o => o.IsActive)
                .ThenByDescending(o => o.CreateAt)
                .ToListAsync();
        }

        public async Task<Organization?> GetOrganizationDetailsAsync(int? id)
        {
            return await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddOrganizationAsync(Organization organization)
        {
            _context.Add(organization);
            await _context.SaveChangesAsync();
        }

        public async Task<Organization?> FindOrganizationAsync(int id)
        {
            return await _context.Organizations.FindAsync(id);
        }

        public async Task UpdateOrganizationAsync(Organization organization)
        {
            _context.Update(organization);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveOrganizationAsync(Organization organization)
        {
            _context.Organizations.Remove(organization);
            await _context.SaveChangesAsync();
        }

        public bool OrganizationExists(int id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }

        public async Task<Organization?> GetOrganizationForDeleteAsync(int? id)
        {
            return await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}