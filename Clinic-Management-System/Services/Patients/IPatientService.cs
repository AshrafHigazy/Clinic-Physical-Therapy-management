using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Common;
using System.Collections.Generic;

namespace Clinic_Management_System.Services.Patients
{
    public interface IPatientService
    {
        List<Patient> GetPatients(string? searchString, string? sortOrder);
        Patient? GetPatientById(int? id);
        Patient? FindPatient(int? id);
        ServiceResult CreatePatient(Patient patient);
        ServiceResult UpdatePatient(int id, Patient patientFromRq);
        ServiceResult DeletePatient(int? id);
    }
}
