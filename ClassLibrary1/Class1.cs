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

using CsvHelper;
using CsvHelper.Configuration;
using iTextSharp.text.html.simpleparser;
using iTextSharp.tool.xml;



namespace ClassLibrary1
{

    
public sealed class CsvRowItemMap : CsvClassMap<CsvRowItem>
{
    public CsvRowItemMap()
    {
        AutoMap();
        Map(m => m. DisplayOrder).Ignore();
    }
}

public class CsvRowItem
{

    public string Number { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }

    public string ClauseRef{ get; set; }
    public string Section { get; set; }

    public int DisplayOrder { get; set; }

}
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
        //private void GenerateExcelReport()
        //{
        //    string htmlPath = generatedFolderBasePath + "latest_report_new.html";
        //    string csvPath = generatedFolderBasePath + "latest_report.csv";
        //    string excelPath = generatedFolderBasePath + "latest_report_new.xlsx";

        //    if (File.Exists(csvPath))
        //        File.Delete(csvPath);

        //    Application excel = new Application();

        //    Workbook xls1 = excel.Workbooks.Open(htmlPath);
        //    xls1.SaveAs(csvPath, XlFileFormat.xlCSVWindows);
        //    xls1.Close();
        //}

        private List<CsvRowItem> GenerateCsvObject(ReportForm obj)
        {
            var allCsvRows = new List<CsvRowItem>();

            foreach (var section  in obj.AllSections.OrderBy(x=>x.SectionOrder))
            {
                foreach (var ques in section.AllQuestions.OrderBy(x=>x.QuestionDisplayOrder))
                {
                    if (ques.Grid == null)
                    {
                        var item = new CsvRowItem()
                        {
                            Number = ques.ContainerDisplayNumberStr,
                            Answer= ques.AnswerText,
                            ClauseRef= ques.ClauseRefValue,
                            Question = ques.DisplayText,
                            Section = section.SectionName,
                            DisplayOrder = ques.QuestionDisplayOrder
                        };

                        allCsvRows.Add(item);
                    }
                    else
                    {
                        var isFirstRowAdded = false;

                        foreach (var gridRow in ques.Grid.AllRows.Where(x=>x.RowNo!=1))
                        {
                            var getAnswerText = GetFormattedAnswerText(ques.Grid.AllRows[0], gridRow);
                            var rowText = string.Empty;
                            if ((gridRow.RowNo-1)>1)
                            rowText = " Row (" + (gridRow.RowNo - 1) + ")"; 

                            var gridRowItem = new CsvRowItem()
                            {
                                Question = ques.DisplayText + rowText,
                                Answer= getAnswerText,
                                Section = section.SectionName,
                                DisplayOrder = ques.QuestionDisplayOrder
                            };

                            if (!isFirstRowAdded)
                            {
                                gridRowItem.Number = ques.ContainerDisplayNumberStr;
                                gridRowItem.ClauseRef= ques.ClauseRefValue;
                            }

                            isFirstRowAdded = true;

                            allCsvRows.Add(gridRowItem);
                        }
                    }
                }
            }
            return allCsvRows;
        }

        private string GetFormattedAnswerText(GridRow header, GridRow row)
        {
            StringBuilder answerText = new StringBuilder();

            for (int i = 0; i < row.AllCols.Count; i++)
            {
                var stringtoappend = ", ";
                if (i + 1 == row.AllCols.Count) stringtoappend = " ";
                answerText.Append(header.AllCols[i].ColText + " - " + row.AllCols[i].ColText + stringtoappend);
            }

            return answerText.ToString();

        }
        private void GenerateCsvReport(ReportForm reportContent)
        {
          string csvPath = generatedFolderBasePath + "latest_report_new.csv";
            if (File.Exists(csvPath))
                File.Delete(csvPath);

            using (var csv = new CsvWriter(new StreamWriter(csvPath)))
            {
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.Delimiter = ",";
                csv.Configuration.RegisterClassMap<CsvRowItemMap>();

                var allcsvRows = GenerateCsvObject(reportContent);

                //var headerRecord = new CsvRowItem()
                //{
                //    AnswerText = "ANSWERS",
                //    ClauseRefValue = "CLAUSEREF",
                //    Section = "SECTIONS",
                //    DisplayText = "QUESTIONS",
                //    DisplayNumber = "NUMBER"
                //};

                csv.WriteHeader<CsvRowItem>();

                foreach (var item in allcsvRows)
                {
                    csv.WriteRecord(item);
                }
            }
        }

        public void CreatePdfNewNew()
        {
            var cssText = System.IO.File.ReadAllText(@"C:\git\pdf-csv-generator\AO.LPR.Reports\report\pdf\css\pdf.css");
            var formObj = new GenericLPRObjService().GenerateReportObjectNew();
            var htmlText = new PdfReportGenerator().GenerateHtml(formObj);

            //CreateHtml(cssText,htmlText);
            GenerateCsvReport(formObj);

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
