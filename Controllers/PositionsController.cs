using Kadry.Data;
using Kadry.Models;
using Microsoft.EntityFrameworkCore;

namespace Kadry.Controllers
{
    public class PositionsController : LookupControllerBase<Position>
    {
        public PositionsController(AppDbContext db) : base(db) { }

        protected override DbSet<Position> Set => Db.Positions;
        protected override string Title => "Должности";
        protected override string ControllerName => "Positions";
        protected override bool HasEmployees(int id) => Db.Employees.Any(e => e.PositionId == id);
    }
}
