using Microsoft.AspNetCore.Mvc;

namespace fluffyjohn.Controllers
{
    public class FilesController : Controller
    {
        [Route("/Files/{**dirpath}")]
        public IActionResult Index()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Redirect("~/Identity/Account/Login");
            }

            return View();
        }
    }
}
    