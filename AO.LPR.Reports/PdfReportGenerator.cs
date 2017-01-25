using System;
using System.IO;
using System.Linq;
using System.Text;
using AO.LPR.Reports;

public class PdfReportGenerator : IReportGenerator
{

    string _templateLocationBasePath =
           @"C:\git\pdf-csv-generator\AO.LPR.Reports\report\pdf\templates\{0}.html";
    public string GenerateHtml(ReportForm reportContent)
    {
        StringBuilder reportHtml = new StringBuilder(File.ReadAllText(string.Format(_templateLocationBasePath, PdfTemplates.report)));

        /*START - Header update and replace in report html */
        StringBuilder headerHtml = new StringBuilder(File.ReadAllText(string.Format(_templateLocationBasePath, PdfTemplates.header)));

        headerHtml.Replace(Placeholders.date, DateTime.Now.Date.ToShortDateString());
        headerHtml.Replace(Placeholders.time, DateTime.Now.ToShortTimeString());
        headerHtml.Replace(Placeholders.ref_number, reportContent.FormName + " - " + reportContent.Id);

        /*Append #1 - Header added*/
        reportHtml.Replace(Placeholders.header, headerHtml.ToString());

        /*END- Header update and replace in report html */

        /*START - Section update and replace in report html */

        StringBuilder allSections = new StringBuilder();

        foreach (var section in reportContent.AllSections)
        {

            StringBuilder sectionHtml =
                new StringBuilder(File.ReadAllText(string.Format(_templateLocationBasePath, PdfTemplates.section_table)));

            string questionTemplateName = PdfTemplates.questions_simple_tr;
            if (section.ShowClauseRef)
            {
                string clauseRefTr=  File.ReadAllText(string.Format(_templateLocationBasePath, PdfTemplates.header_clause_ref_tr));
                sectionHtml.Replace(Placeholders.clause_ref_tr,clauseRefTr);

                questionTemplateName = PdfTemplates.questions_simple_clause_ref_tr;
            }
            else
            {
                sectionHtml.Replace(Placeholders.clause_ref_tr, string.Empty);
            }

            sectionHtml.Replace(Placeholders.section_name, section.SectionName);

            var allQuestionsInSectionHtml = new StringBuilder();

            //for each question get the RIGHT tr html and add to the overall questions html
            //once thats done, repalce questions_in_Section in sections html with this BIGGG html 

            foreach (var question in section.AllQuestions.OrderBy(x => x.QuestionDisplayOrder))
            {
                allQuestionsInSectionHtml.Append(GetQuestionsHtmlContainerDisplayNumber(question, questionTemplateName));
            }

            /*Append #2 - Section creation complete - all questions*/
            sectionHtml.Replace(Placeholders.questions_in_section, allQuestionsInSectionHtml.ToString());

            allSections.Append(sectionHtml);
        }
        /*Append #3 - ADD SECTION TO REPORT */
            reportHtml.Replace(Placeholders.section, allSections.ToString());

            reportHtml.Replace(Placeholders.next_section_table, string.Empty);

            /*END- Section update and replace in report html */
            return reportHtml.ToString();
        
    }
    public string GetQuestionsHtmlContainerDisplayNumber(QuestionWithAnswer question, string templateName)
    {
        StringBuilder questionsHtml =
                 new StringBuilder(
                     File.ReadAllText(string.Format(_templateLocationBasePath, templateName)));

        var textToShowAsAnswerText = question.AnswerText;
        var textToShowAsQuestionText = question.DisplayText;
        var textToShowAsQuestionDisplayNumber = question.ContainerDisplayNumberStr;
        var textToShowAsClauseRefVal = question.ClauseRefValue;

        if (question.ShowAs == DisplayType.aschildofcontainer || question.ShowAs == DisplayType.aschildofquestion) textToShowAsQuestionDisplayNumber = string.Empty;

        if (question.ShowAs == DisplayType.grid) textToShowAsAnswerText = GetQuestionHtmlForGridType(question.Grid);

        if (string.IsNullOrEmpty(textToShowAsAnswerText) && question.ShowAs == DisplayType.grid) return string.Empty;

        questionsHtml.Replace(Placeholders.question_display_number,
            textToShowAsQuestionDisplayNumber);
        questionsHtml.Replace(Placeholders.question_text, textToShowAsQuestionText);

        questionsHtml.Replace(Placeholders.answer_text, textToShowAsAnswerText);

        questionsHtml.Replace(Placeholders.clause_ref_val, textToShowAsClauseRefVal);

        return questionsHtml.ToString(); 

    }
    public string GetQuestionHtmlForGridType(GridTable grid)
    {
        if (grid.AllRows.Count == 1 && grid.AllRows[0].RowNo == 1)
            return string.Empty;

        StringBuilder gridTableHtml =
                 new StringBuilder(
                     File.ReadAllText(string.Format(_templateLocationBasePath, PdfTemplates.grid_table)));

        StringBuilder allTrs = new StringBuilder();

        foreach (var row in grid.AllRows.OrderBy(x => x.RowNo))
        {
            StringBuilder trHtml = new StringBuilder(
                     File.ReadAllText(string.Format(_templateLocationBasePath, PdfTemplates.grid_tr)));

            StringBuilder allTds = new StringBuilder();
            var templateName = row.RowNo == 1 ? PdfTemplates.grid_td_header : PdfTemplates.grid_td;

            foreach (var col in row.AllCols.OrderBy(x => x.ColumnNo))
            {
                
                StringBuilder tdHtml = new StringBuilder(
                     File.ReadAllText(string.Format(_templateLocationBasePath, templateName)));

                tdHtml.Replace(Placeholders.grid_col_text, col.ColText);
                allTds.Append(tdHtml);
            }

            trHtml.Replace(Placeholders.grid_columns, allTds.ToString());
            allTrs.Append(trHtml);
        }

        gridTableHtml.Replace(Placeholders.grid_rows, allTrs.ToString());

        return gridTableHtml.ToString();
    }
}