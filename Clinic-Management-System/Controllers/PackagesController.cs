using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class PackagesController : Controller
    {
        private readonly IPackageRepository _packageRepository;

        public PackagesController(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        [HttpGet]
        public IActionResult Create(int patientId)
        {
            var patient = _packageRepository.GetPatientWithPackages(patientId);

            if (patient == null)
                return NotFound("المريض غير موجود");

            ViewBag.PatientId = patient.Id;
            ViewBag.PatientName = patient.FullName;
            ViewBag.Checks = _packageRepository.GetChecksByPatient(patientId);
            ViewBag.Organizations = _packageRepository.GetAllOrganizations();

            var packages = patient.Packages.ToList();
            if (!packages.Any())
            {
                TempData["Info"] = "المريض لا يملك أي باقات.";
            }
            else
            {
                var totalRemaining = packages.Sum(p => p.NumOfSessions - p.SessionsCount);
                TempData["Info"] = $"المريض لديه {totalRemaining} جلسات متبقية.";
            }

            return View();
        }

        [HttpGet]
        public IActionResult SearchDoctors(string term)
        {
            var doctors = _packageRepository.SearchDoctors(term);

            return Json(doctors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int patientId, Package package)
        {

            package.PatientId = patientId;

            package.SessionsCount = 0;
            package.StartDate = System.DateTime.Now;
            package.EndDate = null;
            package.Status = "Active";

            if (!ModelState.IsValid)
            {
                ViewBag.Organizations = _packageRepository.GetAllOrganizations();
                ViewBag.Checks = _packageRepository.GetChecksByPatient(patientId);
                TempData["Error"] = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage));
                return View(package);
            }

            if (package.NumOfSessions <= 0)
            {
                ModelState.AddModelError(nameof(package.NumOfSessions), "عدد الجلسات يجب أن يكون أكبر من صفر.");
                ViewBag.Organizations = _packageRepository.GetAllOrganizations();
                ViewBag.Checks = _packageRepository.GetChecksByPatient(patientId);
                return View(package);
            }

            await _packageRepository.AddPackageAsync(package);

            TempData["Success"] = "تمت إضافة الباقة بنجاح.";

            return RedirectToAction("GetAll", "Patient");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var package = await _packageRepository.GetPackageWithPatientAndOrgAsync(id);

            if (package == null) return NotFound();

            ViewBag.Doctors = _packageRepository.GetActiveInternDoctors();
            ViewBag.Organizations = _packageRepository.GetAllOrganizations();
            ViewBag.Checks = _packageRepository.GetChecksByPatient(package.PatientId);

            return View(package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Package package)
        {
            if (id != package.Id) return NotFound();

            var existing = await _packageRepository.GetPackageAsNoTrackingAsync(id);
            if (existing == null) return NotFound();

            package.PatientId = existing.PatientId;

            if (package.NumOfSessions <= 0)
            {
                ModelState.AddModelError(nameof(package.NumOfSessions), "عدد الجلسات يجب أن يكون أكبر من صفر.");
            }

            if (package.NumOfSessions < existing.SessionsCount)
            {
                ModelState.AddModelError(nameof(package.NumOfSessions),
                    $"لا يمكن تقليل إجمالي الجلسات ({package.NumOfSessions}) أقل من الجلسات المستخدمة فعلاً ({existing.SessionsCount}).");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = _packageRepository.GetActiveInternDoctors();
                ViewBag.Organizations = _packageRepository.GetAllOrganizations();
                ViewBag.Checks = _packageRepository.GetChecksByPatient(package.PatientId);
                return View(package);
            }

            package.SessionsCount = existing.SessionsCount;

            package.Status = "Active";
            package.EndDate = null;

            if (package.SessionsCount >= package.NumOfSessions)
            {
                package.SessionsCount = package.NumOfSessions;
                package.Status = "Ended";
                package.EndDate = System.DateTime.Now;
            }

            try
            {
                await _packageRepository.UpdatePackageAsync(package);

                TempData["Success"] = "تم تعديل الباقة بنجاح.";

                return RedirectToAction("GetAll", "Patient");
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "حدث خطأ أثناء الحفظ: " + ex.Message;
                ViewBag.Doctors = _packageRepository.GetActiveInternDoctors();
                ViewBag.Organizations = _packageRepository.GetAllOrganizations();
                ViewBag.Checks = _packageRepository.GetChecksByPatient(package.PatientId);
                return View(package);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var package = await _packageRepository.GetPackageDetailsAsync(id);

            if (package == null) return NotFound();

            if (package.TreatmentSessions != null)
                package.TreatmentSessions = package.TreatmentSessions.OrderBy(s => s.SessionDate).ToList();

            return View(package);
        }

        public async Task<IActionResult> Index()
        {
            var packages = await _packageRepository.GetAllPackagesWithIncludesAsync();

            packages = packages
                .OrderBy(p => p.Status == "Ended")
                .ThenByDescending(p => p.StartDate)
                .ToList();

            return View(packages);
        }

        public async Task<IActionResult> PatientPackages(int patientId)
        {
            var packages = await _packageRepository.GetPatientPackagesAsync(patientId);

            var patient = await _packageRepository.FindPatientAsync(patientId);

            ViewBag.PatientName = patient?.FullName;
            ViewBag.PatientId = patientId;

            return View(packages);
        }

    }
}