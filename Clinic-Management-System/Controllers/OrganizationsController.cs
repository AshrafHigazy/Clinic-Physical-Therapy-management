using Clinic_Management_System.Data;
using Clinic_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management_System.Controllers
{
    [Authorize(Roles = "AdminDoctor,Secretary")]

    public class OrganizationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganizationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Organizations
        public async Task<IActionResult> Index()
        {
            // ✅ الترتيب: الشركات المتعاقدة أولاً ثم حسب التاريخ الأحدث
            var organizations = await _context.Organizations
                .OrderByDescending(o => o.IsActive)       // المتعاقدة حالياً أولاً
                .ThenByDescending(o => o.CreateAt)        // الأحدث في التاريخ
                .ToListAsync();

            return View(organizations);
        }

        // GET: Organizations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var organization = await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (organization == null) return NotFound();

            return View(organization);
        }

        // GET: Organizations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Organizations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CreateAt,TyppeOfContract,IsActive")] Organization organization)
        {
            if (ModelState.IsValid)
            {
                // تحويل القائمة إلى string للتخزين
                if (organization.TyppeOfContract != null)
                    organization.TyppeOfContractSerialized = string.Join(",", organization.TyppeOfContract);

                _context.Add(organization);
                await _context.SaveChangesAsync();

                // ✅ رسالة نجاح الإضافة
                TempData["SuccessMessage"] = "تمت إضافة الشركة بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }

        // POST: Organizations/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if (organization == null)
                return NotFound();

            // عكس حالة التعاقد
            organization.IsActive = !organization.IsActive;

            _context.Update(organization);
            await _context.SaveChangesAsync();

            // ✅ رسالة حالة التفعيل/الإلغاء
            TempData["EditMessage"] = organization.IsActive
                ? "تم تفعيل التعاقد بنجاح!"
                : "تم إلغاء التعاقد بنجاح!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Organizations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var organization = await _context.Organizations.FindAsync(id);
            if (organization == null) return NotFound();

            return View(organization);
        }

        // POST: Organizations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreateAt,TyppeOfContract,IsActive")] Organization organization)
        {
            if (id != organization.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // تحويل القائمة إلى string للتخزين
                    if (organization.TyppeOfContract != null)
                        organization.TyppeOfContractSerialized = string.Join(",", organization.TyppeOfContract);

                    _context.Update(organization);
                    await _context.SaveChangesAsync();

                    // ✅ رسالة نجاح التعديل
                    TempData["EditMessage"] = "تم تعديل بيانات الشركة بنجاح!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizationExists(organization.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(organization);
        }
        [Authorize(Roles = "AdminDoctor")]

        // GET: Organizations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var organization = await _context.Organizations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (organization == null) return NotFound();

            return View(organization);
        }
        [Authorize(Roles = "AdminDoctor")]

        // POST: Organizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if (organization != null)
            {
                _context.Organizations.Remove(organization);
                await _context.SaveChangesAsync();

                // ✅ رسالة نجاح الحذف
                TempData["DeleteMessage"] = "تم حذف الشركة بنجاح!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrganizationExists(int id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }
    }
}
