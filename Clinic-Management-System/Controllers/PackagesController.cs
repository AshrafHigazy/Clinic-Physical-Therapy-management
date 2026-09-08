using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Packages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class PackagesController : Controller
    {
        private readonly IPackageService _packageService;

        public PackagesController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        public IActionResult Create(int patientId)
        {
            var data = _packageService.GetCreateFormData(patientId);
            if (data == null)
                return NotFound("المريض غير موجود");

            ViewBag.PatientId = data.PatientId;
            ViewBag.PatientName = data.PatientName;
            ViewBag.Checks = data.Checks;
            ViewBag.Organizations = data.Organizations;
            TempData["Info"] = data.InfoMessage;

            return View();
        }

        [HttpGet]
        public IActionResult SearchDoctors(string term)
        {
            var doctors = _packageService.SearchDoctors(term);
            return Json(doctors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int patientId, Package package)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Organizations = _packageService.GetAllOrganizations();
                ViewBag.Checks = _packageService.GetChecksByPatient(patientId);
                TempData["Error"] = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage));
                return View(package);
            }

            var result = await _packageService.CreatePackageAsync(patientId, package);
            if (!result.Success)
            {
                ModelState.AddModelError(nameof(package.NumOfSessions), result.Message!);
                ViewBag.Organizations = _packageService.GetAllOrganizations();
                ViewBag.Checks = _packageService.GetChecksByPatient(patientId);
                return View(package);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("GetAll", "Patient");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var editData = await _packageService.GetEditFormDataAsync(id);
            if (editData == null) return NotFound();

            ViewBag.Doctors = editData.Doctors;
            ViewBag.Organizations = editData.Organizations;
            ViewBag.Checks = editData.Checks;

            return View(editData.Package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Package package)
        {
            if (id != package.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = _packageService.GetActiveInternDoctors();
                ViewBag.Organizations = _packageService.GetAllOrganizations();
                ViewBag.Checks = _packageService.GetChecksByPatient(package.PatientId);
                return View(package);
            }

            var result = await _packageService.UpdatePackageAsync(id, package);
            if (!result.Success)
            {
                if (result.Message == "الباقة غير موجودة." || result.Message == "معرف الباقة غير متطابق.")
                    return NotFound();

                if (result.Message != null && (result.Message.Contains("عدد الجلسات") || result.Message.Contains("تقليل إجمالي الجلسات")))
                {
                    ModelState.AddModelError(nameof(package.NumOfSessions), result.Message);
                    ViewBag.Doctors = _packageService.GetActiveInternDoctors();
                    ViewBag.Organizations = _packageService.GetAllOrganizations();
                    ViewBag.Checks = _packageService.GetChecksByPatient(package.PatientId);
                    return View(package);
                }

                TempData["Error"] = result.Message;
                ViewBag.Doctors = _packageService.GetActiveInternDoctors();
                ViewBag.Organizations = _packageService.GetAllOrganizations();
                ViewBag.Checks = _packageService.GetChecksByPatient(package.PatientId);
                return View(package);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("GetAll", "Patient");
        }

        public async Task<IActionResult> Details(int id)
        {
            var package = await _packageService.GetPackageDetailsAsync(id);
            if (package == null) return NotFound();

            return View(package);
        }

        public async Task<IActionResult> Index()
        {
            var packages = await _packageService.GetAllPackagesSortedAsync();
            return View(packages);
        }

        public async Task<IActionResult> PatientPackages(int patientId)
        {
            var (packages, patient) = await _packageService.GetPatientPackagesAsync(patientId);

            ViewBag.PatientName = patient?.FullName;
            ViewBag.PatientId = patientId;

            return View(packages);
        }
    }
}