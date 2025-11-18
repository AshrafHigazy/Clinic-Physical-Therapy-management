using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]
    public class TreatmentSessionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TreatmentSessionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create(int patientId)
        {
            var patient = await _context.Patient.FindAsync(patientId);
            if (patient == null)
            {
                TempData["Error"] = "المريض غير موجود.";
                return RedirectToAction("GetAll", "Patient");
            }

            var package = await _context.Packages
                .Where(p => p.PatientId == patientId && p.Status == "Active" && p.NumOfSessions > p.SessionsCount)
                .OrderBy(p => p.StartDate)
                .FirstOrDefaultAsync();

            if (package == null)
            {
                TempData["Error"] = "❌ لا توجد باكدج متاحة لهذا المريض أو كل الجلسات خلصت.";
                return RedirectToAction("PatientPackages", "Packages", new { patientId });
            }

            // إنشاء جلسة
            var session = new TreatmentSession
            {
                PackageId = package.Id,
                SessionDate = DateTime.Now,
                Prognosis = "غير محدد حالياً"
            };

            _context.treatmentSessions.Add(session);

            package.SessionsCount++;

            if (package.SessionsCount >= package.NumOfSessions)
            {
                package.Status = "Ended";
                package.EndDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "✔ تم إضافة الجلسة بنجاح.";
            return RedirectToAction("PatientPackages", "Packages", new { patientId });
        }

        // Edit session GET
        public async Task<IActionResult> Edit(int id)
        {
            var session = await _context.treatmentSessions
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
                return NotFound();

            return View(session);
        }


        // Edit session POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string prognosis)
        {
            var session = await _context.treatmentSessions.FindAsync(id);

            if (session == null)
                return NotFound();

            session.Prognosis = prognosis;

            _context.treatmentSessions.Update(session);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✔ تم تعديل التقييم بنجاح.";
            return RedirectToAction("Details", "Packages", new { id = session.PackageId });
        }


        // Delete confirm GET
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _context.treatmentSessions
                .Include(s => s.Package)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            return View(session);
        }

        // Delete POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.treatmentSessions.FindAsync(id);
            if (session == null) return NotFound();

            // قبل الحذف: نطرح من SessionsCount بحيث نرجع الباقة لو كانت Ended
            var package = await _context.Packages.FindAsync(session.PackageId);
            if (package != null)
            {
                // guard: SessionsCount should be >0
                if (package.SessionsCount > 0)
                {
                    package.SessionsCount -= 1;

                    // لو الباقة كانت Ended و دلوقتي بقت فيها جلسات متبقية -> نعيدها Active
                    if (package.SessionsCount < package.NumOfSessions)
                    {
                        package.Status = "Active";
                        package.EndDate = null;
                    }

                    _context.Packages.Update(package);
                }
            }

            _context.treatmentSessions.Remove(session);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حذف الجلسة بنجاح.";
            // نرجع لصفحة المريض عبر باكدج (إذا عايز ترجع لـ GetAll استخدمه)
            return RedirectToAction("Details", "Packages", new { id = session.PackageId });
        }
    }
}
