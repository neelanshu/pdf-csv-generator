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
            var cssText = System.IO.File.ReadAllText(@"C:\git\pdf-csv-generator\AO.LPR.Reports\report\pdf\css\pdf.css");
            //var htmlText = System.IO.File.ReadAllText(@"C:\Users\zadmsharmane\Documents\visual studio 2015\Projects\ClassLibrary1\ClassLibrary1\new-Copy.html");

            //var formObj = new GenericLPRObjService().GenerateReportObject();
            var formObj = new GenericLPRObjService().GenerateReportObjectNew();
            var htmlText = new PdfReportGenerator().GenerateHtml(formObj);

            var cssArray = cssText.Trim().Split('}');

            var cssClassesString = string.Join("} ", cssArray);


            string pdfPath =
                @"C:\git\pdf-csv-generator\generatedpdf\latest_pdf.pdf";

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
    }
}
