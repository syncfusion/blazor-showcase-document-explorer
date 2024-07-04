using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DocumentExplorer.Models.FileManager;
using DocumentExplorer.Data;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cors;

namespace DocumentExplorer.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAllOrigins")]
    public class TrashController : ControllerBase
    {
        private PhysicalFileProvider operation;
        private string basePath;
        private string baseLocation;

        public TrashController(IWebHostEnvironment hostingEnvironment)
        {
            this.basePath = hostingEnvironment.ContentRootPath;
            this.baseLocation = this.basePath + "\\wwwroot\\VirtualConnections\\";
            this.operation = new PhysicalFileProvider();
        }

        // Processing the File Manager operations
        [Route("FileOperations")]
        public object FileOperations([FromBody] ReadArgs args)
        {
            if (args.Path == "/Files/")
            {
                args.Path = "/";
            }
            try
            {
                switch (args.Action)
                {
                    // Add your custom action here
                    case "read":
                        // Path - Current path; ShowHiddenItems - Boolean value to show/hide hidden items
                        return this.operation.ToCamelCase(this.GetFiles());
                    case "search":
                        // Path - Current path where the search is performed; SearchString - String typed in the searchbox; CaseSensitive - Boolean value which specifies whether the search must be casesensitive
                        //return this.operation.ToCamelCase(this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive));
                        return this.operation.ToCamelCase(this.SearchFiles(args.SearchString,args.CaseSensitive));
                    case "details":
                        // Path - Current path where details of file/folder is requested; Name - Names of the requested folders
                        return this.operation.ToCamelCase(this.GetDetails(args.Data));
                    case "delete":
                        // Path - Current path where of the folder to be deleted; Names - Name of the files to be deleted
                        return this.operation.ToCamelCase(this.DeleteFiles(args.Data));
                    case "rename":
                    case "create":
                    case "move":
                    case "copy":
                        // Path - Current path of the renamed file; Name - Old file name; NewName - New file name
                        // return this.operation.ToCamelCase(this.operation.Rename(args.Path, args.Name, args.NewName));
                        FileManagerResponse response = new FileManagerResponse();
                        response.Error = new ErrorDetails() { Code = "401", Message = "Restore file to perform this action" };
                        return this.operation.ToCamelCase(response);
                }
                return null;
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public FileManagerResponse GetFiles()
        {
            string connectionId = HttpContext.Session.GetString("ConnectionId");

            if (string.IsNullOrEmpty(connectionId))
            {
                connectionId = Guid.NewGuid().ToString(); // Generate a new unique identifier
                HttpContext.Session.SetString("ConnectionId", connectionId); // Store it in session
            }
            FileManagerResponse readResponse = new FileManagerResponse();
            FileManagerDirectoryContent cwd = new FileManagerDirectoryContent();
            String fullPath = (this.baseLocation + connectionId + "/Trash");
            DirectoryInfo directory = new DirectoryInfo(fullPath);
            cwd.Name = "Trash";
            cwd.Size = 0;
            cwd.IsFile = false;
            cwd.DateModified = directory.LastWriteTime;
            cwd.DateCreated = directory.CreationTime;
            cwd.HasChild = false;
            cwd.Type = directory.Extension;
            cwd.FilterPath = "/";
            cwd.Permission = null;
            readResponse.CWD = cwd;
            string jsonPath = this.basePath + "\\wwwroot\\VirtualConnections\\" + connectionId +"\\User\\trash.json";
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            List<TrashContents> DeletedFiles = JsonConvert.DeserializeObject<List<TrashContents>>(jsonData) ?? new List<TrashContents>();
            List<FileManagerDirectoryContent> files = new List<FileManagerDirectoryContent>();
            foreach (TrashContents file in DeletedFiles)
            {
                files.Add(file.Data);
            }
            readResponse.Files = files;
            return readResponse;
        }
        public FileManagerResponse GetDetails(FileManagerDirectoryContent[] files)
        {
            string connectionId = HttpContext.Session.GetString("ConnectionId");

            if (string.IsNullOrEmpty(connectionId))
            {
                connectionId = Guid.NewGuid().ToString(); // Generate a new unique identifier
                HttpContext.Session.SetString("ConnectionId", connectionId); // Store it in session
            }
            this.operation.RootFolder(this.baseLocation + connectionId + "\\Trash");
            FileManagerResponse response;
            string[] names = new string[files.Length];
            string responseName = "";
            int index = 0;
            foreach (FileManagerDirectoryContent file in files)
            {
                names[index] = file.Id;
                index++;
                responseName = (responseName == "") ? file.Name : (responseName + ", " + file.Name);
            }
            response = this.operation.Details("/", names);
            response.Details.Name = responseName;
            response.Details.Location = "Trash";
            return response;
        }
        public FileManagerResponse DeleteFiles(FileManagerDirectoryContent[] files)
        {
            string connectionId = HttpContext.Session.GetString("ConnectionId");

            if (string.IsNullOrEmpty(connectionId))
            {
                connectionId = Guid.NewGuid().ToString(); // Generate a new unique identifier
                HttpContext.Session.SetString("ConnectionId", connectionId); // Store it in session
            }
            this.operation.RootFolder(this.baseLocation + connectionId);
            string jsonPath = this.basePath + "\\wwwroot\\VirtualConnections\\" + connectionId + "\\User\\trash.json";
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            List<FileManagerDirectoryContent> responseFiles =new List<FileManagerDirectoryContent>();
            List<TrashContents> DeletedFiles = JsonConvert.DeserializeObject<List<TrashContents>>(jsonData) ?? new List<TrashContents>();
            foreach (FileManagerDirectoryContent file in files)
            {
                TrashContents trashFile = DeletedFiles.Find(x => (x.Container.Equals(file.Id)));
                string trashPath = "/Trash/" + trashFile.Container;
                    DeleteDirectory(this.baseLocation + connectionId + trashPath);
                responseFiles.Add(trashFile.Data);
                    DeletedFiles.Remove(trashFile);
            }
            jsonData = JsonConvert.SerializeObject(DeletedFiles);
            System.IO.File.WriteAllText(jsonPath, jsonData);
            return new FileManagerResponse() { Files = responseFiles };
        }
        [Route("EmptyTrash")]
        public IActionResult EmptyTrash()
        {
            string connectionId = HttpContext.Session.GetString("ConnectionId");

            if (string.IsNullOrEmpty(connectionId))
            {
                connectionId = Guid.NewGuid().ToString(); // Generate a new unique identifier
                HttpContext.Session.SetString("ConnectionId", connectionId); // Store it in session
            }
            string jsonPath = this.basePath + "\\wwwroot\\VirtualConnections\\" + connectionId + "\\User\\trash.json";
            string jsonData ="";
            string[] dirs = Directory.GetDirectories(this.baseLocation + connectionId);
            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
            System.IO.File.WriteAllText(jsonPath, jsonData);
            return Content("");
        }

        [Route("Restore")]
        public IActionResult Restore([FromBody] FileManagerDirectoryContent[] files)
        {
            string connectionId = HttpContext.Session.GetString("ConnectionId");

            if (string.IsNullOrEmpty(connectionId))
            {
                connectionId = Guid.NewGuid().ToString(); // Generate a new unique identifier
                HttpContext.Session.SetString("ConnectionId", connectionId); // Store it in session
            }
            this.operation.RootFolder(this.baseLocation + connectionId);
            string jsonPath = this.basePath + "\\wwwroot\\VirtualConnections\\" + connectionId + "\\User\\trash.json";
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            string responseString = "";
            List<TrashContents> DeletedFiles = JsonConvert.DeserializeObject<List<TrashContents>>(jsonData) ?? new List<TrashContents>();
            foreach (FileManagerDirectoryContent file in files)
            {
                TrashContents trashFile = DeletedFiles.Find(x => (x.Container.Equals(file.Id)));
                string fileLocation = "/Files" + trashFile.Path;
                string trashPath = "/Trash/" + trashFile.Container;
                FileManagerResponse response = this.operation.Move(trashPath, fileLocation, new string[] { trashFile.Name }, new string[] { trashFile.Name }, null, null);
                if ((response.Error == null))
                {
                    DeleteDirectory(this.baseLocation + connectionId + trashPath);
                    DeletedFiles.Remove(trashFile);
                    responseString = "Restored";
                }
                else
                {
                    responseString = "Restore Failed";
                }
            }
            jsonData = JsonConvert.SerializeObject(DeletedFiles);
            System.IO.File.WriteAllText(jsonPath, jsonData);
            return Content(responseString);
        }

        public FileManagerResponse SearchFiles(string value, bool caseSensitive)
        {
            string connectionId = HttpContext.Session.GetString("ConnectionId");

            if (string.IsNullOrEmpty(connectionId))
            {
                connectionId = Guid.NewGuid().ToString(); // Generate a new unique identifier
                HttpContext.Session.SetString("ConnectionId", connectionId); // Store it in session
            }
            this.operation.RootFolder(this.baseLocation + connectionId);
            string jsonPath = this.basePath + "\\wwwroot\\VirtualConnections\\" + connectionId + "\\User\\trash.json";
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            List<TrashContents> DeletedFiles = JsonConvert.DeserializeObject<List<TrashContents>>(jsonData) ?? new List<TrashContents>();
            List<TrashContents> searchFiles = DeletedFiles.FindAll(x => new Regex(WildcardToRegex(value), (caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase)).IsMatch(x.Name));
            List<FileManagerDirectoryContent> data = new List<FileManagerDirectoryContent>();
            foreach(TrashContents file in searchFiles) { 
                data.Add(file.Data);
            }
            return new FileManagerResponse() { Files=data} ;
        }
        public virtual string WildcardToRegex(string value)
        {
            return "^" + Regex.Escape(value).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
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
    public class TrashContents
    {
        public string Container { get; set; }
        public DateTime DateDeleted { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public FileManagerDirectoryContent Data { get; set; }
    }

}