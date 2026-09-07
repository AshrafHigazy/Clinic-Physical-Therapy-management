using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class TreatmentSessionRepository : ITreatmentSessionRepository
    {
        private readonly ApplicationDbContext _context;

        public TreatmentSessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Patient?> FindPatientAsync(int patientId)
        {
            return await _context.Patient.FindAsync(patientId);
        }

        public async Task<Package?> GetActivePackageByPatientAsync(int patientId)
        {
            return await _context.Packages
                .Where(p => p.PatientId == patientId && p.Status == "Active" && p.NumOfSessions > p.SessionsCount)
                .OrderBy(p => p.StartDate)
                .FirstOrDefaultAsync();
        }

        public void AddTreatmentSession(TreatmentSession session)
        {
            _context.treatmentSessions.Add(session);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<TreatmentSession?> GetSessionByIdAsync(int id)
        {
            return await _context.treatmentSessions
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<TreatmentSession?> FindSessionAsync(int id)
        {
            return await _context.treatmentSessions.FindAsync(id);
        }

        public async Task UpdateSessionAsync(TreatmentSession session)
        {
            _context.treatmentSessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task<TreatmentSession?> GetSessionWithPackageAsync(int id)
        {
            return await _context.treatmentSessions
                .Include(s => s.Package)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Package?> FindPackageAsync(int id)
        {
            return await _context.Packages.FindAsync(id);
        }

        public void UpdatePackage(Package package)
        {
            _context.Packages.Update(package);
        }

        public void RemoveSession(TreatmentSession session)
        {
            _context.treatmentSessions.Remove(session);
        }

        public async Task SaveChangesForDeleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}