using Kadry.Data;
using Kadry.Models;
using Microsoft.EntityFrameworkCore;

namespace Kadry.Controllers
{
    public class EducationsController : LookupControllerBase<Education>
    {
        public EducationsController(AppDbContext db) : base(db) { }

        protected override DbSet<Education> Set => Db.Educations;
        protected override string Title => "Образование";
        protected override string ControllerName => "Educations";
        protected override bool HasEmployees(int id) => Db.Employees.Any(e => e.EducationId == id);
    }
}
