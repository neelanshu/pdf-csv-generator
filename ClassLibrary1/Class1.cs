using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AO.LPR.Reports;
using iTextSharp.text.html.simpleparser;
using iTextSharp.tool.xml;
using Microsoft.Office.Interop.Excel;

namespace ClassLibrary1
{
    public class Class1
    {

        string excelTemplateLocationBasePath =
               @"C:\git\pdf-csv-generator\AO.LPR.Reports\report\excel\templates\{0}.html";

        private string generatedFolderBasePath = @"C:\git\pdf-csv-generator\generated\";

        private void CreateHtml(string cssText, string htmlText)
        {
            string htmlPath = generatedFolderBasePath + "latest_report_new.html";
            
            StringBuilder htmlTemplate =
                 new StringBuilder(
                     File.ReadAllText(string.Format(excelTemplateLocationBasePath, PdfTemplates.complete_html_report)));

            htmlTemplate.Replace(Placeholders.complete_html_css, cssText);
            htmlTemplate.Replace(Placeholders.complete_html_body, htmlText);

            using (var fs = File.Open(htmlPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter s = new StreamWriter(fs))
                {
                    s.Write(htmlTemplate);
                }
            }
        }
        private void GenerateExcelReport()
        {
            string htmlPath = generatedFolderBasePath + "latest_report_new.html";
            string csvPath = generatedFolderBasePath + "latest_report_new.csv";
            string excelPath = generatedFolderBasePath + "latest_report_new.xlsx";

            if (File.Exists(excelPath))
                File.Delete(excelPath);
            
            if (File.Exists(csvPath))
                File.Delete(csvPath);

            Application excel = new Application();

            Workbook xls1 = excel.Workbooks.Open(htmlPath);
            xls1.SaveAs(csvPath, XlFileFormat.xlCSVWindows);
            xls1.Close();

            Workbook xls2 = excel.Workbooks.Open(htmlPath);
            xls2.SaveAs(excelPath, XlFileFormat.xlOpenXMLWorkbook);
            xls2.Close();
        }

        public void CreatePdfNewNew()
        {
            var cssText = System.IO.File.ReadAllText(@"C:\git\pdf-csv-generator\AO.LPR.Reports\report\pdf\css\pdf.css");
            var formObj = new GenericLPRObjService().GenerateReportObjectNew();
            var htmlText = new PdfReportGenerator().GenerateHtml(formObj);

            CreateHtml(cssText,htmlText);
            GenerateExcelReport();

            var cssArray = cssText.Trim().Split('}');
            var cssClassesString = string.Join("} ", cssArray);

            string pdfPath =generatedFolderBasePath + "latest_report.pdf";

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
