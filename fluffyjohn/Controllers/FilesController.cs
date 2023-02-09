using Microsoft.AspNetCore.Mvc;

namespace fluffyjohn.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
