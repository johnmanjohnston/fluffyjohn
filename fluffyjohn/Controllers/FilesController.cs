﻿using Microsoft.AspNetCore.Mvc;

namespace fluffyjohn.Controllers
{
    public class FilesController : Controller { 
        [Route("/ViewFiles/{**dirpath}")]
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated == false)
            {
                return Redirect("~/Login");
            }

            return View();
        }

        public async Task<IActionResult> Upload()
        {
            if (Request.Method.ToLower() != "post")
            {
                return Redirect("/ViewFiles/");
            }

            string userSubDir = ((string)Request.Headers.Referer).Split("ViewFiles")[1] + "/";
            string userDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + userSubDir;
            IFormFileCollection files = Request.Form.Files;

            int fCount = files.Count;
            for (int i = 0; i < fCount; i++)
            {
                IFormFile fl = files[i];
                using FileStream inputStream = new FileStream(userDir + fl.FileName, FileMode.Create);
                await fl.CopyToAsync(inputStream);
            }

            Response.Cookies.Append("toast-content", $"upload-success.{fCount}");
            return Redirect(Request.Headers.Referer);
        }
    }
}
    