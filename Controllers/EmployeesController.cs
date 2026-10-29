using Kadry.Data;
using Kadry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kadry.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _db;
        public EmployeesController(AppDbContext db) => _db = db;

        public IActionResult Index()
        {
            var employees = _db.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Profession)
                .Include(e => e.Education)
                .OrderBy(e => e.FullName)
                .ToList();
            return View(employees);
        }

        private void FillLookups(Employee? employee = null)
        {
            ViewBag.Departments = new SelectList(_db.Departments.OrderBy(x => x.Name).ToList(), "Id", "Name", employee?.DepartmentId);
            ViewBag.Positions = new SelectList(_db.Positions.OrderBy(x => x.Name).ToList(), "Id", "Name", employee?.PositionId);
            ViewBag.Professions = new SelectList(_db.Professions.OrderBy(x => x.Name).ToList(), "Id", "Name", employee?.ProfessionId);
            ViewBag.Educations = new SelectList(_db.Educations.OrderBy(x => x.Name).ToList(), "Id", "Name", employee?.EducationId);
        }

        public IActionResult Create()
        {
            FillLookups();
            return View("Form", new Employee());
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (employee.HireDate < employee.BirthDate)
                ModelState.AddModelError("HireDate", "Дата приёма не может быть раньше даты рождения.");

            if (!ModelState.IsValid)
            {
                FillLookups(employee);
                return View("Form", employee);
            }

            _db.Employees.Add(employee);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var employee = _db.Employees.Find(id);
            if (employee == null) return NotFound();
            FillLookups(employee);
            return View("Form", employee);
        }

        [HttpPost]
        public IActionResult Edit(int id, Employee employee)
        {
            if (id != employee.Id) return BadRequest();

            if (employee.HireDate < employee.BirthDate)
                ModelState.AddModelError("HireDate", "Дата приёма не может быть раньше даты рождения.");

            if (!ModelState.IsValid)
            {
                FillLookups(employee);
                return View("Form", employee);
            }

            _db.Employees.Update(employee);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var employee = _db.Employees.Find(id);
            if (employee != null)
            {
                _db.Employees.Remove(employee);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
