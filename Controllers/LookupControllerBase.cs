using Kadry.Data;
using Kadry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kadry.Controllers
{
    // Общий код для Подразделений, Должностей, Профессий и Образования:
    // список, добавление, редактирование, удаление (с проверкой ссылок).
    public abstract class LookupControllerBase<TEntity> : Controller
        where TEntity : class, ILookupEntity, new()
    {
        protected readonly AppDbContext Db;

        protected LookupControllerBase(AppDbContext db) => Db = db;

        protected abstract DbSet<TEntity> Set { get; }
        protected abstract string Title { get; }
        protected abstract string ControllerName { get; }
        protected abstract bool HasEmployees(int id);

        public IActionResult Index(int? editId)
        {
            var vm = new LookupIndexViewModel
            {
                Title = Title,
                ControllerName = ControllerName,
                Items = Set.OrderBy(x => x.Name)
                    .Select(x => new LookupItem { Id = x.Id, Name = x.Name })
                    .ToList(),
                EditId = editId,
                EditName = editId == null ? "" : (Set.Find(editId.Value)?.Name ?? "")
            };
            return View("~/Views/Shared/LookupIndex.cshtml", vm);
        }

        [HttpPost]
        public IActionResult Save(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Название не может быть пустым.";
                return RedirectToAction("Index");
            }

            if (id == 0)
            {
                Set.Add(new TEntity { Name = name.Trim() });
            }
            else
            {
                var entity = Set.Find(id);
                if (entity != null) entity.Name = name.Trim();
            }

            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HasEmployees(id))
            {
                TempData["Error"] = "Нельзя удалить: запись используется в данных сотрудников.";
                return RedirectToAction("Index");
            }

            var entity = Set.Find(id);
            if (entity != null)
            {
                Set.Remove(entity);
                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }

    public class LookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class LookupIndexViewModel
    {
        public string Title { get; set; } = "";
        public string ControllerName { get; set; } = "";
        public List<LookupItem> Items { get; set; } = new();
        public int? EditId { get; set; }
        public string EditName { get; set; } = "";
    }
}
