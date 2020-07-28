using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
//File Manager's base functions are available in the below namespace
using DocumentExplorer.Models.FileManager;
using Syncfusion.Blazor.DocumentEditor;
using DocIO = Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using Syncfusion.Blazor.PdfViewer;
using DocumentExplorer.Models;

namespace DocumentExplorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentEditorController : ControllerBase
    {   
        private string basePath;
        private string baseLocation;
        IWebHostEnvironment _hostingEnvironment;
        public DocumentEditorController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            this.basePath = _hostingEnvironment.ContentRootPath;
            this.baseLocation = this.basePath + "\\wwwroot\\";
        }

        [Route("Import")]
        public string[] Import([FromBody] FileManagerDirectoryContent args)
        {
            string fileLocation = this.baseLocation + args.Path.Replace("/", "\\");
            List<string> returnArray = new List<string>();
            using (FileStream fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read))
            {
                WordDocument document = WordDocument.Load(fs, GetImportFormatType(Path.GetExtension(fileLocation).ToLower()));
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                document.Dispose();
                returnArray.Add(json);
                return ConvertToImages(fs, returnArray, GetDocIOFormatType(Path.GetExtension(args.Path).ToLower())).ToArray();
            }            

        }
        private List<string> ConvertToImages(FileStream fs, List<string> returnStrings, Syncfusion.DocIO.FormatType type)
        {
            DocIO.WordDocument wd = new DocIO.WordDocument(fs, type);
            //Instantiation of DocIORenderer for Word to PDF conversion
            DocIORenderer render = new DocIORenderer();
            //Converts Word document into PDF document
            PdfDocument pdfDocument = render.ConvertToPDF(wd);
            //Releases all resources used by the Word document and DocIO Renderer objects
            render.Dispose();
            wd.Dispose();
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
            Bitmap[] bitmapimage = pdfExportImage.ExportAsImage(0, pdfExportImage.PageCount-1);
            foreach (Bitmap bitmap in bitmapimage)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    returnStrings.Add("data:image/png;base64," + Convert.ToBase64String(ms.ToArray()));
                }
            }
            return returnStrings;
        }
        [Route("OpenFromZip")]
        public string[] OpenFromZip([FromBody] FileManagerDirectoryContent args)
        {
            List<string> returnArray = new List<string>();
            using (FileStream fs = new FileStream(args.Path, FileMode.Open, FileAccess.Read))
            {
                WordDocument document = WordDocument.Load(fs, GetImportFormatType(Path.GetExtension(args.Path).ToLower()));
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                document.Dispose();
                returnArray.Add(json);
                return ConvertToImages(fs, returnArray, GetDocIOFormatType(Path.GetExtension(args.Path).ToLower())).ToArray();
            }

        }
               
        private ImportFormatType GetImportFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("DocumentEditor does not support this file format.");
            switch (format.ToLower())
            {
                case Constants.Dotx:
                case Constants.Docx:
                case Constants.Docm:
                case Constants.Dotm:
                    return ImportFormatType.Docx;
                case Constants.Dot:
                case Constants.Doc:
                    return ImportFormatType.Doc;
                case Constants.Rtf:
                    return ImportFormatType.Rtf;
                case Constants.Txt:
                    return ImportFormatType.Txt;
                case Constants.Xml:
                    return ImportFormatType.WordML;
                case Constants.Html:
                    return ImportFormatType.Html;
                default:
                    throw new NotSupportedException("DocumentEditor does not support this file format.");
            }
        }

        private Syncfusion.DocIO.FormatType GetDocIOFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("DocumentEditor does not support this file format.");
            switch (format.ToLower())
            {
                case Constants.Dotx:
                case Constants.Docx:
                case Constants.Docm:
                case Constants.Dotm:
                    return Syncfusion.DocIO.FormatType.Docx;
                case Constants.Dot:
                case Constants.Doc:
                    return Syncfusion.DocIO.FormatType.Doc;
                case Constants.Rtf:
                    return Syncfusion.DocIO.FormatType.Rtf;
                case Constants.Txt:
                    return Syncfusion.DocIO.FormatType.Txt;
                case Constants.Xml:
                    return Syncfusion.DocIO.FormatType.WordML;
                case Constants.Html:
                    return Syncfusion.DocIO.FormatType.Html;
                default:
                    throw new NotSupportedException("DocumentEditor does not support this file format.");
            }
        }
    }
}