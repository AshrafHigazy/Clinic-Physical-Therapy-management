using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class ReceptionistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReceptionistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ الترتيب: النشط أولاً ثم الأحدث بالتاريخ
        public async Task<IActionResult> Index()
        {
            var list = await _context.Receptionist
                .OrderByDescending(r => r.IsActive)
                .ThenByDescending(r => r.HiringDate)
                .ToListAsync();

            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _context.Receptionist
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        public IActionResult Create()
        {
            var model = new Receptionist
            {
                HiringDate = DateTime.Now,
                IsActive = true
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Salary,Shift,Phone,Role")] Receptionist receptionist)
        {
            if (ModelState.IsValid)
            {
                receptionist.HiringDate = DateTime.Now;
                receptionist.IsActive = true;
                receptionist.AppointmentLinks = new List<AppointmentByPatientOrReceptionist>();

                _context.Add(receptionist);
                await _context.SaveChangesAsync();

                // ✅ رسالة نجاح الإضافة
                TempData["SuccessMessage"] = "تمت إضافة موظف الاستقبال بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            return View(receptionist);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _context.Receptionist.FindAsync(id);
            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,HiringDate,Salary,Shift,Phone,IsActive,Password,Role,Email")] Receptionist receptionist)
        {
            if (id != receptionist.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receptionist);
                    await _context.SaveChangesAsync();

                    // ✅ رسالة نجاح التعديل
                    TempData["EditMessage"] = "تم تعديل بيانات موظف الاستقبال بنجاح!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceptionistExists(receptionist.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(receptionist);
        }
        [Authorize(Roles = "AdminDoctor")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var receptionist = await _context.Receptionist
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AdminDoctor")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receptionist = await _context.Receptionist.FindAsync(id);
            if (receptionist != null)
            {
                _context.Receptionist.Remove(receptionist);
                await _context.SaveChangesAsync();

                // ✅ رسالة نجاح الحذف
                TempData["DeleteMessage"] = "تم حذف موظف الاستقبال بنجاح!";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleStatus(int id)
        {
            var receptionist = await _context.Receptionist.FindAsync(id);
            if (receptionist == null)
                return NotFound();

            receptionist.IsActive = !receptionist.IsActive;
            _context.Update(receptionist);
            await _context.SaveChangesAsync();

            // ✅ رسالة تفعيل أو تعطيل
            TempData["EditMessage"] = receptionist.IsActive
                ? "تم تفعيل الموظف بنجاح!"
                : "تم تعطيل الموظف بنجاح!";

            return RedirectToAction(nameof(Index));
        }

        private bool ReceptionistExists(int id)
        {
            return _context.Receptionist.Any(e => e.Id == id);
        }
    }
}
