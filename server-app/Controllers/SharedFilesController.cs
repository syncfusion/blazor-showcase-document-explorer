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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Blazor.PdfViewer;
using Syncfusion.Pdf;
using Syncfusion.Presentation;
using Syncfusion.PresentationRenderer;
using System.Drawing;
using Microsoft.AspNetCore.Cors;
using DocumentExplorer.Models;
using SkiaSharp;

namespace DocumentExplorer.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAllOrigins")]
    public class SharedFilesController : ControllerBase
    {
        private PhysicalFileProvider operation;
        private string basePath;
        public SharedFilesController(IWebHostEnvironment hostingEnvironment)
        {
            this.basePath = hostingEnvironment.ContentRootPath;
            this.operation = new PhysicalFileProvider();
            this.operation.RootFolder(this.basePath + "\\wwwroot\\SharedFiles"); // Data\\Files denotes in which files and folders are available.
        }

        // Processing the File Manager operations
        [Route("FileOperations")]
        public object FileOperations([FromBody] FileManagerFilterContent args)
        {
            switch (args.Action)
            {
                // Add your custom action here
                case "read":
                    // Path - Current path; ShowHiddenItems - Boolean value to show/hide hidden items
                    return this.operation.ToCamelCase(this.operation.GetFiles(args.Path, args.ShowHiddenItems));
                case "details":
                    // Path - Current path where details of file/folder is requested; Name - Names of the requested folders
                    return this.operation.ToCamelCase(this.operation.Details(args.Path, args.Names));
                case "create":
                    FileManagerResponse createresponse = new FileManagerResponse();
                    createresponse.Error = new ErrorDetails() { Code = "401", Message = "Restricted to perform this action" };
                    return this.operation.ToCamelCase(createresponse);
                case "search":
                    // Path - Current path where the search is performed; SearchString - String typed in the searchbox; CaseSensitive - Boolean value which specifies whether the search must be casesensitive
                    return this.operation.ToCamelCase(this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive));
                case "delete":
                case "copy":
                case "move":
                case "rename":
                    FileManagerResponse renameresponse = new FileManagerResponse();
                    renameresponse.Error = new ErrorDetails() { Code = "401", Message = "Restricted to perform this action" };
                    return this.operation.ToCamelCase(renameresponse);
            }
            return null;
        }

        [Route("Download")]
        public IActionResult Download(string downloadInput)
        {

            FileManagerFilterContent args = JsonConvert.DeserializeObject<FileManagerFilterContent>(downloadInput);
            FileManagerDirectoryContent[] items = args.Data;
            string[] names = args.Names;
            for (var i = 0; i < items.Length; i++)
            {
                names[i] = ((items[i].FilterPath + items[i].Name).Substring(1));
            }
            return operation.Download("/", names);
        }


        [Route("GetImage")]
        public IActionResult GetImage(FileManagerFilterContent args)
        {
            return this.operation.GetImage(args.Path, args.Id, false, null, null);
        }
        [Route("GetPreviewImage")]
        public IActionResult GetPreviewImage(FileManagerFilterContent args)
        {
            string baseFolder = this.basePath + "\\wwwroot\\SharedFiles";

            try
            {
                String fullPath = baseFolder + args.Path;
                string extension = Path.GetExtension(fullPath);
                Stream imageStream = null;
                if (extension == Constants.Pdf)
                {
                    FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                    PdfRenderer pdfExportImage = new PdfRenderer();
                    //Loads the PDF document 
                    pdfExportImage.Load(fileStream);
                    //Exports the PDF document pages into images
                    SkiaSharp.SKBitmap[] skBitmaps = pdfExportImage.ExportAsImage(0, 0);
                    System.Drawing.Bitmap[] bitmapImages = new System.Drawing.Bitmap[skBitmaps.Length];

                    for (int i = 0; i < skBitmaps.Length; i++)
                    {
                        using (SKImage skImage = SKImage.FromBitmap(skBitmaps[i]))
                        using (SKData skData = skImage.Encode(SKEncodedImageFormat.Png, 100))
                        using (System.IO.MemoryStream stream = new System.IO.MemoryStream(skData.ToArray()))
                        {
                            bitmapImages[i] = new System.Drawing.Bitmap(stream);
                        }
                    }
                    imageStream = new MemoryStream();
                    bitmapImages[0].Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                    imageStream.Position = 0;
                    pdfExportImage.Dispose();
                    fileStream.Close();
                }
                else if (extension == Constants.Docx || extension == Constants.Rtf || extension == Constants.Doc)
                {
                    FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                    //Loads file stream into Word document
                    WordDocument document = new WordDocument(fileStream, Syncfusion.DocIO.FormatType.Automatic);
                    fileStream.Dispose();
                    //Instantiation of DocIORenderer for Word to PDF conversion
                    DocIORenderer render = new DocIORenderer();
                    //Converts Word document into PDF document
                    PdfDocument pdfDocument = render.ConvertToPDF(document);
                    //Releases all resources used by the Word document and DocIO Renderer objects
                    render.Dispose();
                    document.Dispose();
                    //Saves the PDF file
                    MemoryStream outputStream = new MemoryStream();
                    pdfDocument.Save(outputStream);
                    outputStream.Position = 0;
                    //Closes the instance of PDF document object
                    pdfDocument.Close();

                    PdfRenderer pdfExportImage = new PdfRenderer();
                    //Loads the PDF document 
                    pdfExportImage.Load(outputStream);
                    //Exports the PDF document pages into images
                    SkiaSharp.SKBitmap[] skBitmaps = pdfExportImage.ExportAsImage(0, 0);
                    System.Drawing.Bitmap[] bitmapImages = new System.Drawing.Bitmap[skBitmaps.Length];

                    for (int i = 0; i < skBitmaps.Length; i++)
                    {
                        using (SKImage skImage = SKImage.FromBitmap(skBitmaps[i]))
                        using (SKData skData = skImage.Encode(SKEncodedImageFormat.Png, 100))
                        using (System.IO.MemoryStream stream = new System.IO.MemoryStream(skData.ToArray()))
                        {
                            bitmapImages[i] = new System.Drawing.Bitmap(stream);
                        }
                    }
                    imageStream = new MemoryStream();
                    bitmapImages[0].Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                    imageStream.Position = 0;

                    fileStream.Close();
                }
                else if (extension == Constants.Pptx)
                {
                    IPresentation presentation = Presentation.Open(fullPath);
                    //Initialize PresentationRenderer for image conversion
                    presentation.PresentationRenderer = new PresentationRenderer();
                    //Convert the first slide to image
                    imageStream = presentation.Slides[0].ConvertToImage(ExportImageFormat.Png);
                    presentation.Dispose();
                }
                FileStreamResult fileStreamResult = new FileStreamResult(imageStream, "APPLICATION/octet-stream");
                return fileStreamResult;
            }
            catch
            {
                return null;
            }
        }


        public class FileManagerFilterContent : FileManagerDirectoryContent
        {
            public string RootType { get; set; }
        }

    }
}