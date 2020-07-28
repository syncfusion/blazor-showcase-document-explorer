using DocumentExplorer.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
//File Manager's base functions are available in the below namespace
using DocumentExplorer.Models.FileManager;
//File Manager's operations are available in the below namespace
using DocumentExplorer.Data;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace DocumentExplorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZipViewerController : ControllerBase
    {
        private PhysicalFileProvider operation;        
        private string tempDir;
        private string basePath;
        private string baseLocation;
        private IWebHostEnvironment _hostingEnvironment;


        public ZipViewerController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            this.basePath = _hostingEnvironment.ContentRootPath;
            this.baseLocation = this.basePath + "\\wwwroot\\";
            // Temprorary location to store content of the zip file
            // this.tempDir = this.basePath + "\\" + "tempZipStorage";
            this.tempDir = Path.GetTempPath() + "tempZipStorage";
            if (!Directory.Exists(this.tempDir))
                Directory.CreateDirectory(this.tempDir);
            this.operation = new PhysicalFileProvider();
            //this.operation.RootFolder(this.basePath + "\\wwwroot\\Files"); // Data\\Files denotes in which files and folders are available.
            this.operation.RootFolder(this.tempDir);
        }

        [Route("Root")]
        public string Root()
        {
            return this.tempDir;
        }

        // Processing the File Manager operations
        [Route("FileOperations")]
        public object FileOperations([FromBody] ReadArgs args)
        {
            try
            {
                switch (args.Action)
                {
                    // Add your custom action here
                    case "read":
                        // Path - Current path; ShowHiddenItems - Boolean value to show/hide hidden items
                        return this.operation.ToCamelCase(this.operation.GetFiles(args.Path, args.ShowHiddenItems));
                    case "search":
                    case "details":
                    case "delete":
                    case "copy":
                    case "move":
                    case "create":
                    case "rename":
                        FileManagerResponse response = new FileManagerResponse();
                        response.Error = new ErrorDetails() { Code = "401", Message = "Extract the Zip file to perform this action" };
                        return this.operation.ToCamelCase(response);
                }
                return null;
            }
            catch (IOException e)
            {
                throw e;
            }
        }
        [Route("ExtractZip")]
        public IActionResult ExtractZip([FromBody] FileManagerDirectoryContent args)
        {
            DeleteDirectoryContent();
            string zipLocation = this.baseLocation + args.Path;
            ZipFile.ExtractToDirectory(zipLocation, this.tempDir);
            return Content("Extracted");
        }

        public string Extract(string ZipPath)
        {
            try
            {
                DeleteDirectoryContent();
                string zipLocation = this.baseLocation + ZipPath;
                if (System.IO.File.Exists(zipLocation))
                {
                    ZipFile.ExtractToDirectory(zipLocation, this.tempDir);
                    return "Extracted";
                }
                else
                {
                    return "PathNotFound";
                }
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public void DeleteDirectoryContent()
        {
            string path = tempDir;
            try
            {
                string[] files = Directory.GetFiles(path);
                string[] dirs = Directory.GetDirectories(path);
                foreach (string file in files)
                {
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    System.IO.File.Delete(file);
                }
                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }
            }
            catch (IOException e)
            {
                throw e;
            }
        }
        public void DeleteDirectory(string path)
        {

            try
            {
                string[] files = Directory.GetFiles(path);
                string[] dirs = Directory.GetDirectories(path);
                foreach (string file in files)
                {
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    System.IO.File.Delete(file);
                }
                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }
                Directory.Delete(path, true);
            }
            catch (IOException e)
            {
                throw e;
            }
        }
    }
    public class ReadArgs : FileManagerDirectoryContent
    {
        public string ZipPath { get; set; }
    }
}