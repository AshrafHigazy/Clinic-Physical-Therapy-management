using Clinic_Management_System.Data;
using Clinic_Management_System.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Repositories
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context) { }

        public System.Collections.Generic.List<Patient> GetPatients(string? searchString, string sortOrder)
        {
            var patients = _context.Patient.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(p =>
                    p.FullName.Contains(searchString) ||
                    p.Phone.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Age":
                    patients = patients.OrderBy(p => p.Age);
                    break;
                case "Age_desc":
                    patients = patients.OrderByDescending(p => p.Age);
                    break;
                case "Date_desc":
                    patients = patients.OrderByDescending(p => p.CreateAt);
                    break;
                default:
                    patients = patients.OrderBy(p => p.CreateAt);
                    break;
            }

            return patients.ToList();
        }

        public Patient? GetPatientById(int? id)
            => _context.Patient.FirstOrDefault(p => p.Id == id);

        public void AddPatient(Patient patient) => Add(patient);

        public Patient? FindPatient(int? id) => GetById(id!);

        public void UpdatePatient(Patient patient) => Update(patient);

        public bool PatientExists(int id) => Exists(p => p.Id == id);

        public void RemovePatient(Patient patient) => Remove(patient);
    }
}