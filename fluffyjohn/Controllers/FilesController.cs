using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace fluffyjohn.Controllers
{
    public class FilesController : Controller { 
        [Route("/viewfiles/{**dirpath}")]
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated == false)
            {
                return Redirect("~/login");
            }

            return View();
        }

        public async Task<IActionResult> Upload()
        {
            if (Request.Method.ToLower() != "post")
            {
                return Redirect("~/viewfiles/");
            }

            string userSubDir = ((string)Request.Headers.Referer).Split("viewfiles")[1] + "/";
            string userDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + userSubDir;
            IFormFileCollection files = Request.Form.Files;

            int fCount = files.Count;

            if (fCount  == 0)
            {
                return Redirect("~/viewfiles/");
            }

            for (int i = 0; i < fCount; i++)
            {
                IFormFile fl = files[i];
                using FileStream inputStream = new FileStream(userDir + fl.FileName, FileMode.Create);
                await fl.CopyToAsync(inputStream);
            }

            if (fCount != 0)
            {
                Response.Cookies.Append("toast-content", $"upload-success.{fCount}");
            }

            return Redirect(Request.Headers.Referer);
        }

        [HttpPost]
        [Route("/files/newdir/")]
        public IActionResult NewDir(string? dirname) 
        {
            string userSubDir = ((string)Request.Headers.Referer).Split("viewfiles")[1] + "/";
            string userDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + userSubDir;

            DirectoryInfo dirInfo = new DirectoryInfo($"{userDir}/{dirname}");
            dirInfo.Create();

            return Redirect(Request.Headers.Referer);
        }

        [Route("/delfile/{**fpath}")]
        public IActionResult DeleteFile(string? fpath)
        {
            if (PathFormatter.ValidateEntryPath(fpath) == false)
            {
                return Redirect("~/viewfiles/");
            }

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + fpath;
            string fName = Path.GetFileName(absolutePath);
            FileInfo fileInfo = new FileInfo(absolutePath);
            fileInfo.Delete();

            Response.Cookies.Append("toast-content", $"delete-success.{fName}");

            return Redirect(Request.Headers.Referer);
        }

        [Route("~/deldir/{**dirpath}")]
        public IActionResult DeleteDirectory(string? dirpath)
        {
            if (PathFormatter.ValidateEntryPath(dirpath) == false)
            {
                return Redirect("~/viewfiles");
            }

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + dirpath;
            
            DirectoryInfo dirInfo = new DirectoryInfo(absolutePath);
            dirInfo.Delete(true);
            string dirName = dirInfo.Name;

            Response.Cookies.Append("toast-content", $"delete-success.{dirName}");

            return Redirect(Request.Headers.Referer);
        }

        [Route("/viewcontent/{**fpath}")]
        public IActionResult ViewFileContent(string? fpath)
        {
            // Validate and get file path and name
            if (PathFormatter.ValidateEntryPath(fpath) == false)
            {
                return Redirect("~/viewfiles/");
            }

            FileContentResult? fData = GetFileData(fpath, false);

            if (fData != null) { return fData; }
            else { return Content("Not found"); }
        }

        [Route("/downloadfile/{**fpath}")]
        public IActionResult DownloadFile(string? fpath)
        {
            if (PathFormatter.ValidateEntryPath(fpath) == false)
            {
                return Redirect("~/viewfiles/");
            }

            FileContentResult? fData = GetFileData(fpath, true);

            if (fData != null) { return fData; }
            else { return Content("Not found"); }
        }

        // Utility
        private FileContentResult? GetFileData(string? fpath, bool download = false)
        {
            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + fpath;
            string fname = fpath!;

            char dirSplitter = '/';
            if (fname.Contains(dirSplitter))
            {
                fname = fpath!.Split(dirSplitter)[fname.Split(dirSplitter).Length - 1];
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
                    Inline = download == false,
                };

                Response.Headers.Append("Content-Disposition", cd.ToString());
                return File(fData, fmType!);
            }

            return null;
        }
    }
}
    