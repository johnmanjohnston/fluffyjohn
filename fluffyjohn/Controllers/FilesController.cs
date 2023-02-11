using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;
using System.Text;
using fluffyjohn.Utils.Security;
using fluffyjohn.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

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
            var userDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";
            var files = Request.Form.Files;

            foreach (var formFile in files)
            {
                if (formFile.Length > 0) 
                {
                    using (var inputStream = new FileStream(userDir + formFile.FileName, FileMode.Create))
                    {
                        await formFile.CopyToAsync(inputStream);
                        byte[] array = new byte[inputStream.Length];
                        inputStream.Seek(0, SeekOrigin.Begin);
                        inputStream.Read(array, 0, array.Length);
                    }
                }
            }

            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = $"<html><body>Welcome {User.Identity.Name}</body></html>"
            };
        }
    }
}
    