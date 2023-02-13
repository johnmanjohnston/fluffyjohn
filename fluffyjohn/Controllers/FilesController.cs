using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;
using System.Text;
using fluffyjohn.Utils.Security;
using fluffyjohn.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace fluffyjohn.Controllers
{
    public class FilesController : Controller { 
        [Route("/ViewFiles/{**dirpath}")]
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated == false)
            {
                return Redirect("~/Identity/Account/Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var userSubDir = ((string)Request.Headers.Referer).Split("ViewFiles")[1] + "/";
            var userDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + userSubDir;
            var files = Request.Form.Files;

            Console.WriteLine(userDir);

            foreach (var fl in files)
            {
                if (fl.Length > 0) 
                {
                    using var inputStream = new FileStream(userDir + fl.FileName, FileMode.Create);
                    await fl.CopyToAsync(inputStream);
                }
            }

            return Redirect(Request.Headers.Referer);
        }
    }
}
    