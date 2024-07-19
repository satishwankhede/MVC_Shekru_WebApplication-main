using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Shekru_WebApplication.Models;

namespace MVC_Shekru_WebApplication.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly MVC_ShekruContext _context;

        public EmployeesController(MVC_ShekruContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var sampleContext = _context.Employees.Include(e => e.DesignationIdRefNavigation).Include(e => e.GradeIdRefNavigation);
            return View(await sampleContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.DesignationIdRefNavigation)
                .Include(e => e.GradeIdRefNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DesignationIdRef"] = new SelectList(_context.Designations, "DesignationIdRef", "DesignationName");
            ViewData["GradeIdRef"] = new SelectList(_context.DesignationGrades, "GradeIdRef", "GradeName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Firstname, Lastname, EmailAddress, Phonenumber, DesignationIdRef, GradeIdRef")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DesignationIdRef"] = new SelectList(_context.Designations, "DesignationIdRef", "DesignationName", employee.DesignationIdRef);
            ViewData["GradeIdRef"] = new SelectList(_context.DesignationGrades, "GradeIdRef", "GradeName", employee.GradeIdRef);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DesignationIdRef"] = new SelectList(_context.Designations, "DesignationIdRef", "DesignationName", employee.DesignationIdRef);
            ViewData["GradeIdRef"] = new SelectList(_context.DesignationGrades, "GradeIdRef", "GradeName", employee.GradeIdRef);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Firstname,Lastname,EmailAddress,Phonenumber,DesignationIdRef,GradeIdRef")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DesignationIdRef"] = new SelectList(_context.Designations, "DesignationIdRef", "DesignationName", employee.DesignationIdRef);
            ViewData["GradeIdRef"] = new SelectList(_context.DesignationGrades, "GradeIdRef", "GradeName", employee.GradeIdRef);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.DesignationIdRefNavigation)
                .Include(e => e.GradeIdRefNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        [HttpPost]
        public IActionResult FindGrid(string CId)
        {
            var grade = _context.DesignationGrades
                .Where(u => u.DesignationIdRef == Convert.ToInt32(CId))
                .Select(s => new { Id = s.GradeIdRef, Name = s.GradeName })
                .OrderBy(s => s.Name)
                .ToList();
            return Json(grade);
        }
    }
}
