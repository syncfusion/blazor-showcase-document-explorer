using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentExplorer.Models.FileManager;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;


namespace DocumentExplorer.Models.FileManager
{
    public  interface IPhysicalFileProvider : IFileProvider
    {        
            void RootFolder(string folderName);
        }
    
}
