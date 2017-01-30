using System;
using System.CodeDom;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AO.LPR.Reports;

using CsvHelper;
using CsvHelper.Configuration;
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
public class PDfBase
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

        private List<SearchEx> GetFakeSearchResultsNoPivot()
        {
            var allForms = new List<SearchEx>();

            var headerEx = new SearchEx() {FormName = "Form"};
            headerEx.AllAnswers = new List<string>();
            headerEx.AllAnswers.Add(Questions.Name);
            headerEx.AllAnswers.Add(Questions.Age);
            headerEx.AllAnswers.Add(Questions.Job); headerEx.AllAnswers.Add(Questions.Nationality);
            allForms.Add(headerEx);

            for (int i = 0; i < 10; i++)
            {
                var searchEx = new SearchEx() {FormName = "Form - " + i + 1};
                searchEx.AllAnswers = new List<string>();

                var guid = Guid.NewGuid();
                searchEx.AllAnswers.Add(guid + " - "+(i * 10 ^ i).ToString());
                searchEx.AllAnswers.Add(guid + " - " + (i * 10 ^ i).ToString());
                searchEx.AllAnswers.Add(guid + " - " + (i * 10 ^ i).ToString()); searchEx.AllAnswers.Add(guid + " - " + (i * 10 ^ i).ToString());

                allForms.Add(searchEx);
            }

            return allForms;

        }

        public List<SearchEx> GetFakeSearchResultsPivot()
        {
            var returnValue = new List<SearchEx>();
            var allForms = GetFakeSearchReportForms();

            //create headers first


            return returnValue;
        }
        public  void GenerateSearchCsvReport ()
        {
            string csvPath = generatedFolderBasePath + "latest_search_report.csv";
            if (File.Exists(csvPath))
                File.Delete(csvPath);

            using (var csv = new CsvWriter(new StreamWriter(csvPath)))
            {
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.Delimiter = ",";
                //csv.Configuration.RegisterClassMap<CsvRowItemMap>();

                var allcsvRows = GetFakeSearchResultsNoPivot();

                //allcsvRows[0].AllAnswers.ForEach(item => csv.WriteField(item));

                //csv.NextRecord();
                foreach (var item in allcsvRows)
                {
                    csv.WriteField(item.FormName);
                    foreach (var ans in item.AllAnswers)
                    {
                        csv.WriteField(ans);
                    }
                    csv.NextRecord();
                }
            }
        }

        public List<QuestionAnswer> GetRandomQuestions(int i)
        {
            var randomList = new List<QuestionAnswer>();
            if (i % 2 == 0)
            {
                randomList.Add(new QuestionAnswer() { DisplayOrder = 1, QId = 1, QText = Questions.Name, AText = "Neelanshu - " + (i * 10 ^ i) });
                randomList.Add(new QuestionAnswer() { DisplayOrder = 2, QId = 2, QText = Questions.Address, AText = "London - " + (i * 10 ^ i) });
                randomList.Add(new QuestionAnswer() { DisplayOrder = 3, QId = 3, QText = Questions.Job, AText = "Permanent - " + (i * 10 ^ i) });

                return randomList;
            }

            randomList.Add(new QuestionAnswer() { DisplayOrder = 1, QId = 4, QText = Questions.Age, AText = " " + (i * 10 ^ i) });
            randomList.Add(new QuestionAnswer() { DisplayOrder = 2, QId = 5, QText = Questions.Nationality, AText = "BI- " + (i * 10 ^ i) });
            randomList.Add(new QuestionAnswer() { DisplayOrder = 3,QId = 6, QText = Questions.Gender, AText = "Male - " + (i * 10 ^ i) });

            return randomList;
        }

        public List<SearchReportForm> GetFakeSearchReportForms()
        {
            var returnValue = new List<SearchReportForm>();
            for (int i = 1; i <= 6; i++)
            {
                var form = new SearchReportForm()
                {
                    Id = i,
                    FormName = "Form - " + i,
                    Questions = GetRandomQuestions(i)
                };

                returnValue.Add(form);
            }
            return new List<SearchReportForm>();
        }

    }


    public class SearchEx
    {
        public string FormName { get; set; }
        public List<string> AllAnswers { get; set; }
    }
    public class Questions
    {
        public const string Name = "What is your name";
        public const string Address= "Where do you live";
        public const string Job = "What is your Job";
        public const string Age= "What is your Age";
        public const string Nationality = "What is your nationality";
        public const string Gender = "What is your gender";
    }

    public class QuestionAnswer
    {
        public int DisplayOrder { get; set; }
        public int QId { get; set; }
        public string QText { get; set; }
        public string AText { get; set; }
    }

    public class SearchReportForm
    {
        public int Id { get; set; }
        public string FormName { get; set; }

        public List<QuestionAnswer> Questions { get; set; }
    }
}
