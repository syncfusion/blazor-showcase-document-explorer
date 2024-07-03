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
using System.IO;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Cors;

namespace DocumentExplorer.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAllOrigins")]
    public class FileManagerController : ControllerBase
    {
        private PhysicalFileProvider operation;
        private string basePath;
        string root = "wwwroot\\Files";
        string rootPath;
        string virtualConnection;
        public FileManagerController(IWebHostEnvironment hostingEnvironment)
        {
            this.basePath = hostingEnvironment.ContentRootPath;
            this.operation = new PhysicalFileProvider();
            if (basePath.EndsWith("\\"))
                this.rootPath = this.basePath + this.root;
            else
                this.rootPath = this.basePath + "\\" + this.root;
            this.operation.RootFolder(rootPath);
        }
        // Processing the File Manager operations
        [Route("FileOperations")]
        public object FileOperations([FromBody] FileManagerCustomContent args)
        {

            args.RootType = HttpContext.Request.Headers["RootType"];
            string connectionId = HttpContext.Session.GetString("ConnectionId");
            if (args.Path == "/Files/")
            {
                args.Path = "/";
            }
            if (string.IsNullOrEmpty(connectionId))
            {
                connectionId = Guid.NewGuid().ToString(); // Generate a new unique identifier
                HttpContext.Session.SetString("ConnectionId", connectionId); // Store it in session
            }

            if (basePath.EndsWith("\\"))
            {
                virtualConnection = this.basePath + "wwwroot\\VirtualConnections";
            }
            else
            {
                virtualConnection = this.basePath + "\\wwwroot\\VirtualConnections";
            }
            DateTime currentTime = DateTime.Now;
            DateTime deletionThreshold = currentTime.AddHours(-24);
            DirectoryInfo virtualDirectoryInfo = new DirectoryInfo(virtualConnection);

            if (Directory.Exists(virtualConnection) && virtualDirectoryInfo.LastWriteTime <= deletionThreshold)
            {
                Directory.Delete(virtualConnection, true);
            }
            if (!Directory.Exists(virtualConnection))
            {
                //Create virtual  root directory
                Directory.CreateDirectory(virtualConnection);
            }
            string userID = virtualConnection + "\\" + connectionId + "\\Files";
            string virtualUser = virtualConnection + "\\" + connectionId + "\\User";
            string virtualTrash = virtualConnection + "\\" + connectionId + "\\Trash";
            if (!Directory.Exists(userID))
            {
                Directory.CreateDirectory(userID);
                Directory.CreateDirectory(virtualUser);
                Directory.CreateDirectory(virtualTrash);
                CopyFolder(rootPath, userID);
                CopyFolder(this.basePath + "\\wwwroot\\User", virtualUser);
                CopyFolder(this.basePath + "\\wwwroot\\Trash", virtualTrash);
            }

            //Set user directory as root
            this.operation.RootFolder(userID);

            if (args.Action == "delete" || args.Action == "rename")
            {
                if ((args.TargetPath == null) && (args.Path == ""))
                {
                    FileManagerResponse response = new FileManagerResponse();
                    response.Error = new ErrorDetails { Code = "401", Message = "Restricted to modify the root folder." };
                    return this.operation.ToCamelCase(response);
                }
            }

            switch (args.Action)
            {
                // Add your custom action here
                case "read":
                    if ((args.RootType != null) && ((args.RootType == "Recent")))
                    {
                        FileManagerResponse result1 = this.operation.Search(args.Path, "*", args.ShowHiddenItems, false);
                        return FilterRecentFiles(result1);
                    }
                    else
                    {
                        return this.operation.ToCamelCase(this.operation.GetFiles(args.Path, args.ShowHiddenItems));
                    }
                case "delete":

#if Publish
                    FileManagerResponse deleteResponse = new FileManagerResponse();
                    deleteResponse.Error = new ErrorDetails() { Code = "401", Message = "Restricted to perform this action" };
                    return this.operation.ToCamelCase(deleteResponse);
#else
                    FileManagerDirectoryContent[] items1 = args.Data;
                    string[] names1 = args.Names;
                    return this.operation.ToCamelCase(MoveToTrash(args.Data, connectionId, virtualConnection, virtualUser));
#endif
                case "copy":
                case "move":
                    FileManagerResponse response = new FileManagerResponse();
                    response.Error = new ErrorDetails() { Code = "401", Message = "Restricted to perform this action" };
                    return this.operation.ToCamelCase(response);
                case "details":
                    if ((args.RootType != null) && ((args.RootType == "Recent")))
                    {
                        FileManagerDirectoryContent[] items = args.Data;
                        string[] names = args.Names;
                        for (var i = 0; i < items.Length; i++)
                        {
                            names[i] = ((items[i].FilterPath + items[i].Name).Substring(1));
                        }
                        return this.operation.ToCamelCase(this.operation.Details("/", names, args.Data));
                    }
                    else
                    {
                        return this.operation.ToCamelCase(this.operation.Details(args.Path, args.Names, args.Data));
                    }
                case "create":
                    // Path - Current path where the folder is to be created; Name - Name of the new folder
                    return this.operation.ToCamelCase(this.operation.Create(args.Path, args.Name));
                case "search":
                    // Path - Current path where the search is performed; SearchString - String typed in the searchbox; CaseSensitive - Boolean value which specifies whether the search must be casesensitive                    
                    if ((args.RootType != null) && ((args.RootType == "Recent")))
                    {
                        FileManagerResponse result1 = this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive);
                        return FilterRecentFiles(result1);
                    }
                    else
                    {
                        return this.operation.ToCamelCase(this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive));
                    }
                case "rename":
                    // Path - Current path of the renamed file; Name - Old file name; NewName - New file name
                    if ((args.RootType != null) && (args.RootType == "Recent"))
                    {
                        var items = args.Data;
                        var name = ((items[0].FilterPath + items[0].Name).Substring(1));
                        var newName = ((items[0].FilterPath + args.NewName).Substring(1));
                        return this.operation.ToCamelCase(this.operation.Rename("/", name, newName));
                    }
                    else
                    {
                        return this.operation.ToCamelCase(this.operation.Rename(args.Path, args.Name, args.NewName));
                    }
            }
            return null;
        }
        public FileManagerResponse FilterRecentFiles(FileManagerResponse result)
        {
            IEnumerable<FileManagerDirectoryContent> allFiles = (result.Files);
            allFiles = allFiles?.Where(item => item.DateModified.AddDays(5).CompareTo(DateTime.Now) != -1 && item.IsFile == true);
            result.Files = allFiles;
            return result;
        }
        public FileManagerResponse MoveToTrash(FileManagerDirectoryContent[] dataArray, string userId, String virtualConnection, string virtualUser)
        {
            string jsonPath = virtualUser + "\\trash.json";
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            List<TrashContents> DeletedFiles = JsonConvert.DeserializeObject<List<TrashContents>>(jsonData) ?? new List<TrashContents>();
            PhysicalFileProvider trashOperation = new PhysicalFileProvider();
            string root = virtualConnection + "\\" + userId;
            trashOperation.RootFolder(root);
            List<FileManagerDirectoryContent> deletedFiles = new List<FileManagerDirectoryContent>();
            foreach (FileManagerDirectoryContent data in dataArray)
            {
                string fileLocation = "/Files" + data.FilterPath;
                DateTime deleteTime = DateTime.Now;
                string container = deleteTime.ToFileTimeUtc().ToString();
                string trashPath = "/Trash/" + container;
                Directory.CreateDirectory(root + trashPath);
                FileManagerResponse response = trashOperation.Move(fileLocation, trashPath, new string[] { data.Name }, null, null, null);
                if ((response.Error == null))
                {
                    TrashContents deletedFile = new TrashContents()
                    {
                        Container = container,
                        Data = data,
                        DateDeleted = deleteTime,
                        Name = data.Name,
                        Path = data.FilterPath
                    };
                    deletedFile.Data.DateModified = deletedFile.DateDeleted;
                    deletedFile.Data.Id = deletedFile.Container;
                    DeletedFiles.Add(deletedFile);
                    deletedFiles.Add(response.Files.First());
                }
            }
            jsonData = JsonConvert.SerializeObject(DeletedFiles);
            System.IO.File.WriteAllText(jsonPath, jsonData);
            return new FileManagerResponse() { Files = deletedFiles };
        }
        [Route("Upload")]
        public IActionResult Upload(string path, IList<IFormFile> uploadFiles, string action)
        {
#if Publish
            //Restrict the upload functionality for publish settings
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.StatusCode = 403;
            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "The upload functionality is restricted in this online demo. To test this demo application with upload functionality, please download the source code from the GitHub location (https://github.com/syncfusion/blazor-showcase-document-explorer) and run it.";
            return Content("The upload functionality is restricted in this online demo. To test this demo application with upload functionality, please download the source code from the GitHub location (https://github.com/syncfusion/blazor-showcase-document-explorer) and run it.");
#else
            string connectionId = HttpContext.Session.GetString("ConnectionId");

            if (string.IsNullOrEmpty(connectionId))
            {
                connectionId = Guid.NewGuid().ToString(); // Generate a new unique identifier
                HttpContext.Session.SetString("ConnectionId", connectionId); // Store it in session
            }
            FileManagerResponse uploadResponse;
            PhysicalFileProvider uploadOperation = new PhysicalFileProvider();
            string basePath = this.basePath + "\\wwwroot\\VirtualConnections\\" + connectionId;
            string userID = this.basePath + "wwwroot\\VirtualConnections\\" + connectionId + "\\Files";
            uploadOperation.RootFolder(userID);
            uploadResponse = operation.Upload(path, uploadFiles, action, basePath, null);
            if (uploadResponse.Error != null)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = Convert.ToInt32(uploadResponse.Error.Code);
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = uploadResponse.Error.Message;
            }
            return Content("");
#endif
        }

        [Route("Download")]
        public IActionResult Download(string downloadInput)
        {
            FileManagerDirectoryContent args = JsonConvert.DeserializeObject<FileManagerDirectoryContent>(downloadInput);
            FileManagerDirectoryContent[] items = args.Data;
            string[] names = args.Names;
            for (var i = 0; i < items.Length; i++)
            {
                names[i] = ((items[i].FilterPath + items[i].Name).Substring(1));
            }
            return operation.Download("/", names);
        }

        [Route("ToggleStarred")]
        public IActionResult ToggleStarred([FromBody] FileManagerCustomContent args)
        {
            string connectionId = HttpContext.Session.GetString("ConnectionId");

            if (string.IsNullOrEmpty(connectionId))
            {
                connectionId = Guid.NewGuid().ToString(); // Generate a new unique identifier
                HttpContext.Session.SetString("ConnectionId", connectionId); // Store it in session
            }
            string basePath = this.basePath + "\\wwwroot\\VirtualConnections\\" + connectionId +"\\User";
            string jsonPath = basePath + "\\star.json";
            StreamReader reader = new StreamReader(jsonPath);
            string jsonData = reader.ReadToEnd();
            reader.Dispose();
            List<string> starredFiles = JsonConvert.DeserializeObject<List<string>>(jsonData) ?? new List<string>();
            string path = args.Path.Replace(Path.DirectorySeparatorChar, '/');
            if (args.Starred && !starredFiles.Contains(path))
            {
                starredFiles.Add(path);
            }
            else if (!args.Starred && starredFiles.Contains(path))
            {
                starredFiles.Remove(path);
            }
            jsonData = JsonConvert.SerializeObject(starredFiles);
            System.IO.File.WriteAllText(jsonPath, jsonData);
            return Content("");
        }

        [Route("GetImage")]
        public IActionResult GetImage(FileManagerDirectoryContent args)
        {
            return this.operation.GetImage(args.Path, args.Id, false, null, null);
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }

        public class FileManagerCustomContent : FileManagerDirectoryContent
        {
            public string RootType { get; set; }

            //Custom parameter inidicating starred files
            public bool Starred { get; set; }
        }
        public class FileResponse
        {
            public FileManagerDirectoryContent CWD { get; set; }

            public IEnumerable<FileManagerCustomContent> Files { get; set; }

            public ErrorDetails Error { get; set; }

            public FileDetails Details { get; set; }

        }
        public class TrashContents
        {
            public string Container { get; set; }
            public DateTime DateDeleted { get; set; }
            public string Path { get; set; }
            public string Name { get; set; }
            public FileManagerDirectoryContent Data { get; set; }
        }
    }
}