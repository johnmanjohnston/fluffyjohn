using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace fluffyjohn.Controllers
{
    public class FilesController : Controller { 
        [Route("/ViewFiles/{**dirpath}")]
        public IActionResult Index()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Redirect("~/Identity/Account/Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var filePath = Directory.GetCurrentDirectory() + "/UserFileStorer/";


            foreach (var formFile in Request.Form.Files)
            {
                if (formFile.Length > 0) 
                {
                    using (var inputStream = new FileStream(filePath + formFile.FileName, FileMode.Create))
                    {
                        // read file to stream
                        await formFile.CopyToAsync(inputStream);
                        // stream to byte array
                        byte[] array = new byte[inputStream.Length];
                        inputStream.Seek(0, SeekOrigin.Begin);
                        inputStream.Read(array, 0, array.Length);
                        // get file name
                        string fName = formFile.FileName;
                    }
                }
            }

                    return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = $"<html><body>Welcome{filePath}</body></html>"
            };
        }
    }
}
    