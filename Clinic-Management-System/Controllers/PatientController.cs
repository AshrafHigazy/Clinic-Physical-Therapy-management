using Clinic_Management_System.Models;
using Clinic_Management_System.Models.Enums;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public IActionResult GetAll(string searchString, string sortOrder, string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                ViewBag.SelectedDate = date;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["AgeSortParm"] = sortOrder == "Age" ? "Age_desc" : "Age";
            ViewData["DateSortParm"] = System.String.IsNullOrEmpty(sortOrder) ? "Date_desc" : "";

            var patients = _patientRepository.GetPatients(searchString, sortOrder);

            return View("GetAll", patients);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Patient patient = _patientRepository.GetPatientById(id);
            if (patient == null)
                return NotFound();

            return View("Details", patient);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Patient patient, List<MedicalRecord> MedicalRecords)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", patient);
            }
            patient.CreateAt = System.DateTime.Now;
            _patientRepository.AddPatient(patient);

            TempData["SuccessMessage"] = "تمت إضافة المريض بنجاح!";

            return RedirectToAction("GetAll");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            Patient patient = _patientRepository.FindPatient(id);
            if (patient == null) return NotFound();

            return View("Edit", patient);
        }
        #region Edit Action (Not Complete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Patient patientFromRq)
        {
            if (id != patientFromRq.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    Patient patientDB = _patientRepository.GetPatientById(id);
                    patientDB.FullName = patientFromRq.FullName;
                    patientDB.Address = patientFromRq.Address;
                    patientDB.Age = patientFromRq.Age;
                    patientDB.CreateAt = patientFromRq.CreateAt;
                    patientDB.Phone = patientFromRq.Phone;
                    patientDB.Gender = patientFromRq.Gender;
                    patientDB.MedicalRecords = patientFromRq.MedicalRecords;
                    _patientRepository.UpdatePatient(patientDB);
                    TempData["EditMessage"] = "تم تعديل بيانات المريض بنجاح!";
                    return RedirectToAction("GetAll");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_patientRepository.PatientExists(id))
                        return NotFound();
                    throw;
                }
            }
            return View("Edit", patientFromRq);
        }
        #endregion
        [Authorize(Roles = "AdminDoctor")]

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            Patient patient = _patientRepository.GetPatientById(id);
            if (patient != null)
            {
                _patientRepository.RemovePatient(patient);
                return RedirectToAction("GetAll");
            }
            else
            {
                return NotFound();
            }

        }

    }
}