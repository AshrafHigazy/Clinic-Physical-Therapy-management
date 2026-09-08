using Clinic_Management_System.Models;
using Clinic_Management_System.Services.Checks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class ChecksController : Controller
    {
        private readonly ICheckService _checkService;

        public ChecksController(ICheckService checkService)
        {
            _checkService = checkService;
        }

        public async Task<IActionResult> Index()
        {
            var checks = await _checkService.GetChecksAsync();
            return View(checks);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _checkService.GetCheckDetailsAsync(id);
            if (check == null)
                return NotFound();

            return View(check);
        }

        public IActionResult Create(int? patientId)
        {
            if (patientId.HasValue)
            {
                var patient = _checkService.GetPatientById(patientId.Value);
                ViewBag.PatientName = patient?.FullName;

                var check = new Check
                {
                    PatientId = patientId.Value,
                    CreatedAt = System.DateTime.Now
                };
                return View(check);
            }

            ViewData["PatientId"] = new SelectList(_checkService.GetPatientsForSelect(), "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CheckId,PatientId,Sugestion,ClinicAssessment,Diagnosis,PlaneOfTreatment,MethodsOfTreatment")] Check check)
        {
            if (check.PatientId == 0)
            {
                ModelState.AddModelError("PatientId", "يجب اختيار المريض قبل إنشاء الكشف.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["PatientId"] = new SelectList(_checkService.GetPatientsForSelect(), "Id", "FullName", check.PatientId);
                return View(check);
            }

            await _checkService.CreateCheckAsync(check);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _checkService.GetCheckForEditAsync(id);
            if (check == null)
                return NotFound();

            ViewBag.PatientName = check.Patient?.FullName;
            return View(check);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CheckId,PatientId,Sugestion,ClinicAssessment,Diagnosis,PlaneOfTreatment,MethodsOfTreatment,CreatedAt")] Check check)
        {
            if (id != check.CheckId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(check);

            var result = await _checkService.UpdateCheckAsync(id, check);
            if (!result.Success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "AdminDoctor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _checkService.GetCheckForEditAsync(id);
            if (check == null)
                return NotFound();

            ViewBag.PatientName = check.Patient?.FullName;
            return View(check);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _checkService.DeleteCheckAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}