using System.Collections;
using ClosedXML.Excel;
using Kadry.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kadry.Controllers
{
    public class ReportsController : Controller
    {
        private readonly AppDbContext _db;
        public ReportsController(AppDbContext db) => _db = db;

        public IActionResult Index(string? type, int? employeeId)
        {
            ViewBag.Employees = _db.Employees.OrderBy(e => e.FullName).ToList();
            ViewBag.Type = type;

            var data = BuildReport(type, employeeId);
            ViewBag.Columns = data == null ? null : GetColumns(data);
            ViewBag.Rows = data == null ? null : GetRows(data);
            return View();
        }

        private object? BuildReport(string? type, int? employeeId) => type switch
        {
            "card" => BuildCard(employeeId),
            "list" => BuildEmployeeList(),
            "payroll" => BuildPayroll(),
            "education" => BuildEducation(),
            "staffing" => BuildStaffing(),
            _ => null
        };

        private object? BuildCard(int? employeeId)
        {
            if (employeeId == null) return null;
            var e = _db.Employees
                .Include(x => x.Department).Include(x => x.Position)
                .Include(x => x.Profession).Include(x => x.Education)
                .FirstOrDefault(x => x.Id == employeeId);
            if (e == null) return null;

            return new[]
            {
                new { Поле = "ФИО", Значение = e.FullName },
                new { Поле = "Пол", Значение = e.Gender },
                new { Поле = "Дата рождения", Значение = e.BirthDate.ToShortDateString() },
                new { Поле = "Образование", Значение = e.Education!.Name },
                new { Поле = "Должность", Значение = e.Position!.Name },
                new { Поле = "Профессия", Значение = e.Profession!.Name },
                new { Поле = "Подразделение", Значение = e.Department!.Name },
                new { Поле = "Дата приёма", Значение = e.HireDate.ToShortDateString() },
                new { Поле = "Оклад", Значение = e.Salary.ToString("N2") },
                new { Поле = "Паспорт", Значение = $"{e.PassportSeries} {e.PassportNumber}" },
                new { Поле = "Кем выдан", Значение = e.PassportIssuedBy },
                new { Поле = "Дата выдачи", Значение = e.PassportIssueDate.ToShortDateString() },
                new { Поле = "Адрес", Значение = e.Address },
            };
        }

        private object BuildEmployeeList() =>
            _db.Employees
                .Include(e => e.Department).Include(e => e.Position)
                .Include(e => e.Profession).Include(e => e.Education)
                .ToList()
                .OrderBy(e => e.Department!.Name).ThenBy(e => e.FullName)
                .Select(e => new
                {
                    Подразделение = e.Department!.Name,
                    ФИО = e.FullName,
                    Должность = e.Position!.Name,
                    Профессия = e.Profession!.Name,
                    Образование = e.Education!.Name,
                    Оклад = e.Salary.ToString("N2"),
                    ДатаПриёма = e.HireDate.ToShortDateString()
                })
                .ToList();

        private object BuildPayroll() =>
            _db.Employees.Include(e => e.Department).ToList()
                .GroupBy(e => e.Department!.Name)
                .Select(g => new
                {
                    Подразделение = g.Key,
                    Сотрудников = g.Count(),
                    ФОТ = g.Sum(x => x.Salary).ToString("N2"),
                    СреднийОклад = Math.Round(g.Average(x => x.Salary), 2).ToString("N2")
                })
                .OrderBy(x => x.Подразделение)
                .ToList();

        private object BuildEducation() =>
            _db.Employees.Include(e => e.Education).ToList()
                .GroupBy(e => e.Education!.Name)
                .Select(g => new { Образование = g.Key, Сотрудников = g.Count() })
                .OrderByDescending(x => x.Сотрудников)
                .ToList();

        private object BuildStaffing() =>
            _db.Employees.Include(e => e.Department).Include(e => e.Position).ToList()
                .GroupBy(e => (Dept: e.Department!.Name, Pos: e.Position!.Name))
                .Select(g => new
                {
                    Подразделение = g.Key.Dept,
                    Должность = g.Key.Pos,
                    Количество = g.Count(),
                    ФОТ = g.Sum(x => x.Salary).ToString("N2")
                })
                .OrderBy(x => x.Подразделение).ThenBy(x => x.Должность)
                .ToList();

        private static List<string> GetColumns(object data)
        {
            var list = ((IEnumerable)data).Cast<object>().ToList();
            if (list.Count == 0) return new();
            return list[0].GetType().GetProperties().Select(p => p.Name).ToList();
        }

        private static List<List<string>> GetRows(object data)
        {
            var list = ((IEnumerable)data).Cast<object>().ToList();
            return list.Select(item => item.GetType().GetProperties()
                .Select(p => p.GetValue(item)?.ToString() ?? "")
                .ToList()).ToList();
        }

        [HttpPost]
        public IActionResult ExportExcel(string type, int? employeeId)
        {
            var data = BuildReport(type, employeeId);
            if (data == null) return RedirectToAction("Index", new { type, employeeId });

            var columns = GetColumns(data);
            var rows = GetRows(data);

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Отчёт");

            for (int c = 0; c < columns.Count; c++)
                ws.Cell(1, c + 1).Value = columns[c];

            for (int r = 0; r < rows.Count; r++)
                for (int c = 0; c < rows[r].Count; c++)
                    ws.Cell(r + 2, c + 1).Value = rows[r][c];

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var bytes = stream.ToArray();

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "report.xlsx");
        }
    }
}
