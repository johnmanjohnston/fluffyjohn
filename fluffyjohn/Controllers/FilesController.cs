﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.RegularExpressions;

using fluffyjohn.Models;
using System.Net;
using System.Web;

namespace fluffyjohn.Controllers
{
    public class FilesController : Controller {
        [Route("/viewfiles/{**dirpath}")]
        public IActionResult Index()
        {
            return !User.Identity!.IsAuthenticated ? Redirect("~/login/") : View();
        }

        #region Main
        public async Task<IActionResult> Upload()
        {
            if (Request.Method.ToLower() != "post" || Request.Headers.Referer == string.Empty 
                || !User.Identity!.IsAuthenticated)
            {
                return Redirect("~/viewfiles/");
            }

            IFormFileCollection files = Request.Form.Files;
            int fCount = files.Count;

            if (fCount == 0)
            {
                return Redirect("~/viewfiles/");
            }

            string userSubDir = WebUtility.UrlDecode(Request.Headers.Referer).Split("viewfiles")[1] + "/";
            string userRoot = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";
            string userDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + userSubDir;


            for (int i = 0; i < fCount; i++)
            {
                IFormFile fl = files[i];

                // Copy to version control dir before replacing file
                if (System.IO.File.Exists(userDir + Path.GetFileName(fl.FileName)))
                {
                    string vcDir = userRoot + "/.fluffyjohn/vc/" + userSubDir;
                    Log($"vcdir: ${vcDir}");

                    Directory.CreateDirectory(vcDir);
                    System.IO.File.Copy(userDir + Path.GetFileName(fl.FileName), vcDir + Path.GetFileName(fl.FileName), true);
                }

                using FileStream fileStream = new(userDir + Path.GetFileName(fl.FileName), FileMode.Create);
                await fl.CopyToAsync(fileStream);
            }

            Response.Cookies.Append("toast-content", $"upload-success.{fCount}");

            return Request.Headers.Referer != string.Empty ? Redirect(Request.Headers.Referer) : Redirect("~/viewfiles");
        }

        [HttpPost]
        [Route("/revert/")]
        public IActionResult Revert([FromBody] RevertModel data)
        {
            string? curPath = WebUtility.UrlDecode(data.currentFilePath);
            string? oldPath = WebUtility.UrlDecode(data.oldFilePath);

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";

            string fullCurPath = absolutePath + curPath;
            string fullOldPath = absolutePath + oldPath;

            if (!System.IO.File.Exists(fullOldPath))
            {
                return StatusCode(404); // File to replace doesn't exist. Return 404.
            }

            // Swap content of file in fullCurPath and fullOldPath
            FileInfo curInfo = new(fullCurPath);
            FileInfo oldInfo = new(fullOldPath);

            // Write old file to .fluffyjohn/temp
            if (!Directory.Exists($"{absolutePath}/.fluffyjohn/temp")) { Directory.CreateDirectory($"{absolutePath}/.fluffyjohn/temp"); }

            curInfo.CopyTo($"{absolutePath}/.fluffyjohn/temp/{curInfo.Name}", true);

            oldInfo.CopyTo(fullCurPath, true);

            // Copy temp to old location
            System.IO.File.Copy($"{absolutePath}/.fluffyjohn/temp/{curInfo.Name}", fullOldPath, true);

            // Cleanup .fluffyjohn/temp/
            FileInfo tempFileInfo = new($"{absolutePath}/.fluffyjohn/temp/{curInfo.Name}");
            tempFileInfo.Delete();

            return Ok();
        }

        [HttpPost]
        [Route("/files/newdir/")]
        public IActionResult NewDir(string? dirname)
        {
            if (!User.Identity!.IsAuthenticated) { return Redirect("~/login/"); }

            if (string.IsNullOrEmpty(dirname) || dirname[0] == '.')
            {
                return Request.Headers.Referer != string.Empty ? Redirect(Request.Headers.Referer) : StatusCode(400);
            }

            string userSubDir = WebUtility.UrlDecode(Request.Headers.Referer).Split("viewfiles")[1] + "/";
            string userDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + userSubDir;

            if (Directory.Exists($"{userDir}/{dirname}"))
            {
                return Redirect(Request.Headers.Referer);
            }

            DirectoryInfo dirInfo = new($"{userDir}/{dirname}");

            // Flag dirs containing "#" because same char is used to scroll to elements with ID
            Regex validPath = new(@"^(?!.*#)(?:[a-zA-Z]:|\\)(\\[^\\/:*?""<>|\r\n]*)+$");
                
            if (!validPath.IsMatch(dirInfo.FullName) || !dirInfo.FullName.Contains(SecurityUtils.MD5Hash(User!.Identity!.Name!)))
            {
                Response.Cookies.Append("toast-content", "invalid-dirname");

                return Request.Headers.Referer != string.Empty ? Redirect(Request.Headers.Referer) : Redirect("~/viewfiles");
            }

            try
            {
                dirInfo.Create();
            } 

            catch
            {
                Response.Cookies.Append("toast-content", "dircreate-exception");
            }

            return Request.Headers.Referer != string.Empty ? Redirect(Request.Headers.Referer) : Redirect("~/viewfiles");
        }

        [Route("/delete/")]
        public IActionResult Delete([FromBody] DeleteModel data) 
        {
            string? path = data.Path;
            bool isFile = data.IsFile;

            path = WebUtility.UrlDecode(path);

            if (string.IsNullOrEmpty(path)) 
            {
                return StatusCode(400);
            }

            if (!PathFormatter.ValidateEntryPath(path) || !User.Identity!.IsAuthenticated)
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
                    FileInfo fileInfo = new(absolutePath);
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
        public IActionResult RenameItem([FromBody] RenameItemModel data) 
        {
            if (!User.Identity!.IsAuthenticated) { return Redirect("~/login"); }

            string? orginalPath = data.OrginalPath;
            string? newPath = data.NewPath;
            bool isFile = data.IsFile;

            orginalPath = WebUtility.UrlDecode(orginalPath);
            newPath = WebUtility.UrlDecode(newPath);

            if (string.IsNullOrEmpty(orginalPath) || string.IsNullOrEmpty(newPath)) { return StatusCode(400); }

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";

            string fullOrginalPath = absolutePath + orginalPath;
            string fullNewPath = absolutePath + newPath;

            if (isFile)
            {
                FileInfo newFileInfo = new(fullNewPath);
                if (!newFileInfo.FullName.Contains(SecurityUtils.MD5Hash(User!.Identity!.Name!))) 
                {
                    return StatusCode(400);
                }
            }
            else
            {
                DirectoryInfo newDirInfo = new(fullNewPath);
                if (!newDirInfo.FullName.Contains(SecurityUtils.MD5Hash(User!.Identity!.Name!))) 
                {
                    return StatusCode(400);
                }
            }

            if (!fullNewPath.Contains(SecurityUtils.MD5Hash(User.Identity!.Name!)) || newPath.Contains('#'))
            {
                return StatusCode(400);
            }

            try
            {
                if (isFile)
                {
                    System.IO.File.Move(fullOrginalPath, fullNewPath);
                }

                else 
                {
                    Directory.Move(fullOrginalPath, fullNewPath);
                }
            } 
            
            catch
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/406
                // Not Acceptable
                return StatusCode(406);
            }

            return StatusCode(200);
        }

        [Route("/viewcontent/{**fpath}")]
        public IActionResult ViewFileContent(string? fpath)
        {
           fpath = HttpUtility.UrlDecode(fpath);

            // Validate and get file path and name
            if (!PathFormatter.ValidateEntryPath(fpath) || !User.Identity!.IsAuthenticated || string.IsNullOrEmpty(fpath))
            {
                return Request.Headers.Referer != string.Empty ? Redirect(Request.Headers.Referer) : Redirect("~/viewfiles");
            }

            FileContentResult? fData = GetFileData(fpath, false);

            return fData != null ? fData : StatusCode(404, "404 Not Found");
        }

        [Route("/downloadfile/{**fpath}")]
        public IActionResult DownloadFile(string? fpath)
        {
            fpath = HttpUtility.UrlDecode(fpath);

            if (!PathFormatter.ValidateEntryPath(fpath) || !User.Identity!.IsAuthenticated || string.IsNullOrEmpty(fpath))
            {
                return Redirect("~/viewfiles/");
            }

            FileContentResult? fData = GetFileData(fpath, true);

            return fData != null ? fData : StatusCode(404, "404 Not Found");
        }

        [Route("/copy/")]
        public IActionResult CopyItem([FromBody] CopyModel data) 
        {
            var path = data.Path;
            var isFile = data.IsFile;

            path = WebUtility.UrlDecode(path);

            // Authenticate user
            if (!User.Identity!.IsAuthenticated) { return Redirect("~/login/"); }

            // Create clipboard dir
            string userRootDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";

            if (!Directory.Exists(userRootDir + ".fluffyjohn/clipboard")) 
            {
                Directory.CreateDirectory(userRootDir + ".fluffyjohn/clipboard");
            }

            // Clear clipboard and write
            DirectoryInfo dirInfo = new(userRootDir + ".fluffyjohn/clipboard");
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
                FileInfo fInfo = new(userRootDir + path);
                if (!fInfo.Exists) { return StatusCode(404); }
                fInfo.CopyTo(userRootDir + ".fluffyjohn/clipboard/" + Path.GetFileName(fInfo.FullName), true);
            }
            else {
                DirectoryInfo dInfo = new(userRootDir + path);
                if (!dInfo.Exists) { return StatusCode(404);  }

                string dest = userRootDir + ".fluffyjohn/clipboard";
                if (!CopyDirectory(userRootDir + path, dest)) { return StatusCode(500); }
            }

            return StatusCode(200);
        }

        [Route("/paste/")]
        public IActionResult Paste([FromBody] PasteModel data) 
        {
            string targetPasteRoute = WebUtility.UrlDecode(data.Route) + "/";
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
        // ============================== SELECT ITEMS ACTION ==============================

        [Route("/selectcopy/")]
        public IActionResult SelectCopy([FromBody] SelectCopyModel data) 
        {
            string userRootDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";
            int failedCopies = 0;

            if (!Directory.Exists(userRootDir + ".fluffyjohn/clipboard"))
            {
                Directory.CreateDirectory(userRootDir + ".fluffyjohn/clipboard");
            }

            // Clear clipboard and write
            DirectoryInfo dirInfo = new(userRootDir + ".fluffyjohn/clipboard");
            foreach (var file in dirInfo.GetFiles())
            {
                file.Delete();
            }

            var dirs = Directory.GetDirectories(userRootDir + ".fluffyjohn/clipboard");

            foreach (var dir in dirs)
            {
                Directory.Delete(dir, true);
            }

            foreach (var path in data.Paths!)
            {
                var formattedPath = WebUtility.UrlDecode(path);

                // All dir paths end with "/". If the path doesn't, then it's a file
                if (formattedPath.EndsWith("/"))
                {
                    DirectoryInfo dInfo = new(userRootDir + formattedPath);

                    // If one or more directories don't exist, just continue, to
                    // not interrupt the copying for other directories
                    if (!dInfo.Exists)
                    {
                        failedCopies++;
                        continue;
                    }

                    if (!CopyDirectory(userRootDir + formattedPath, userRootDir + "/.fluffyjohn/clipboard/"))
                    {
                        failedCopies++;
                    }
                }

                else
                {
                    FileInfo fInfo = new(userRootDir + formattedPath);
                    // If one or more files are missing, don't return an error code and interrupt other 
                    // file copies, just continue
                    if (!fInfo.Exists)
                    {
                        failedCopies++;
                        continue;
                    }

                    try
                    {
                        fInfo.CopyTo(userRootDir + ".fluffyjohn/clipboard/" + Path.GetFileName(fInfo.FullName), true);
                    }

                    catch
                    {
                        failedCopies++;
                    }
                }
            }

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/207
            // 207 Multi-Status

            // Use 207 to inform browser that althought most of the transaction went
            // smoothly, some things went wrong. In this case, we inform the browser
            // how many files failed to copy, and in this case we only return 207
            // if failedCopies == 0
            return failedCopies == 0 ? StatusCode(200, failedCopies) : StatusCode(207, failedCopies);
        }

        [Route("/deletecopy/")]
        public IActionResult DeleteCopy([FromBody] DeleteCopyModel data) 
        {
            string userRootDir = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/";
            int failedDeletes = 0;

            foreach (var path in data.Paths!)
            {
                var formattedPaths = WebUtility.UrlDecode(path);

                // If path ends with "/", then it's a directory, else it's a file
                if (formattedPaths.EndsWith("/"))
                {
                    DirectoryInfo dirInfo = new(userRootDir + formattedPaths);

                    try { dirInfo.Delete(true); }
                    catch { failedDeletes++; }
                }

                else 
                {
                    FileInfo fileInfo = new(userRootDir + formattedPaths);

                    try { fileInfo.Delete(); }
                    catch { failedDeletes++; }
                }
            }

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/207
            // 207 Multi-Status

            // Use 207 to inform browser that althought most of the transaction went
            // smoothly, some things went wrong. In this case, we inform the browser
            // how many files failed to copy, and in this case we only return 207
            // if failedDeletes == 0
            return failedDeletes == 0 ? StatusCode(200) : StatusCode(207, failedDeletes);
        }

        #endregion
        #region Utility
        // ==================================== UTILITY ====================================
        private static void Log(string msg) => System.Diagnostics.Debug.WriteLine(msg);

        private static bool CopyDirectory(string source, string dest) 
        {
            if (!Directory.Exists(dest)) 
            {
                Directory.CreateDirectory(dest); 
            }

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
            if (!User.Identity!.IsAuthenticated) 
            {
                return null;
            }

            string absolutePath = Directory.GetCurrentDirectory() + "/UserFileStorer/" + SecurityUtils.MD5Hash(User.Identity!.Name!) + "/" + fpath;
            string? fname = fpath!;

            char dirSplitter = '/';
            fname = fname.Contains(dirSplitter) ? fpath!.Split(dirSplitter)[fname.Split(dirSplitter).Length - 1] : fpath;

            if (System.IO.File.Exists(absolutePath))
            {
                // Return file
                byte[] fData = System.IO.File.ReadAllBytes(absolutePath);
                new FileExtensionContentTypeProvider().TryGetContentType(fname!, out string? fmType);

                fmType ??= "application/octet-stream";

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
        #endregion
    }
}
    