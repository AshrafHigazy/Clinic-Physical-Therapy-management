using Clinic_Management_System.Models;

namespace Clinic_Management_System.Repositories
{
    public interface IPatientRepository
    {
        System.Collections.Generic.List<Patient> GetPatients(string? searchString, string sortOrder);
        Patient? GetPatientById(int? id);
        void AddPatient(Patient patient);
        Patient? FindPatient(int? id);
        void UpdatePatient(Patient patient);
        bool PatientExists(int id);
        void RemovePatient(Patient patient);
    }
}