using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class ChecksController : Controller
    {
        private readonly ICheckRepository _checkRepository;

        public ChecksController(ICheckRepository checkRepository)
        {
            _checkRepository = checkRepository;
        }

        public async Task<IActionResult> Index()
        {
            var checks = await _checkRepository.GetChecksAsync();

            return View(checks);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _checkRepository.GetCheckDetailsAsync(id);

            if (check == null)
                return NotFound();

            return View(check);
        }

        public IActionResult Create(int? patientId)
        {
            if (patientId.HasValue)
            {
                var patient = _checkRepository.GetPatientById(patientId.Value);
                ViewBag.PatientName = patient?.FullName;

                var check = new Check
                {
                    PatientId = patientId.Value,
                    CreatedAt = System.DateTime.Now
                };
                return View(check);
            }

            ViewData["PatientId"] = new SelectList(_checkRepository.GetPatientsForSelect(), "Id", "FullName");
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
                ViewData["PatientId"] = new SelectList(_checkRepository.GetPatientsForSelect(), "Id", "FullName", check.PatientId);
                return View(check);
            }

            check.CreatedAt = System.DateTime.Now;
            await _checkRepository.AddCheckAsync(check);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _checkRepository.GetCheckForEditAsync(id);

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

            try
            {
                await _checkRepository.UpdateCheckAsync(check);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_checkRepository.CheckExists(check.CheckId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "AdminDoctor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _checkRepository.GetCheckForEditAsync(id);

            if (check == null)
                return NotFound();

            ViewBag.PatientName = check.Patient?.FullName;
            return View(check);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var check = await _checkRepository.FindCheckAsync(id);
            if (check != null)
            {
                await _checkRepository.RemoveCheckAsync(check);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}