using Kadry.Data;
using Kadry.Models;
using Microsoft.EntityFrameworkCore;

namespace Kadry.Controllers
{
    public class ProfessionsController : LookupControllerBase<Profession>
    {
        public ProfessionsController(AppDbContext db) : base(db) { }

        protected override DbSet<Profession> Set => Db.Professions;
        protected override string Title => "Профессии";
        protected override string ControllerName => "Professions";
        protected override bool HasEmployees(int id) => Db.Employees.Any(e => e.ProfessionId == id);
    }
}
