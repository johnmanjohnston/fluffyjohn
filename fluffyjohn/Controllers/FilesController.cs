using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.StaticFiles;
using System.Web;

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

        [Route("/viewcontent/{**fpath}")]
        public IActionResult ViewFileContent(string? fpath)
        {
            // Validate and get file path and name
            if (fpath == string.Empty)
            {
                return Redirect("~/ViewFiles/");
            }

            FileContentResult? fData = GetFileData(fpath, false);


            if (fData != null) { return fData; }
            else { return Content("Not found"); }
        }

        [Route("/DownloadFile/{**fpath}")]
        public IActionResult DownloadFile(string? fpath)
        {
            FileContentResult? fData = GetFileData(fpath, true);

            if (fData != null) { return fData; }
            else { return Content("Not found"); }
        }

        // Utility
        private FileContentResult? GetFileData(string? fpath, bool download = false)
        {
            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + fpath;
            string fname = fpath!;

            if (fname.Contains('/'))
            {
                fname = fpath!.Split('/')[fname.Split('/').Length - 1];
            }

            else
            {
                fname = fpath!;
            }

            if (System.IO.File.Exists(absolutePath))
            {
                // Return file
                byte[] fData = System.IO.File.ReadAllBytes(absolutePath);
                string? fmType;
                new FileExtensionContentTypeProvider().TryGetContentType(fname, out fmType);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = fname,
                    Inline = !download,
                };

                Response.Headers.Append("Content-Disposition", cd.ToString());
                return File(fData, fmType!);
            }

            return null;
        }
    }
}
    