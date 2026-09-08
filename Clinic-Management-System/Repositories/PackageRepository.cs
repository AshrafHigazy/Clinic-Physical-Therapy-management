using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class PackageRepository : Repository<Package>, IPackageRepository
    {
        public PackageRepository(ApplicationDbContext context) : base(context) { }

        public Patient? GetPatientWithPackages(int patientId)
        {
            return _context.Patient
                .Include(p => p.Packages)
                .FirstOrDefault(p => p.Id == patientId);
        }

        public List<Check> GetChecksByPatient(int patientId)
            => _context.Checks.Where(c => c.PatientId == patientId).ToList();

        public List<Organization> GetAllOrganizations()
            => _context.Organizations.ToList();

        public List<DoctorSearchDto> SearchDoctors(string term)
        {
            return _context.InternDoctors
                .Where(d => d.FullName.Contains(term))
                .Select(d => new DoctorSearchDto { id = d.InternDoctorId, name = d.FullName })
                .ToList();
        }

        public void AddPackage(Package package) => Add(package);

        public async Task<Package?> GetPackageWithPatientAndOrgAsync(int id)
        {
            return await _context.Packages
                .Include(p => p.Patient)
                .Include(p => p.Organization)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public List<InternDoctor> GetActiveInternDoctors()
            => _context.InternDoctors.Where(d => d.IsActive).ToList();

        public async Task<Package?> GetPackageAsNoTrackingAsync(int id)
            => await _context.Packages.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        public void UpdatePackage(Package package) => Update(package);

        public async Task<Package?> GetPackageDetailsAsync(int id)
        {
            return await _context.Packages
                .AsNoTracking()
                .Include(p => p.Patient)
                .Include(p => p.InternDoctor)
                .Include(p => p.Check)
                .Include(p => p.Organization)
                .Include(p => p.TreatmentSessions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Package>> GetAllPackagesWithIncludesAsync()
        {
            return await _context.Packages
                .Include(p => p.Patient)
                .Include(p => p.InternDoctor)
                .Include(p => p.Organization)
                .Include(p => p.TreatmentSessions)
                .ToListAsync();
        }

        public async Task<List<Package>> GetPatientPackagesAsync(int patientId)
        {
            return await _context.Packages
                .Where(p => p.PatientId == patientId)
                .Include(p => p.InternDoctor)
                .Include(p => p.Organization)
                .ToListAsync();
        }

        public async Task<Patient?> FindPatientAsync(int patientId)
            => await _context.Patient.FindAsync(patientId);
    }
}