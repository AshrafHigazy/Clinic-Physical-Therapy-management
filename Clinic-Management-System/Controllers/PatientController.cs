using Clinic_Management_System.Data;
using Clinic_Management_System.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
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

            var patients = _context.Patient.AsQueryable();

            // البحث
            if (!string.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(p =>
                    p.FullName.Contains(searchString) ||
                    p.Phone.Contains(searchString));

            }

            // الترتيب
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

            return View("GetAll", patients.ToList());
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Patient patient = _context.Patient.FirstOrDefault(p => p.Id == id);
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
            patient.CreateAt = DateTime.Now;
            _context.Patient.Add(patient);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "تمت إضافة المريض بنجاح!";

            return RedirectToAction("GetAll");
        }


        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            Patient patient = _context.Patient.Find(id);
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
                    Patient patientDB = _context.Patient.FirstOrDefault(p => p.Id == id);
                    patientDB.FullName = patientFromRq.FullName;
                    patientDB.Address = patientFromRq.Address;
                    patientDB.Age = patientFromRq.Age;
                    patientDB.CreateAt = patientFromRq.CreateAt;
                    patientDB.Phone = patientFromRq.Phone;
                    patientDB.Gender = patientFromRq.Gender;
                    patientDB.MedicalRecords = patientFromRq.MedicalRecords;
                    _context.Update(patientDB);
                    _context.SaveChanges();
                    TempData["EditMessage"] = "تم تعديل بيانات المريض بنجاح!";
                    return RedirectToAction("GetAll");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Patient.Any(p => p.Id == id))
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

            Patient patient = _context.Patient.FirstOrDefault(p => p.Id == id);
            if (patient != null)
            {
                _context.Patient.Remove(patient);
                _context.SaveChanges();
                return RedirectToAction("GetAll");
            }
            else
            {
                return NotFound();
            }

        }

    }
}