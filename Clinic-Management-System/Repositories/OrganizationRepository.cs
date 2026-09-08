using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(ApplicationDbContext context) : base(context) { }

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

        public void AddOrganization(Organization organization) => Add(organization);

        public async Task<Organization?> FindOrganizationAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public void UpdateOrganization(Organization organization) => Update(organization);

        public void RemoveOrganization(Organization organization) => Remove(organization);

        public bool OrganizationExists(int id) => Exists(e => e.Id == id);

        public async Task<Organization?> GetOrganizationForDeleteAsync(int? id)
        {
            return await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}