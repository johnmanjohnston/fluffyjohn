using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.RegularExpressions;

using fluffyjohn.Models;

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

        #region Main
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
                using FileStream inputStream = new(userDir + fl.FileName, FileMode.Create);
                await fl.CopyToAsync(inputStream);
            }

            if (fCount != 0)
            {
                Response.Cookies.Append("toast-content", $"upload-success.{fCount}");
            }

            if (Request.Headers.Referer != string.Empty)
            {
                return Redirect(Request.Headers.Referer);
            }
            else
            {
                return Redirect("~/viewfiles");
            }
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

            DirectoryInfo dirInfo = new($"{userDir}/{dirname}");

            Regex validPath = new (@"^(?:[a-zA-Z]\:|\\)(\\[^\\/:*?""<>|\r\n]*)+$");

            if (!validPath.IsMatch(dirInfo.FullName))
            {
                Response.Cookies.Append("toast-content", "invalid-dirname");

                if (Request.Headers.Referer != string.Empty)
                { return Redirect(Request.Headers.Referer); }
                else
                { return Redirect("~/viewfiles"); }
            }

            dirInfo.Create();

            if (Request.Headers.Referer != string.Empty)
            { return Redirect(Request.Headers.Referer); }
            else
            { return Redirect("~/viewfiles"); }
        }

        [Route("/delete")]
        public IActionResult Delete([FromBody] DeleteModel data) 
        {
            string path = data.path;
            bool isFile = data.isFile;

            if (PathFormatter.ValidateEntryPath(path) == false || User.Identity!.IsAuthenticated == false)
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/401
                // Unauthorized
                return StatusCode(401);
            }

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity.Name!) + "/" + path;

            try 
            {
                if (isFile)
                {
                    FileInfo fileInfo = new FileInfo(absolutePath);
                    fileInfo.Delete();
                }
                else 
                {
                    DirectoryInfo dirInfo = new(absolutePath);
                    dirInfo.Delete(true);
                }
            }

            catch 
            {

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500
                // Internal Server Error
                return StatusCode(500);
            }

            return StatusCode(200);
        }

        [HttpPost]
        [Route("/rename/")]    
        public IActionResult RenameFile([FromBody] RenameItemModel data) 
        {
            if (User.Identity!.IsAuthenticated == false) { return Redirect("~/login"); }

            var orginalPath = data.orginalPath;
            var newpath = data.newPath;
            var isFile = data.isFile;

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";

            string fullOrginalPath = absolutePath + orginalPath;
            string fullNewPath = absolutePath + newpath;

            try 
            {
                if (isFile)
                {
                    System.IO.File.Move(fullOrginalPath, fullNewPath);
                }
                else 
                {
                    System.IO.Directory.Move(fullOrginalPath, fullNewPath);
                }
            } 
            
            catch
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/406
                // Not Acceptable
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

        [Route("/copy/")]
        public IActionResult CopyFile([FromBody] CopyModel data) 
        {
            var path = data.path;
            var isFile = data.isFile;

            // Validate
            if (User.Identity!.IsAuthenticated == false) { return Redirect("~/login/"); }

            // Create clipboard dir
            string userRootDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";

            if (!Directory.Exists(userRootDir + ".fluffyjohn/clipboard")) 
            {
                Directory.CreateDirectory(userRootDir + ".fluffyjohn/clipboard");
            }

            // Clear clipboard and write
            DirectoryInfo dirInfo = new DirectoryInfo(userRootDir + ".fluffyjohn/clipboard");
            foreach (var file in dirInfo.GetFiles())
            {
                file.Delete();
            }

            var dirs = Directory.GetDirectories(userRootDir + ".fluffyjohn/clipboard");

            foreach (var dir in dirs) 
            {
                Directory.Delete(dir, true);
            }

            if (isFile)
            {
                FileInfo fInfo = new FileInfo(userRootDir + path);
                if (false == fInfo.Exists) { return StatusCode(404); }
                fInfo.CopyTo(userRootDir + ".fluffyjohn/clipboard/" + Path.GetFileName(fInfo.FullName), true);
            }
            else {
                DirectoryInfo dInfo = new DirectoryInfo(userRootDir + path);
                if (false == dInfo.Exists) { return StatusCode(404);  }

                string dest = userRootDir + ".fluffyjohn/clipboard";
                if (CopyDirectory(userRootDir + path, dest) == false) { return StatusCode(500); }
            }

            return StatusCode(200);
        }

        [Route("/paste/")]
        public IActionResult Paste([FromBody] PasteModel data) 
        {
            string targetPasteRoute = data.route + "/";
            string userRootDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";

            try
            {
                CopyDirectory(userRootDir + "/.fluffyjohn/clipboard/", userRootDir + "/" + targetPasteRoute);
            } 
            
            catch 
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500
                // Internal Server Error
                StatusCode(500);
            }

            return StatusCode(200);
        }
        #endregion
        #region Utility
        // ==================================== UTILITY ====================================
        private void Log(string msg) { System.Diagnostics.Debug.WriteLine(msg); }

        private bool CopyDirectory(string source, string dest) 
        {
            try
            {
                foreach (string dPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dPath.Replace(source, dest));
                }

                foreach (string newFilePath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
                {
                    System.IO.File.Copy(newFilePath, newFilePath.Replace(source, dest), true);
                }

                return true;
            }

            catch { return false; }
        }   

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
                new FileExtensionContentTypeProvider().TryGetContentType(fname, out string? fmType);

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
        #endregion
    }
}
    