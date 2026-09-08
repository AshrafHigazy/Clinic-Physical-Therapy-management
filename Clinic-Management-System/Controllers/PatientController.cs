using Clinic_Management_System.Models;
using Clinic_Management_System.Models.Enums;
using Clinic_Management_System.Services.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public IActionResult GetAll(string searchString, string sortOrder, string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                ViewBag.SelectedDate = date;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["AgeSortParm"] = sortOrder == "Age" ? "Age_desc" : "Age";
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Date_desc" : "";

            var patients = _patientService.GetPatients(searchString, sortOrder);

            return View("GetAll", patients);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var patient = _patientService.GetPatientById(id);
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

            var result = _patientService.CreatePatient(patient);
            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction("GetAll");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var patient = _patientService.FindPatient(id);
            if (patient == null) return NotFound();

            return View("Edit", patient);
        }

        #region Edit Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Patient patientFromRq)
        {
            if (id != patientFromRq.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var result = _patientService.UpdatePatient(id, patientFromRq);
                if (!result.Success)
                    return NotFound();

                TempData["EditMessage"] = result.Message;
                return RedirectToAction("GetAll");
            }
            return View("Edit", patientFromRq);
        }
        #endregion

        [Authorize(Roles = "AdminDoctor")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var result = _patientService.DeletePatient(id);
            if (!result.Success)
                return NotFound();

            return RedirectToAction("GetAll");
        }
    }
}