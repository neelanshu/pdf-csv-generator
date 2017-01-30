using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AO.LPR.Reports
{
    public interface IReportGenerator
    {
        string GenerateHtml(ReportForm reportContent);
    }


    public interface IReportTemplate
    {

    }

    public enum ReportType
    {
        pdf,
        excel,
        searchexcel
    }


    public static class PdfTemplates
    {
        public const string complete_html_report= "complete-html-report";
        public const string section_table = "section-table";
        public const string section_table_with_clause_ref = "section-table-with-clause-ref";
        public const string questions_simple_tr = "questions-simple-tr";
        public const string questions_simple_clause_ref_tr = "questions-simple-clause-ref-tr";
        public const string questions_simple_tr_updated_col_width= "questions-simple-tr-with-updated-col-width";
        public const string questions_container_clause_ref_tr = "questions-container-clause-ref-tr";
        public const string questions_clause_ref_table = "questions-clause-ref_table";
        public const string questions_complex = "questions-complex";
        public const string containers = "containers";
        public const string header = "header";
        public const string report = "report";
        public const string grid_table = "question-grid-table";
        public const string grid_tr = "question-grid-tr";
        public const string grid_td = "question-grid-td";
        public const string grid_td_header = "question-grid-td-header";
        public const string header_clause_ref_tr = "header-clause-ref-tr";
        
    }



    public static class Placeholders
    {
        public const string header = "{ph_content_header}";
        public const string date = "{ph_content_date}";
        public const string time = "{ph_content_time}";
        public const string ref_number= "{ph_ref_number}";
        public const string section = "{ph_content_section}";
        public const string section_name = "{ph_section_name}";
        public const string next_section_table = "{ph_next_section_table}";
        public const string questions_in_section= "{ph_questions_in_section}";
        public const string question_display_number = "{ph_question_display_number}";
        public const string question_text = "{ph_question_text}";
        public const string answer_text = "{ph_answer_text}";
        public const string grid_col_text = "{ph_grid_col_text}";
        public const string grid_columns = "{ph_grid_columns}";
        public const string grid_rows= "{ph_grid_rows}";
        public const string header_class = "subtblheader";
        public const string complete_html_css= "{ph_css}";
        public const string complete_html_body = "{ph_body}";
        public const string clause_ref_tr = "{ph_clause_ref_tr}";
        public const string clause_ref_val = "{ph_clause_ref_val}";

    }

}

public class ContainerNew
    {
    /// <summary>
    /// Order by DisplayNumberDecimal after ordering by Section Order 
    /// </summary>
        public float DisplayNumberDecimal
        {
            get
            {
                float v;
                float.TryParse(DisplayNumber, out v);

                return v;
            }
        }
        public string DisplayNumber { get; set; }
        public string ContainerDisplayText { get; set; }
        public int ContainerId { get; set; }
        public bool IsUniqueContainer 
        {
            get { return string.IsNullOrEmpty(ContainerDisplayText); }
        }
        public bool GroupAllQuestionsUnderThisContainer { get { return ContainerAsParent; } }
        public bool ContainerAsParent { get { return !IsUniqueContainer; } }
        public bool ShowChildren { get; set; }
        public List<QuestionNew> Questions { get; set; }
}


//if questionType GRID AND isnotdefaultcontainer

//-- mark question obj -- usecontainertext as question text flag
//-- grid columns = childquestion.count
//-- order = take QUESTION TEXT AND display order

//if questiontype NOT GRID AND isnotdefaultcontainer 

//--MARK QUESTION OBJ -- USECOTNAINERTEXT AS QUESTIONTEXT FLAG
//-- showaschildquestions = true
//-- order = take question text and display order

    public class GridCol
{
    public string ColText { get; set; }
    public int ColumnNo { get; set; }
}
public class GridRow
{
    public int RowNo { get; set; }
    public List<GridCol> AllCols{ get; set; }
}
public class GridTable
{
    public List<GridRow> AllRows
    {
        get;set;
    }
}
public class QuestionNew
{
    public string DisplayText { get; set; } //either question or container text 
    public int QuestionId { get; set; }
    public int DisplayOrder { get; set; }
    public bool ShowClauseRef { get; set; }

    //public List<AnswerNew> Answers { get; set; }
    public string AnswerHeaderText { get; set; }
    public string AnswerText { get; set; }
    public int AnswerId { get; set; }
    public string ClauseRefValue { get; set; }
    public bool IsGrid { get; set; }
    public GridTable Table { get; set; }
}


public enum DisplayType
{
    withoutcontainer,
    containerasquestion, //only applicable for initial lenders row 
    aschildofcontainer, //for all questions to be shown with a container as parent
    aschildofquestion, //for all questions to be shown with a question as parent
    grid
}
public class QuestionWithAnswer
{
    public string DisplayText { get; set; } //either question or container text 
    public DisplayType ShowAs { get; set; }
    public int QuestionId { get; set; }
    public decimal ContainerDisplayNumber
    {
        get
        {
            decimal v;
            decimal.TryParse(ContainerDisplayNumberStr, out v);

            return v;
        }
    }
    public string ContainerDisplayNumberStr { get; set; }

    public string ContainerHeaderText { get; set; }
    public int QuestionDisplayOrder { get; set; }
    public bool ShowClauseRef { get; set; }
    public string AnswerText { get; set; }
    public int AnswerId { get; set; }
    public string ClauseRefValue { get; set; }
    public GridTable Grid { get; set; }
}


public class AnswerNew
    {
        public string HeaderText { get; set; }
        public int DisplayOrder { get; set; }
        public string AnswerText { get; set; }
        public int AnswerId { get; set; }
        public string ClauseRefValue { get; set; }
    }

    public class SectionNew
    {
        public string Name { get; set; }
        public int Id { get; set; }
       
        public List<ContainerNew> Containers { get; set; }
    //    public List<QuestionNew> Questions { get; set; }
    
        /// <summary>
        /// Order by Section first when creating pdf 
        /// </summary>
        public int Order { get; set; }
    }

public class SectionWithQuestions
{
    public string SectionName { get; set; }
    public int SectionId { get; set; }
    public List<QuestionWithAnswer> AllQuestions { get; set; }

    /// <summary>
    /// Order by Section first when creating pdf 
    /// </summary>
    public int SectionOrder { get; set; }

    public bool ShowClauseRef { get; set; }
}

public class LprForm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<SectionNew> Sections { get; set; }
    }



public class ReportForm
{
    public int  Id { get; set; }

    public string FormName { get; set; }

    public List<SectionWithQuestions> AllSections { get; set; }
}


