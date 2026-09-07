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

        public void AddOrganization(Organization organization)
        {
            _context.Add(organization);
        }

        public async Task<Organization?> FindOrganizationAsync(int id)
        {
            return await _context.Organizations.FindAsync(id);
        }

        public void UpdateOrganization(Organization organization)
        {
            _context.Update(organization);
        }

        public void RemoveOrganization(Organization organization)
        {
            _context.Organizations.Remove(organization);
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