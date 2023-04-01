using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using fluffyjohn.Models;
using NuGet.Protocol;
using System.IO;

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

            if (fCount == 0)
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
            if (null == dirname || string.Empty == dirname)
            {
                return Redirect(Request.Headers.Referer);
            }

            foreach (var c in dirname!)
            {
                dirname = dirname!.Replace(" ", "-");
            }

            string userSubDir = ((string)Request.Headers.Referer).Split("viewfiles")[1] + "/";
            string userDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + userSubDir;

            if (Directory.Exists($"{userDir}/{dirname}") == true)
            {
                return Redirect(Request.Headers.Referer);
            }

            DirectoryInfo dirInfo = new DirectoryInfo($"{userDir}/{dirname}");
            dirInfo.Create();

            return Redirect(Request.Headers.Referer);
        }

        [Route("/delfile/{**fpath}")]
        public IActionResult DeleteFile(string? fpath)
        {
            if (PathFormatter.ValidateEntryPath(fpath) == false || User.Identity!.IsAuthenticated == false)
            {
                return Redirect("~/viewfiles/");
            }

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity.Name!) + "/" + fpath;
            string fName = Path.GetFileName(absolutePath);
            FileInfo fileInfo = new FileInfo(absolutePath);
            fileInfo.Delete();

            Response.Cookies.Append("toast-content", $"delete-success.{fName}");

            // Referer can sometimes be null, for now, just redirect to /viewfiles
            return Redirect("~/viewfiles");
        }

        [Route("~/deldir/{**dirpath}")]
        public IActionResult DeleteDirectory(string? dirpath)
        {
            if (PathFormatter.ValidateEntryPath(dirpath) == false || User.Identity!.IsAuthenticated == false)
            {
                return Redirect("~/viewfiles");
            }

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity.Name!) + "/" + dirpath;

            DirectoryInfo dirInfo = new DirectoryInfo(absolutePath);
            dirInfo.Delete(true);
            string dirName = dirInfo.Name;

            Response.Cookies.Append("toast-content", $"delete-success.{dirName}");

            // Referer can sometimes be null, for now, just redirect to /viewfiles
            return Redirect("~/viewfiles");
        }

        private void Log(string msg) { System.Diagnostics.Debug.WriteLine(msg); }

        [HttpPost]
        [Route("/renamef/")]    
        public IActionResult RenameFile([FromBody] RenameItemModel data) 
        {
            if (User.Identity!.IsAuthenticated == false) { return Redirect("~/login"); }

            var orginalPath = data.orginalpath;
            var newpath = data.newpath;

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";

            string fullOrginalPath = absolutePath + orginalPath;
            string fullNewPath = absolutePath + newpath;

            try 
            { 
                System.IO.File.Move(fullOrginalPath, fullNewPath);
            } 
            
            catch
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/406
                return StatusCode(406);
            }

            if (Request.Headers.Referer != string.Empty)
            {
                return Redirect(Request.Headers.Referer);
            }
            else 
            {
                return Redirect("~/");
            }
        }

        [HttpPost]
        [Route("~/renamed")]
        public IActionResult RenameDirectory([FromBody] RenameItemModel data) 
        {
            if (User.Identity!.IsAuthenticated == false) { return Redirect("~/login"); }

            var orginalPath = data.orginalpath;
            var newpath = data.newpath;

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";

            string fullOrginalPath = absolutePath + orginalPath;
            string fullNewPath = absolutePath + newpath;

            Log(fullOrginalPath);
            Log(fullNewPath);

            try
            {
                System.IO.Directory.Move(fullOrginalPath, fullNewPath);
            }

            catch
            {
                 https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/406
                return StatusCode(406);
            }

            if (Request.Headers.Referer != string.Empty)
            {
                return Redirect(Request.Headers.Referer);
            }
            else
            {
                return Redirect("~/");
            }
        }

        [Route("/viewcontent/{**fpath}")]
        public IActionResult ViewFileContent(string? fpath)
        {
            // Validate and get file path and name
            if (PathFormatter.ValidateEntryPath(fpath) == false || User.Identity!.IsAuthenticated == false)
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
            if (PathFormatter.ValidateEntryPath(fpath) == false || User.Identity!.IsAuthenticated == false)
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
            if (User.Identity!.IsAuthenticated == false) 
            {
                return null;
            }

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
    