using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace AO.LPR.Reports
{
    public class ExcelReportGenerator
    {

        public void GenerateExcel()
        {


            string htmlFilePathAndName = @"C:\git\pdf-csv-generator\AO.LPR.Reports\TextFile.html";
            string newXlsxFilePathAndName = @"C:\git\pdf-csv-generator\generatedpdf\TextFile.xlsx";

            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xls = excel.Workbooks.Open(htmlFilePathAndName);
            xls.SaveAs(newXlsxFilePathAndName, XlFileFormat.xlOpenXMLWorkbook);
        }
    }
}
