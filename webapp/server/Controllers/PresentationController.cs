using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using DocumentExplorer.Models.FileManager;
using Syncfusion.Pdf;
using Syncfusion.Presentation;
using Syncfusion.PresentationRenderer;

namespace DocumentExplorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresentationController : ControllerBase
    {
        private string basePath;
        private string baseLocation;
        private IWebHostEnvironment _hostingEnvironment;
        public PresentationController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            basePath = _hostingEnvironment.ContentRootPath;
            baseLocation = basePath + "\\wwwroot\\";
        }

        [Route("ConvertToPDF")]
        public string[] ConvertToPDF([FromBody] FileManagerDirectoryContent args)
        {
            string fileLocation = this.baseLocation + args.Path.Replace("/", "\\");
            // If document get open from zip file, we have maintained the extracted document path in TargetPath property.
            if (args.TargetPath != null)
                fileLocation = args.TargetPath;
            List<string> returnArray = new List<string>();
            using FileStream fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read);
            //Open the existing presentation
            IPresentation presentation = Syncfusion.Presentation.Presentation.Open(fs);
            //Convert the PowerPoint document to PDF document.
            PdfDocument pdfDocument = PresentationToPdfConverter.Convert(presentation);
            //Save the document as a stream and retrun the stream
            MemoryStream stream = new MemoryStream();
            //Save the created PowerPoint document to MemoryStream
            pdfDocument.Save(stream);
            stream.Position = 0;
            returnArray.Add("data:application/pdf;base64," + Convert.ToBase64String(stream.ToArray()));
            //Dispose the document objects.
            presentation.Dispose();
            pdfDocument.Dispose();
            stream.Dispose();
            return returnArray.ToArray();

        }
    }
}