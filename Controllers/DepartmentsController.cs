using Kadry.Data;
using Kadry.Models;
using Microsoft.EntityFrameworkCore;

namespace Kadry.Controllers
{
    public class DepartmentsController : LookupControllerBase<Department>
    {
        public DepartmentsController(AppDbContext db) : base(db) { }

        protected override DbSet<Department> Set => Db.Departments;
        protected override string Title => "Подразделения";
        protected override string ControllerName => "Departments";
        protected override bool HasEmployees(int id) => Db.Employees.Any(e => e.DepartmentId == id);
    }
}
