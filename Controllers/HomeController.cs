using Microsoft.AspNetCore.Mvc;

namespace Kadry.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Error() => Content("Произошла ошибка на сервере.");
    }
}
