using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
//File Manager's base functions are available in the below namespace
using DocumentExplorer.Models.FileManager;
//File Manager's operations are available in the below namespace
using DocumentExplorer.Data;
using DocIO = Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using Syncfusion.Blazor.PdfViewer;
using Syncfusion.Presentation;
using Syncfusion.PresentationRenderer;
using DocumentExplorer.Models;

namespace DocumentExplorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreviewController : ControllerBase
    {
        private PhysicalFileProvider operation;
        private string basePath;
        public PreviewController(IWebHostEnvironment hostingEnvironment)
        {
            basePath = hostingEnvironment.ContentRootPath;
            operation = new PhysicalFileProvider();
            operation.RootFolder(this.basePath + "\\wwwroot\\Files"); // Data\\Files denotes in which files and folders are available.
        }

        [Route("GetPreview")]
        public string GetPreview([FromBody] FileManagerDirectoryContent args)
        {
            string baseFolder = this.basePath + "\\wwwroot\\Files";
            try
            {
                String fullPath = baseFolder + args.Path;
                string extension = Path.GetExtension(fullPath);
                Stream imageStream = null;
                if (extension == Constants.Pdf)
                {
                    try
                    {
                        FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                        PdfRenderer pdfExportImage = new PdfRenderer();
                        //Loads the PDF document 
                        pdfExportImage.Load(fileStream);
                        //Exports the PDF document pages into images
                        Bitmap[] bitmapimage = pdfExportImage.ExportAsImage(0, 0);
                        imageStream = new MemoryStream();
                        bitmapimage[0].Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                        imageStream.Position = 0;
                        pdfExportImage.Dispose();
                        fileStream.Close();
                    }
                    catch
                    {
                        imageStream = null;
                    }
                }
                else if (extension == Constants.Docx || extension == Constants.Rtf || extension == Constants.Doc || extension == Constants.Txt)
                {
                    try
                    {
                        FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                        //Loads file stream into Word document
                        DocIO.WordDocument document = new DocIO.WordDocument(fileStream, GetDocIOFormatType(extension));
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
                        Bitmap[] bitmapimage = pdfExportImage.ExportAsImage(0, 0);
                        imageStream = new MemoryStream();
                        bitmapimage[0].Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                        imageStream.Position = 0;

                        fileStream.Close();
                    }
                    catch
                    {
                        imageStream = null;
                    }
                }
                else if (extension == Constants.Pptx)
                {
                    try
                    {
                        IPresentation presentation = Presentation.Open(fullPath);
                        //Initialize PresentationRenderer for image conversion
                        presentation.PresentationRenderer = new PresentationRenderer();
                        //Convert the first slide to image
                        imageStream = presentation.Slides[0].ConvertToImage(ExportImageFormat.Png);
                        presentation.Dispose();
                    }
                    catch
                    {
                        imageStream = null;
                    }
                }
                if (imageStream != null)
                {
                    byte[] bytes = new byte[imageStream.Length];
                    imageStream.Read(bytes);
                    string base64 = Convert.ToBase64String(bytes);
                    return "data:image/png;base64, " + base64;
                }
                else
                {
                    return "Error";
                }
            }
            catch
            {
                return "Error";
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
