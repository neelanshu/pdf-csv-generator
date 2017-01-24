using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.html.simpleparser;
using iTextSharp.tool.xml;

namespace ClassLibrary1
{
    public class Class1
    {
        public void CreatePdfNewNew()
        {
            var cssText = System.IO.File.ReadAllText(@"C:\Users\neelanshu\Desktop\LPR\Flattened-Lt\AO.LPR.Reports\report\pdf\css\pdf.css");
            //var htmlText = System.IO.File.ReadAllText(@"C:\Users\zadmsharmane\Documents\visual studio 2015\Projects\ClassLibrary1\ClassLibrary1\new-Copy.html");

            var formObj = new GenericLPRObjService().GenerateReportObject();
            var htmlText = new PdfReportGenerator().GenerateHtml(formObj);

            var cssArray = cssText.Trim().Split('}');

            var cssClassesString = string.Join("} ", cssArray);


            string pdfPath =
                @"C:\Users\neelanshu\Desktop\LPR\Flattened-Lt\latest_pdf.pdf";

            //string pdfPath =
              //  @"C:\Users\zadmsharmane\Documents\visual studio 2015\Projects\ClassLibrary1\ClassLibrary1\form_pdf.pdf";


            FileStream pdfStream = new System.IO.FileStream(pdfPath, System.IO.FileMode.Create,
     System.IO.FileAccess.Write, System.IO.FileShare.None);

            var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4);
            var writer = PdfWriter.GetInstance(document, pdfStream);
            document.Open();

            using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssClassesString)))
            {
                using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlText)))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlMemoryStream, cssMemoryStream);
                }
            }

            document.Close();
        }

        private void CreatePdfNew()
        {
            var cssText = System.IO.File.ReadAllText(@"C:\Users\zadmsharmane\Documents\visual studio 2015\Projects\ClassLibrary1\ClassLibrary1\new.css");

            var htmlText= System.IO.File.ReadAllText(@"C:\Users\zadmsharmane\Documents\visual studio 2015\Projects\ClassLibrary1\ClassLibrary1\new.html");


            string pdfPath =
                @"C:\Users\zadmsharmane\Documents\visual studio 2015\Projects\ClassLibrary1\ClassLibrary1\trimmed.pdf";

            FileStream pdfStream = new System.IO.FileStream(pdfPath, System.IO.FileMode.Create,
                System.IO.FileAccess.Write, System.IO.FileShare.None);

            //var memoryStream = new MemoryStream(pdfPath);
            var document = new Document();
            var writer = PdfWriter.GetInstance(document, pdfStream);
            document.Open();

            var xmlWorkerFontProvider = new XMLWorkerFontProvider();
            //xmlWorkerFontProvider.Register("http://localhost:58045/Resources/Fonts/TequilaSunset-Regular.ttf");
            //xmlWorkerFontProvider.Register("http://localhost:58045/Resources/Fonts/MfSippinOnSunshine.ttf");

            using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText)))
            {
                using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlText)))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlMemoryStream, cssMemoryStream);
                }
            }

            //var pdfContent = new PdfContent
            //{
            //    MemoryStream = memoryStream,
            //    FileName = "SomeName"
            //};
            //return pdfContent;


        }
        private void CreatePdf()
        {
           String html =
                File.ReadAllText(
                    @"C:\Users\zadmsharmane\Documents\visual studio 2015\Projects\ClassLibrary1\ClassLibrary1\Temp.HTML");



            string pdfPath =
                @"C:\Users\zadmsharmane\Documents\visual studio 2015\Projects\ClassLibrary1\ClassLibrary1\trimmed.pdf";

            using (Document doc = new Document())
            {
                using (FileStream pdfStream = new System.IO.FileStream(pdfPath, System.IO.FileMode.Create,
                    System.IO.FileAccess.Write, System.IO.FileShare.None))

                {

                    using (PdfWriter writer = PdfWriter.GetInstance(doc, pdfStream))
                    {
                        writer.CloseStream = false;
                        doc.Open();
                        doc.OpenDocument();
                        doc.NewPage();
                        if (doc.IsOpen() == true)
                        {
                            using (StringReader reader = new StringReader(html))
                            {
                                doc.Add(new Paragraph("I have a new pdf "));
                                using (HTMLWorker worker = new HTMLWorker(doc))
                                {
                                    worker.Open();
                                    worker.StartDocument();
                                    worker.NewPage();
                                    worker.Parse(reader);
                                    List<IElement> ie = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(
                                        reader, null);
                                    foreach (IElement element in ie)
                                    {
                                        doc.Add((IElement) element);
                                    }
                                    worker.EndDocument();
                                    worker.Close();
                                }
                            }
                        }
                        writer.Close();
                    }
                }

                doc.CloseDocument();
                doc.Close();
                doc.Dispose();
            }
        }
    }
}
