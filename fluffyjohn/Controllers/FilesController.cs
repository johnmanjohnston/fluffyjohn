using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;
using System.Text;
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
        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            if (Request.Method.ToLower() == "get")
            {
                return Redirect("/ViewFiles/");
            }

            string userSubDir = ((string)Request.Headers.Referer).Split("ViewFiles")[1] + "/";
            string userDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + userSubDir;
            IFormFileCollection files = Request.Form.Files;

            foreach (IFormFile fl in files)
            {
                if (fl.Length > 0)
                {
                    using FileStream inputStream = new FileStream(userDir + fl.FileName, FileMode.Create);
                    await fl.CopyToAsync(inputStream);
                }
            }

            return Redirect(Request.Headers.Referer);
        }
    }
}
    