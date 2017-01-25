using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AO.LPR.Reports;
using Newtonsoft.Json;

public class GenericLPRObjService
{
    public List<int> QuestionIdsNotDealtWith = new List<int>();
    public ReportForm GenerateReportObjectNew()
    {
        var json =
            File.ReadAllText(
                @"C:\git\pdf-csv-generator\AO.LPR.Reports\full_json.json");

        var reportForm = new ReportForm() {AllSections = new List<SectionWithQuestions>()};
        var formObj = JsonConvert.DeserializeObject<RootObject>(json);

        reportForm.FormName = formObj.name;
        reportForm.Id = formObj.id;
        foreach (
                var sec in
                formObj.sections.OrderBy(x => x.displayOrder))
           
        {
            var section = new SectionWithQuestions()
            {
                SectionId = sec.id,
                SectionName = sec.name,
                SectionOrder = sec.displayOrder,
                AllQuestions = new List<QuestionWithAnswer>()
            };

            var showClauseRef = false; 
            foreach (var container in sec.questionContainers)
            {
                if (!showClauseRef)
                {
                    showClauseRef = container.questions.Any(x => x.showClauseRef);
                }
                var isDefaultEmptyContainer = string.IsNullOrEmpty(container.containerText);
                var containerId = container.id;
                var containerText = container.containerText;
                var containerNumber = container.number;

                DisplayType showAs = isDefaultEmptyContainer
                    ? DisplayType.withoutcontainer
                    : DisplayType.aschildofcontainer;

                if (section.SectionName.ToLower().Equals("overview") && containerNumber.Equals("1.1"))
                    showAs = DisplayType.containerasquestion;

                PopulateQuestionWithAnswerRecursiveNew(section, container.questions, containerText,
                    containerNumber, showAs);
               
            }
            section.ShowClauseRef = showClauseRef;
            reportForm.AllSections.Add(section);
        }
        return reportForm;
    }
    public void PopulateQuestionWithAnswerSingle(SectionWithQuestions section, Question question, string containerText,
        string containerNumber, DisplayType showAs)
    {
        if (!question.isComposite)
        {
            section.AllQuestions.Add(
                PopulateQuestionWithAnswerForSimple(question, containerNumber, containerText, showAs));

            if (question.hasChildren && question.childQuestions != null)
            {
                var showAsLocal = DisplayType.aschildofquestion;
                var containerNumberLocal = string.Empty;
                if (question.showChildrenIfOptionId > 0)
                {
                    //only proceed to populate child questions
                    //if the parent question was answered with an answer that prompts for a child question
                    if (question.showChildrenIfOptionId ==
                        question.answers.First(x => x.id > 0 || x.option != null && x.option.isSelected).option.id)
                    {
                        //string clauseRefValueFromParent = string.Empty;
                        //if (question.answers.Any(x => !string.IsNullOrEmpty(x.clauseRef)))
                        //{
                        //    var ans = question.answers.FirstOrDefault(x => !string.IsNullOrEmpty(x.clauseRef));
                        //    clauseRefValueFromParent = ans.clauseRef;
                        //}

                        var allChildQuestions = question.childQuestions.Select(TransformToQuestion).ToList();
                        PopulateQuestionWithAnswerRecursiveNew(section, allChildQuestions, containerText,
                            containerNumberLocal,
                            showAsLocal);
                    }
                }
                else
                {
                    //TOD - check if this is needed 

                    //string clauseRefValueFromParent = string.Empty;
                    //if (question.answers.Any(x => !string.IsNullOrEmpty(x.clauseRef)))
                    //{
                    //    var ans = question.answers.FirstOrDefault(x => !string.IsNullOrEmpty(x.clauseRef));
                    //    clauseRefValueFromParent = ans.clauseRef;
                    //}

                    var allChildQuestions = question.childQuestions.Select(TransformToQuestion).ToList();
                    PopulateQuestionWithAnswerRecursiveNew(section, allChildQuestions, containerText,
                        containerNumberLocal,
                        showAsLocal);
                }

            }

        }
        else //if question is composite //shouldnt come here 
        {
            QuestionIdsNotDealtWith.Add(question.id);
        }
    }

    public QuestionWithAnswer PopulateQuestionWithAnswerForGridWrapper(List<Question> allQuestionsInAContainerOrParent,
        string containerText, string containerNumber, DisplayType showAs, string clauseRefValueFromParent)
    {
        if (showAs == DisplayType.aschildofquestion)
        {
            containerText = allQuestionsInAContainerOrParent[0].questionText;
        }

        return PopulateQuestionWithAnswerForGrid(allQuestionsInAContainerOrParent, containerText,
            containerNumber, clauseRefValueFromParent);
    }

    public void PopulateQuestionWithAnswerRecursiveNew(SectionWithQuestions section,
        List<Question> allQuestionsInAContainerOrParent,
        string containerText, string containerNumber, DisplayType showAs)
    {
        // if all questions inside a container are composite and grid type
        //only going to happen when a container has only grid rows as questions

        bool edgeCased = false;

        if (allQuestionsInAContainerOrParent.TrueForAll(
            x => x.isComposite && x.hasChildren && x.questionType.ToLower().Contains("grid")))
        {
            string clauseRefValueFromParent = string.Empty;
            if (allQuestionsInAContainerOrParent.Any(x=>x.answers.Any(y => !string.IsNullOrEmpty(y.clauseRef))))
            {
                var ans =
                    allQuestionsInAContainerOrParent[0].answers.FirstOrDefault(x => !string.IsNullOrEmpty(x.clauseRef));
                clauseRefValueFromParent = ans.clauseRef;
            }

            section.AllQuestions.Add(PopulateQuestionWithAnswerForGridWrapper(allQuestionsInAContainerOrParent,
                containerText,
                containerNumber, showAs, clauseRefValueFromParent));

            edgeCased = true;
        }


        if (!edgeCased && showAs == DisplayType.containerasquestion) //only for initial lenders columns
        {
            section.AllQuestions.Add(PopulateQuestionWithAnswerForContainerAsQuestion(allQuestionsInAContainerOrParent,
                containerText,
                containerNumber));

            edgeCased = true;
        }


        if (!edgeCased && showAs == DisplayType.withoutcontainer)
        {
            //change nothing - populate questions with values sent from the calling method 

            foreach (var qu in allQuestionsInAContainerOrParent.OrderBy(x => x.displayOrder))
            {
                PopulateQuestionWithAnswerSingle(section, qu, containerText, containerNumber,
                     showAs);
            }
        }


        if (!edgeCased && showAs == DisplayType.aschildofcontainer)
        {
            QuestionWithAnswer parentQuestion = CreateQuestionWithAnswerFromParentContainer(
                allQuestionsInAContainerOrParent[0].displayOrder, containerNumber, containerText);

            section.AllQuestions.Add(parentQuestion);

            containerNumber = string.Empty;
            showAs = DisplayType.aschildofcontainer;

            foreach (var qu in allQuestionsInAContainerOrParent.OrderBy(x => x.displayOrder))
            {
                PopulateQuestionWithAnswerSingle(section, qu, containerText, containerNumber,
                     showAs);
            }
        }
    }
    
    public QuestionWithAnswer PopulateQuestionWithAnswerForSimple(Question q, string displayNumber, string containerText, DisplayType showAs)
    {
        var question = new QuestionWithAnswer()
        {
            ShowAs = showAs,
            DisplayText = q.questionText,
            QuestionDisplayOrder = q.displayOrder,
            QuestionId = q.id,
            ContainerDisplayNumberStr = displayNumber,
            ContainerHeaderText = containerText
        };

        var answerForThisQUestion = q.answers.FirstOrDefault(x => x.id > 0 || (x.option != null && x.option.isSelected));
        if (answerForThisQUestion == null)
        {
            question.AnswerId = 0;
            question.AnswerText = string.Empty;
            
        }
        else
        {
            answerForThisQUestion =
                PopulateAnswerText(
                    q.answers.First(x => x.id > 0 || (x.option != null && x.option.isSelected)));

            question.AnswerId = answerForThisQUestion.id;
            question.AnswerText = answerForThisQUestion.answerText;
            //question.ClauseRefValue = answerForThisQUestion.clauseRef;
        }

        if (q.answers.Any(x => !string.IsNullOrEmpty(x.clauseRef)))
        {
            var ans = q.answers.FirstOrDefault(x => !string.IsNullOrEmpty(x.clauseRef));
            question.ClauseRefValue = ans.clauseRef;
        }

        return question;
    }
    public QuestionWithAnswer CreateQuestionWithAnswerFromParentContainer(int displayOrder, string displayNumber, string containerText)
    {
        //var displayOrderOfFirstQuestion = allQuestionsInContainer[0].displayOrder;
        var displayOrderOfFirstQuestion = displayOrder;

        var containerAsFirstQuestion = new QuestionWithAnswer()
        {
            ShowAs = DisplayType.withoutcontainer,
            DisplayText = containerText,
            QuestionDisplayOrder = displayOrderOfFirstQuestion,
            ContainerDisplayNumberStr = displayNumber,
            ContainerHeaderText = containerText
        };


        return containerAsFirstQuestion;
    }
    public  QuestionWithAnswer PopulateQuestionWithAnswerForContainerAsQuestion(List<Question> allQuestionsInContainer, string displayText, string displayNumber)
    {
        var question = new QuestionWithAnswer()
        {
            ShowAs = DisplayType.containerasquestion,
            DisplayText = displayText,
            QuestionDisplayOrder = allQuestionsInContainer[0].displayOrder,
            QuestionId = allQuestionsInContainer[0].id,
            ContainerDisplayNumberStr = displayNumber,
            ContainerHeaderText = displayText,
            ClauseRefValue = string.Empty
        };

        question.AnswerText = CreateGroupedAnswers(allQuestionsInContainer);
        return question;
    }
    public QuestionWithAnswer PopulateQuestionWithAnswerForGrid(List<Question> allQuestionsInGrid, string displayText, string displayNumber, string clauseRefValueFromParent)
    {
        var question = new QuestionWithAnswer()
            {
                ShowAs = DisplayType.grid,
                DisplayText = displayText,
                QuestionDisplayOrder = allQuestionsInGrid[0].displayOrder,
                QuestionId = allQuestionsInGrid[0].id,
                ContainerDisplayNumberStr = displayNumber,
                ContainerHeaderText = displayText,
                ClauseRefValue = clauseRefValueFromParent
            };

            question.Grid = CreateGrid(allQuestionsInGrid);
        return question;
    }
    private string CreateGroupedAnswers(List<Question> allQuestionsInContainer)
    {
        var stringbuilder = new StringBuilder();
        var answerFormat = "<b>{0}</b> - {1}"; //mnemonic - comma separated list of answers in that menomnic

        foreach (var question in allQuestionsInContainer)
        {
            List<string> answerForEachQuestion = new List<string>();
            foreach (var ans in question.answers.Where(x => x.id > 0 || x.option != null && x.option.isSelected))
            {
                var answerForThisQuestion = PopulateAnswerText(ans);
                answerForEachQuestion.Add(answerForThisQuestion.answerText);
            }

            string toAdd = string.Empty;
            if (!string.IsNullOrEmpty(string.Join(",", answerForEachQuestion)))
                toAdd = string.Format(answerFormat, question.mnemonic, string.Join(", ", answerForEachQuestion));
            stringbuilder.AppendLine(toAdd);
            //stringbuilder.Append("<br></br>");
        }

        return stringbuilder.ToString();
    }
    private GridTable CreateGrid(List<Question> allQuestionsInAContainer)
    {
        var noOfRows = allQuestionsInAContainer.Count + 1; //one extra row for header 
        var grid = new GridTable() { AllRows = new List<GridRow>()};

        for (int i = 1; i <= noOfRows; i++)
        {
            grid.AllRows.Add(new GridRow() {RowNo = i});
        }

        var allColsForRow = new List<GridCol>(); //headerrow cols

        foreach (var child in allQuestionsInAContainer[0].childQuestions.Where(cq => !cq.isComposite && !cq.hasChildren && !cq.questionType.ToLower().Contains("grid")).OrderBy(x => x.displayOrder)) //each child question is a column 
        {
            allColsForRow.Add(new GridCol() { ColText = child.questionText, ColumnNo = child.displayOrder });
        }

        grid.AllRows.First(x => x.RowNo == 1).AllCols = allColsForRow; //updating headerrow with all the headers

        var orderedQuestions = allQuestionsInAContainer.OrderBy(x => x.displayOrder).ToList();

        for (int i = 0; i < allQuestionsInAContainer.Count; i++)
        {
            var question = orderedQuestions[i];
            //foreach (var question in allQuestions.OrderBy(x => x.displayOrder)) //each question is a row in the table 

            allColsForRow = new List<GridCol>();
            foreach (var child in question.childQuestions.Where(cq=>!cq.isComposite && !cq.hasChildren && !cq.questionType.ToLower().Contains("grid")).OrderBy(x=>x.displayOrder)) //each child question is a column 
            {
                var cAns = child.answers.FirstOrDefault(x => x.id > 0 || (x.option != null && x.option.isSelected));

                if (cAns != null)
                {
                    var answerForThisChildQUestion =
                        PopulateAnswerText(
                            TransformAnswer(child.answers.First(x => x.id > 0 && x.option != null && x.option.isSelected)));

                    allColsForRow.Add(new GridCol()
                    {
                        ColText = answerForThisChildQUestion.answerText,
                        ColumnNo = child.displayOrder
                    });
                }
            }
            grid.AllRows.OrderBy(x=>x.RowNo).First(x => x.RowNo == i+2).AllCols = allColsForRow;
        }
        return grid;
    }
    public Answer6 PopulateAnswerText(Answer6 answer)
    {
        if (answer.answerDate != null &&  answer.answerDate > DateTime.MinValue)
        {
            answer.answerText = answer.answerDate.ToShortDateString();
        }
        else if (answer.userList != null && answer.userList.Count > 0)
        {
            answer.answerText = string.Join(", ", answer.userList.Select(x => x.emailAddress));
        }
        else if (answer.emailList != null && answer.emailList.Count > 0)
        {
            answer.answerText = string.Join(", ", answer.emailList.Select(x => x.text));
        }
        else if (answer.option != null)
        {
            if (answer.option.optionGroupId > 1)
            {
                answer.answerHeader = answer.option.optionGroupText;
            }

            if (answer.option.isSelected)
            {
                if (!string.IsNullOrEmpty(answer.option.text))
                {
                    answer.answerText = answer.option.text;
                }
            }
        }

        return answer;

    }
    public Question TransformToQuestion(ChildQuestion child)
    {
        var obj = new Question()
        {
            displayOrder = child.displayOrder,
            hasChildren = child.hasChildren,
            id = child.id,
            isComposite = child.isComposite,
            //isMandatory = child.isMandatoryThanks ,
            questionText = child.questionText,
            showClauseRef = child.showClauseRef,
            showChildrenIfOptionId = child.showChildrenIfOptionId,
            questionType = child.questionType
        };

        obj.answers = new List<Answer6>();

        if (child.answers!= null)
            foreach (var ans in child.answers)
        {
            obj.answers.Add(TransformAnswer(ans));
        }


        obj.childQuestions = new List<ChildQuestion>();

        if (child.childQuestions != null)
        foreach (var ch in child.childQuestions)
        {
            obj.childQuestions.Add(TransformToQuestion1To2(ch));
        }

        return obj;
    }
    public ChildQuestion TransformToQuestion1To2(ChildQuestion2 child)
    {
        var obj = new ChildQuestion()
        {
            displayOrder = child.displayOrder,
            hasChildren = child.hasChildren,
            id = child.id,
            isComposite = child.isComposite,
            isMandatory = child.isMandatory,
            questionText = child.questionText,
            showClauseRef = child.showClauseRef,
            questionType = child.questionType,
            

        };

        obj.answers = new List<Answer7>();

        if (child.answers!= null)
            foreach (var ans in child.answers)
        {
            obj.answers.Add(TransformAnswer7To8(ans));
        }
        
        return obj;

    }
    public Answer7 TransformAnswer7To8(Answer8 answer)
    {
        if (answer== null)
            return new Answer7();


        return new Answer7()
        {
            option = TransformOption6To7(answer.option),
            answerText = answer.answerText,
            id = answer.id,
            //answerHeader = answer.answerHeader,
            clauseRef = answer.clauseRef,
            isDirty = answer.isDirty,
            questionId = answer.questionId

        };

    }
    public Option6 TransformOption6To7(Option7 option)
    {
        if (option == null)
            return new Option6();


        return new Option6()
        {
            id = option.id,
            text = option.text,
            optionGroupId = option.optionGroupId,
            isSelected = option.isSelected,
            optionGroupText = option.optionGroupText
        };
    }
    public Answer6 TransformAnswer(Answer7 answer)
    {
        if (answer == null)
            return new Answer6();

        return new Answer6()
        {
            answerDate = answer.answerDate,
            option = TransformOption(answer.option),
            answerText = answer.answerText,
            emailList = answer.emailList,
            id = answer.id,
            answerHeader = answer.answerHeader,
            clauseRef = answer.clauseRef,
            isDirty = answer.isDirty,
            questionId = answer.questionId

        };

    }
    public Option5 TransformOption(Option6 option)
    {
        if (option ==null)
            return new Option5();

        return new Option5()
        {
            id = option.id,
            text = option.text,
            optionGroupId = option.optionGroupId,
            isSelected = option.isSelected,
            optionGroupText = option.optionGroupText
        };

    }
}