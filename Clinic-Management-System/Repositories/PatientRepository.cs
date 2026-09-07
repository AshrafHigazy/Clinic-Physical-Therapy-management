using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Clinic_Management_System.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

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
        {
            return _context.Patient.FirstOrDefault(p => p.Id == id);
        }

        public void AddPatient(Patient patient)
        {
            _context.Patient.Add(patient);
            _context.SaveChanges();
        }

        public Patient? FindPatient(int? id)
        {
            return _context.Patient.Find(id);
        }

        public void UpdatePatient(Patient patient)
        {
            _context.Update(patient);
            _context.SaveChanges();
        }

        public bool PatientExists(int id)
        {
            return _context.Patient.Any(p => p.Id == id);
        }

        public void RemovePatient(Patient patient)
        {
            _context.Patient.Remove(patient);
            _context.SaveChanges();
        }
    }
}