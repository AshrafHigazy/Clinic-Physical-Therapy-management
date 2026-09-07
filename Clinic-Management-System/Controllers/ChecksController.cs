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
        private readonly IUnitOfWork _unitOfWork;

        public ChecksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var checks = await _unitOfWork.Checks.GetChecksAsync();

            return View(checks);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _unitOfWork.Checks.GetCheckDetailsAsync(id);

            if (check == null)
                return NotFound();

            return View(check);
        }

        public IActionResult Create(int? patientId)
        {
            if (patientId.HasValue)
            {
                var patient = _unitOfWork.Checks.GetPatientById(patientId.Value);
                ViewBag.PatientName = patient?.FullName;

                var check = new Check
                {
                    PatientId = patientId.Value,
                    CreatedAt = System.DateTime.Now
                };
                return View(check);
            }

            ViewData["PatientId"] = new SelectList(_unitOfWork.Checks.GetPatientsForSelect(), "Id", "FullName");
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
                ViewData["PatientId"] = new SelectList(_unitOfWork.Checks.GetPatientsForSelect(), "Id", "FullName", check.PatientId);
                return View(check);
            }

            check.CreatedAt = System.DateTime.Now;
            _unitOfWork.Checks.AddCheck(check);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var check = await _unitOfWork.Checks.GetCheckForEditAsync(id);

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
                _unitOfWork.Checks.UpdateCheck(check);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_unitOfWork.Checks.CheckExists(check.CheckId))
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

            var check = await _unitOfWork.Checks.GetCheckForEditAsync(id);

            if (check == null)
                return NotFound();

            ViewBag.PatientName = check.Patient?.FullName;
            return View(check);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var check = await _unitOfWork.Checks.FindCheckAsync(id);
            if (check != null)
            {
                _unitOfWork.Checks.RemoveCheck(check);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}