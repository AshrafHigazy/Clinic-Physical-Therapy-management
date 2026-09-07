using Clinic_Management_System.Models;
using Clinic_Management_System.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class TreatmentSessionController : Controller
    {
        private readonly ITreatmentSessionRepository _treatmentSessionRepository;

        public TreatmentSessionController(ITreatmentSessionRepository treatmentSessionRepository)
        {
            _treatmentSessionRepository = treatmentSessionRepository;
        }

        public async Task<IActionResult> Create(int patientId)
        {
            var patient = await _treatmentSessionRepository.FindPatientAsync(patientId);
            if (patient == null)
            {
                TempData["Error"] = "المريض غير موجود.";
                return RedirectToAction("GetAll", "Patient");
            }

            var package = await _treatmentSessionRepository.GetActivePackageByPatientAsync(patientId);

            if (package == null)
            {
                TempData["Error"] = "❌ لا توجد باكدج متاحة لهذا المريض أو كل الجلسات خلصت.";
                return RedirectToAction("PatientPackages", "Packages", new { patientId });
            }

            var session = new TreatmentSession
            {
                PackageId = package.Id,
                SessionDate = System.DateTime.Now,
                Prognosis = "غير محدد حالياً"
            };

            _treatmentSessionRepository.AddTreatmentSession(session);

            package.SessionsCount++;

            if (package.SessionsCount >= package.NumOfSessions)
            {
                package.Status = "Ended";
                package.EndDate = System.DateTime.Now;
            }

            await _treatmentSessionRepository.SaveChangesAsync();

            TempData["Success"] = "✔ تم إضافة الجلسة بنجاح.";
            return RedirectToAction("PatientPackages", "Packages", new { patientId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var session = await _treatmentSessionRepository.GetSessionByIdAsync(id);

            if (session == null)
                return NotFound();

            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string prognosis)
        {
            var session = await _treatmentSessionRepository.FindSessionAsync(id);

            if (session == null)
                return NotFound();

            session.Prognosis = prognosis;

            await _treatmentSessionRepository.UpdateSessionAsync(session);

            TempData["Success"] = "✔ تم تعديل التقييم بنجاح.";
            return RedirectToAction("Details", "Packages", new { id = session.PackageId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var session = await _treatmentSessionRepository.GetSessionWithPackageAsync(id);

            if (session == null) return NotFound();

            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _treatmentSessionRepository.FindSessionAsync(id);
            if (session == null) return NotFound();

            var package = await _treatmentSessionRepository.FindPackageAsync(session.PackageId);
            if (package != null)
            {

                if (package.SessionsCount > 0)
                {
                    package.SessionsCount -= 1;

                    if (package.SessionsCount < package.NumOfSessions)
                    {
                        package.Status = "Active";
                        package.EndDate = null;
                    }

                    _treatmentSessionRepository.UpdatePackage(package);
                }
            }

            _treatmentSessionRepository.RemoveSession(session);
            await _treatmentSessionRepository.SaveChangesForDeleteAsync();

            TempData["Success"] = "تم حذف الجلسة بنجاح.";

            return RedirectToAction("Details", "Packages", new { id = session.PackageId });
        }
    }
}