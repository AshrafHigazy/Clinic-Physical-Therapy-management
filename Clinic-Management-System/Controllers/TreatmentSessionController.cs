using Clinic_Management_System.Services.TreatmentSessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class TreatmentSessionController : Controller
    {
        private readonly ITreatmentSessionService _sessionService;

        public TreatmentSessionController(ITreatmentSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<IActionResult> Create(int patientId)
        {
            var result = await _sessionService.CreateSessionAsync(patientId);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                if (result.Message == "المريض غير موجود.")
                    return RedirectToAction("GetAll", "Patient");

                return RedirectToAction("PatientPackages", "Packages", new { patientId });
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("PatientPackages", "Packages", new { patientId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var session = await _sessionService.GetSessionByIdAsync(id);
            if (session == null)
                return NotFound();

            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string prognosis)
        {
            var result = await _sessionService.EditPrognosisAsync(id, prognosis);
            if (!result.Success)
                return NotFound();

            TempData["Success"] = result.Message;
            return RedirectToAction("Details", "Packages", new { id = result.Data });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var session = await _sessionService.GetSessionWithPackageAsync(id);
            if (session == null)
                return NotFound();

            return View(session);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _sessionService.DeleteSessionAsync(id);
            if (!result.Success)
                return NotFound();

            TempData["Success"] = result.Message;
            return RedirectToAction("Details", "Packages", new { id = result.Data });
        }
    }
}