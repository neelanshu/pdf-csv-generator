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
    public ReportForm GenerateReportObject()
    {
        //var json =
        //    File.ReadAllText(
        //        @"C:\git\pdf-csv-generator\AO.LPR.Reports\full_json.json");

        var json =
           File.ReadAllText(
               @"C:\git\pdf-csv-generator\AO.LPR.Reports\child_questions_json.json");

        var reportForm = new ReportForm() { AllSections = new List<SectionWithQuestions>() };
        var formObj = JsonConvert.DeserializeObject<RootObject>(json);
        foreach (
            var sec in
            formObj.sections.OrderBy(x => x.displayOrder))
            //formObj.sections.Where(x => x.id == 2).OrderBy(x => x.displayOrder))
        {
            var section = new SectionWithQuestions()
            {
                SectionId = sec.id,
                SectionName = sec.name,
                SectionOrder = sec.displayOrder,
                AllQuestions = new List<QuestionWithAnswer>()
            };

            foreach (var container in sec.questionContainers)
            {
                var isDefaultEmptyContainer = string.IsNullOrEmpty(container.containerText);
                var containerId = container.id;
                var containerText = container.containerText;
                var containerNumber = container.number;

                DisplayType showAs = isDefaultEmptyContainer ? DisplayType.nocontainer : DisplayType.aschildofcontainer;

                if (section.SectionName.ToLower().Equals("overview") && containerNumber.Equals("1.1"))
                showAs = DisplayType.containerasquestion;
                
                PopulateQuestionWithAnswerRecursive(section, container.questions, containerText, containerNumber, showAs);

                #region Working code
                /*uncomment below to get working version
                //if all questions inside a container are composite and grid type
                // haschildren = true 
                // iscomposite = true 
                // and of type grid
                if (container.questions.TrueForAll(
                    x => x.isComposite && x.hasChildren && x.questionType.ToLower().Contains("grid")))
                {

                    //question = new QuestionWithAnswer()
                    //{
                    //    ShowAs = DisplayType.grid,
                    //    DisplayText = containerText,
                    //    QuestionDisplayOrder = container.questions[0].displayOrder,
                    //    QuestionId = container.questions[0].id,
                    //    ContainerDisplayNumberStr = containerNumber,
                    //    ContainerHeaderText = containerText
                    //};

                    //question.Grid = CreateGrid(container.questions);
                    section.AllQuestions.Add(PopulateQuestionWithAnswerForGrid(container.questions, containerText,
                        containerNumber));
                }

                //if questions in the container are non composite 
                //iscomposite = false
                else if (container.questions.TrueForAll(
                    x => !x.isComposite && !x.questionType.ToLower().Contains("grid")))
                {
                    if (container.questions.TrueForAll(x => !x.hasChildren))
                    {
                        foreach (var q in container.questions.OrderBy(x => x.displayOrder))
                        {
                            //question = new QuestionWithAnswer()
                            //{
                            //    ShowAs = DisplayType.nocontainer,
                            //    DisplayText = q.questionText,
                            //    QuestionDisplayOrder = q.displayOrder,
                            //    QuestionId = q.id,
                            //    ContainerDisplayNumberStr = containerNumber,
                            //    ContainerHeaderText = containerText
                            //};

                            //var answerForThisQUestion =
                            //    PopulateAnswerText(
                            //        q.answers.First(x => x.id > 0 || x.option != null && x.option.isSelected));

                            //question.AnswerId = answerForThisQUestion.id;
                            //question.AnswerText = answerForThisQUestion.answerText;
                            //question.ClauseRefValue = answerForThisQUestion.clauseRef;

                            section.AllQuestions.Add(PopulateQuestionWithAnswerForSimple(q, containerNumber,
                                containerText));
                        }
                    }
                    else if (container.questions.TrueForAll(x => x.hasChildren && x.showChildrenIfOptionId > 0))
                        //questions which show below a parent question
                    {
                        foreach (var q in container.questions.OrderBy(x => x.displayOrder))
                        {
                            //question = new QuestionWithAnswer()
                            //{
                            //    ShowAs = DisplayType.nocontainer,
                            //    DisplayText = q.questionText,
                            //    QuestionDisplayOrder = q.displayOrder,
                            //    QuestionId = q.id,
                            //    ContainerDisplayNumberStr = containerNumber,
                            //    ContainerHeaderText = containerText
                            //};

                            //var answerForThisQUestion =
                            //    PopulateAnswerText(
                            //        q.answers.First(x => x.id > 0 || x.option != null && x.option.isSelected));

                            //question.AnswerId = answerForThisQUestion.id;
                            //question.AnswerText = answerForThisQUestion.answerText;
                            //question.ClauseRefValue = answerForThisQUestion.clauseRef;

                            section.AllQuestions.Add(PopulateQuestionWithAnswerForSimple(q, containerNumber,
                                containerText));

                            //check if childquestions are to be considered for display 
                            if (q.showChildrenIfOptionId ==
                                q.answers.First(x => x.id > 0 && x.option != null && x.option.isSelected).option.id)
                            {
                                //iterate through childquestions and create question under the same container 
                                //but with
                                //parent as the main question
                                //keep the display order of the child question


                            }

                        }
                    }
                 }*/
                #endregion
            }
            reportForm.AllSections.Add(section);
        }
        return reportForm;
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
            question.ClauseRefValue = string.Empty;
        }
        else
        {
            answerForThisQUestion =
                PopulateAnswerText(
                    q.answers.First(x => x.id > 0 || (x.option != null && x.option.isSelected)));

            question.AnswerId = answerForThisQUestion.id;
            question.AnswerText = answerForThisQUestion.answerText;
            question.ClauseRefValue = answerForThisQUestion.clauseRef;
        }

        return question;
    }
    public QuestionWithAnswer CreateQuestionWithAnswerFromParentContainer(int displayOrder, string displayNumber, string containerText)
    {
        //var displayOrderOfFirstQuestion = allQuestionsInContainer[0].displayOrder;
        var displayOrderOfFirstQuestion = displayOrder;

        var containerAsFirstQuestion = new QuestionWithAnswer()
        {
            ShowAs = DisplayType.nocontainer,
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
            ContainerHeaderText = displayText
        };

        question.AnswerText = CreateGroupedAnswers(allQuestionsInContainer);
        return question;
    }
    public QuestionWithAnswer PopulateQuestionWithAnswerForGrid(List<Question> allQuestionsInGrid, string displayText, string displayNumber)
    {
        var question = new QuestionWithAnswer()
            {
                ShowAs = DisplayType.grid,
                DisplayText = displayText,
                QuestionDisplayOrder = allQuestionsInGrid[0].displayOrder,
                QuestionId = allQuestionsInGrid[0].id,
                ContainerDisplayNumberStr = displayNumber,
                ContainerHeaderText = displayText
            };

            question.Grid = CreateGrid(allQuestionsInGrid);
        return question;
    }
    private string CreateGroupedAnswers(List<Question> allQuestionsInContainer)
    {
        var stringbuilder = new StringBuilder();
        var answerFormat = "{0} - {1}"; //mnemonic - comma separated list of answers in that menomnic

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
            stringbuilder.Append(toAdd);
            stringbuilder.Append("<br></br>");
        }

        return stringbuilder.ToString();
    }
    public void PopulateQuestionWithAnswerRecursive(SectionWithQuestions section, List<Question> allQuestionsInAContainerOrParent, string containerText, string containerNumber, DisplayType showAs)
    {
        //if all questions inside a container are composite and grid type
        // haschildren = true 
        // iscomposite = true 
        // and of type grid
        
        if (allQuestionsInAContainerOrParent.TrueForAll(x => x.isComposite ))
        {
            if (allQuestionsInAContainerOrParent.TrueForAll(x => x.hasChildren))
            {
                if (allQuestionsInAContainerOrParent.TrueForAll(x => x.questionType.ToLower().Contains("grid")))
                {
                    if (showAs == DisplayType.aschildofquestion)
                    {
                        containerText = allQuestionsInAContainerOrParent[0].questionText;
                    }

                    section.AllQuestions.Add(PopulateQuestionWithAnswerForGrid(allQuestionsInAContainerOrParent,
                        containerText,
                        containerNumber));
                }
                else //if not all are grid type questions
                {

                }
            }
            else //if parent composite questions donot have children
            {
                
            }
        }
        //if parent questions in the container are non composite 
        //iscomposite = false
        else if (allQuestionsInAContainerOrParent.TrueForAll(x => !x.isComposite))
        {
            if (allQuestionsInAContainerOrParent.TrueForAll(x => !x.questionType.ToLower().Contains("grid")))
            {
                if (allQuestionsInAContainerOrParent.TrueForAll(x => !x.hasChildren))
                {
                    if (showAs == DisplayType.nocontainer)
                    {
                        foreach (var q in allQuestionsInAContainerOrParent.OrderBy(x => x.displayOrder))
                        {
                            section.AllQuestions.Add(PopulateQuestionWithAnswerForSimple(q, containerNumber,
                                    containerText, showAs));
                                //nocontainer - questions to be shown with their own questiontext and without container heading etc

                        }
                    }
                    else if (showAs == DisplayType.containerasquestion) //only for initial lenders display 
                    {
                        section.AllQuestions.Add(
                            PopulateQuestionWithAnswerForContainerAsQuestion(allQuestionsInAContainerOrParent,
                                containerText,
                                containerNumber));
                    }
                    else if (showAs == DisplayType.aschildofquestion)
                    {
                        //to do 
                    }
                    else if (showAs == DisplayType.aschildofcontainer)
                    {
                        section.AllQuestions.Add(
                            CreateQuestionWithAnswerFromParentContainer(
                                allQuestionsInAContainerOrParent[0].displayOrder, containerNumber, containerText));

                        var containerNumberEmpty = string.Empty;
                        foreach (var q in allQuestionsInAContainerOrParent.OrderBy(x => x.displayOrder))
                        {
                            section.AllQuestions.Add(PopulateQuestionWithAnswerForSimple(q, containerNumberEmpty,
                                containerText, DisplayType.aschildofcontainer));

                        }
                    }
                }
                else if (allQuestionsInAContainerOrParent.TrueForAll(x => x.hasChildren))
                    //questions which show below a parent question
                {
                    if (allQuestionsInAContainerOrParent.TrueForAll(x => x.showChildrenIfOptionId > 0))
                    {
                        foreach (var q in allQuestionsInAContainerOrParent.OrderBy(x => x.displayOrder))
                        {
                            section.AllQuestions.Add(PopulateQuestionWithAnswerForSimple(q, containerNumber,
                                containerText, showAs)); //nocontainer

                            //check if childquestions are to be considered for display 
                            if (q.showChildrenIfOptionId ==
                                q.answers.First(x => x.id > 0 || x.option != null && x.option.isSelected).option.id)
                            {
                                //iterate through childquestions and create question under the same container 
                                //but with
                                //parent as the main question
                                //keep the display order of the child question

                                var allChildQuestions = q.childQuestions.Select(TransformToQuestion).ToList();
                                //since these are child questions 
                                //they dont need a containernumber 

                                var containerNumberEmpty = string.Empty;
                                PopulateQuestionWithAnswerRecursive(section, allChildQuestions, containerText,
                                    containerNumberEmpty, DisplayType.aschildofquestion);
                            }
                        }
                    }
                    else
                    {
                        foreach (var q in allQuestionsInAContainerOrParent.OrderBy(x => x.displayOrder))
                        {
                            section.AllQuestions.Add(PopulateQuestionWithAnswerForSimple(q, containerNumber,
                                containerText, showAs)); //nocontainer


                            //iterate through childquestions and create question under the same container 
                            //but with
                            //parent as the main question
                            //keep the display order of the child question

                            var allChildQuestions = q.childQuestions.Select(TransformToQuestion).ToList();
                            //since these are child questions 
                            //they dont need a containernumber 

                            var containerNumberEmpty = string.Empty;
                            PopulateQuestionWithAnswerRecursive(section, allChildQuestions, containerText,
                                containerNumberEmpty, DisplayType.aschildofquestion);

                        }
                    }
                }
            }
        }
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
                var answerForThisChildQUestion =
                    PopulateAnswerText(
                        TransformAnswer(child.answers.First(x => x.id > 0 && x.option != null && x.option.isSelected)));

                allColsForRow.Add(new GridCol() {ColText =  answerForThisChildQUestion.answerText, ColumnNo = child.displayOrder });
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

    #region unused code
    //foreach (var ques in container.questions.OrderBy(x => x.displayOrder))
    //{
    //    var question = new QuestionNew()
    //    {
    //        QuestionId = ques.id,
    //        DisplayOrder = ques.displayOrder,
    //        ShowClauseRef = ques.showClauseRef,
    //        DisplayText = ques.questionText,
    //        IsGrid = (ques.questionType.ToLower().Contains("grid")),
    //        //ClauseRef = "" -- should be set as part of answers population
    //    };
    //    container.Questions.Add(PopulateQuestionRecursive(question, ques));
    //}
    //}
    //else if (container.ContainerAsParent)
    //{
    //    foreach (var ques in cont.questions.OrderBy(x => x.displayOrder))
    //    {
    //        var question = new QuestionNew()
    //        {
    //            QuestionId = ques.id,
    //            DisplayOrder = ques.displayOrder,
    //            ShowClauseRef = ques.showClauseRef,
    //            DisplayText = "",
    //            IsGrid = (ques.questionType.ToLower().Contains("grid")),
    //            //ClauseRef = "" -- should be set as part of answers population
    //        };
    //        container.Questions.Add(PopulateQuestionRecursive(question, ques));
    //    }

    //}



    //if atleast one question has showclauseref as TRUE - show the style with showclauseref 
    //Assign answers 

    //a question can have ONE answer 
    //or 
    //multiple answers 
    //or
    //child questions
    //which will have same as above 

    //ONE answer type question - if there is just 1 item in the answers array 
    //and
    //no child questions 

    //make sure we remove all nulls



    //        section.Containers.Add(container);
    //    }
    //    lprForm.Sections.Add(section);
    //}
    public LprForm GetFormDetails()
    {
        var json =
            File.ReadAllText(
                @"C:\Users\neelanshu\Desktop\LPR\ClassLibrary1\ClassLibrary1\AO.LPR.Reports\full_json.json");

        var lprForm = new LprForm() { Sections = new List<SectionNew>() };
        var formObj = JsonConvert.DeserializeObject<RootObject>(json);

        foreach (var sec in formObj.sections.OrderBy(x => x.displayOrder))
        {
            var section = new SectionNew
            {
                Id = sec.id,
                Name = sec.name,
                Order = sec.displayOrder,
                Containers = new List<ContainerNew>()
            };

            foreach (var cont in sec.questionContainers) //ignore overview initial lenders question for now 
            {
                var container = new ContainerNew
                {
                    ContainerId = cont.id,
                    DisplayNumber = cont.number,
                    ContainerDisplayText = cont.containerText,
                };

                if (container.DisplayNumberDecimal == 1.1)
                {
                    break;
                }

                if (!container.ContainerAsParent)  //for containers without container text i.e show the question text as display text
                {
                    foreach (var ques in cont.questions.OrderBy(x => x.displayOrder))
                    {
                        var question = new QuestionNew()
                        {
                            QuestionId = ques.id,
                            DisplayOrder = ques.displayOrder,
                            ShowClauseRef = ques.showClauseRef,
                            DisplayText = ques.questionText,
                            IsGrid = (ques.questionType.ToLower().Contains("grid")),
                            //ClauseRef = "" -- should be set as part of answers population
                        };
                        container.Questions.Add(PopulateQuestionRecursive(question, ques));
                    }
                }
                else if (container.ContainerAsParent)
                {
                    foreach (var ques in cont.questions.OrderBy(x => x.displayOrder))
                    {
                        var question = new QuestionNew()
                        {
                            QuestionId = ques.id,
                            DisplayOrder = ques.displayOrder,
                            ShowClauseRef = ques.showClauseRef,
                            DisplayText = "",
                            IsGrid = (ques.questionType.ToLower().Contains("grid")),
                            //ClauseRef = "" -- should be set as part of answers population
                        };
                        container.Questions.Add(PopulateQuestionRecursive(question, ques));
                    }

                }



                //if atleast one question has showclauseref as TRUE - show the style with showclauseref 
                //Assign answers 

                //a question can have ONE answer 
                //or 
                //multiple answers 
                //or
                //child questions
                //which will have same as above 

                //ONE answer type question - if there is just 1 item in the answers array 
                //and
                //no child questions 

                //make sure we remove all nulls



                section.Containers.Add(container);
            }
            lprForm.Sections.Add(section);
        }
        return lprForm;
    }
    public QuestionNew PopulateQuestionRecursive(QuestionNew question, Question ques)
    {
        if (!ques.isComposite)
        {
            if (ques.answers == null || ques.answers.Count < 1) //no answers provided for this question  
                return question; //if no answers array then no use doing this 

            //here i create a question with a flat string answer - 1 to 1 relationship between container, question and answer 
            foreach (var ans in
                ques.answers.Where(x => x.id > 0 && x.option != null && x.option.isSelected))
            {
                var updatedAnswer = PopulateAnswerText(ans);
                question.AnswerText = updatedAnswer.answerText;
                question.AnswerHeaderText = updatedAnswer.answerHeader;
                question.AnswerId = updatedAnswer.id;
            }

            if (!ques.hasChildren)
                return question;
        }

        if (ques.isComposite)
        {
            if (ques.hasChildren && ques.childQuestions != null)
            {

            }

        }

        //if (ques.hasChildren && ques.childQuestions != null)
        //{
        //    //go through each child question
        //    //and set container same as parent question container 
        //    //and assign answers 
        //    foreach (var child in ques.childQuestions)
        //    {
        //        var questionNew = new QuestionNew()
        //        {
        //            ContainerId = question.ContainerId,
        //            DisplayNumber = string.Empty,
        //            UseContainerTextAsQuestionText = false,
        //            QuestionText = child.questionText,
        //            ContainerAsParent = true
        //        };

        //        PopulateQuestionRecursive(questionNew, TransformToQuestion(child), questionNew.ContainerAsParent);
        //    }
        //}
        //if (ques.hasChildren && ques.childQuestions!= null)
        //{
        //    foreach (var child in ques.childQuestions)
        //    {
        //        //var newQuestion = new QuestionNew()
        //        //{
        //        //    DisplayOrder = ques.displayOrder,
        //        //    Id = ques.id,
        //        //    IsComposite = ques.isComposite,
        //        //    HasChildren = ques.hasChildren,
        //        //    QuestionText = ques.questionText,
        //        //    IsMandatory = ques.isMandatory,
        //        //    HasClauseRef = ques.showClauseRef,
        //        //    DisplayNumber = question.DisplayNumber,
        //        //    ContainerHeading = question.ContainerHeading,
        //        //    UseContainerTextAsQuestionText = question.UseContainerTextAsQuestionText,
        //        //    Answers = new List<AnswerNew>()
        //        //};

        //        var newQuestion = new QuestionNew()
        //        {
        //            ContainerId = question.ContainerId,
        //            DisplayOrder = ques.displayOrder,
        //            Id = ques.id,
        //            QuestionText = ques.questionText,
        //            DisplayNumber = question.DisplayNumber,
        //            UseContainerTextAsQuestionText = !question.UseContainerTextAsQuestionText,
        //            IsGrid = ques.questionType.ToLower().Contains("grid"),
        //            ShowClauseRef = ques.showClauseRef

        //        };

        //        if (!container.IsEmptyContainer)
        //        {
        //            question.QuestionText = container.Text;
        //        }

        //        question.ContainerAsParent = true;

        //        PopulateQuestionRecursive(newQuestion, TransformToQuestion(child));
        //    }
        //}
        //    return question;
        //}
        //if (ques.isComposite)
        //{
        //    if (ques.questionType.ToLower().Contains("grid")) //todo
        //    {
        //        var questionNew = new QuestionNew()
        //        {
        //            ContainerId = question.ContainerId,
        //            DisplayNumber = string.Empty,
        //            UseContainerTextAsQuestionText = false,
        //            QuestionText = ques.questionText,
        //            ContainerAsParent = true
        //        };

        //        if (ques.hasChildren && ques.childQuestions != null)
        //        {
        //            //question.TotalColumnsInGridRow = ques.childQuestions.Count;
        //            //questionNew.GRow = new List<GridRow>();


        //            //var headerRow = new GridRow() {ColumnTexts = new List<string>()};
        //            //question.GRow.Add(headerRow);

        //            //foreach (var child in ques.childQuestions.OrderBy(x=>x.displayOrder))
        //            //{
        //            //    question.GRow[0].ColumnTexts.Add(child.questionText);


        //            //    row.ColumnTexts = new List<string>();
        //            //    row.ColumnTexts.Add(child.questionText);
        //            //    questionNew.GRow.Add(new GridRow());

        //            //    PopulateQuestionRecursive(questionNew, TransformToQuestion(child), questionNew.ContainerAsParent);
        //            //}
        //        }
        //    }


        //    if (ques.hasChildren)
        //    {
        //        foreach (var child in ques.childQuestions)
        //        {
        //            var childQuestion = new QuestionNew()
        //            {
        //                DisplayOrder = ques.displayOrder,
        //                Id = ques.id,
        //                //IsComposite = ques.isComposite,
        //                //HasChildren = ques.hasChildren,
        //                QuestionText = ques.questionText,
        //                //IsMandatory = ques.isMandatory,
        //                //HasClauseRef = ques.showClauseRef,
        //                DisplayNumber = question.DisplayNumber,
        //                //ContainerHeading = question.ContainerHeading,
        //                UseContainerTextAsQuestionText = question.UseContainerTextAsQuestionText,
        //                //Answers = new List<AnswerNew>()
        //            };
        //            //var serviceDtoQuestion = PopulateQuestionRecursive(childQuestion, TransformToQuestion(child));

        //            //foreach (var ans in serviceDtoQuestion.Answers)
        //            //{


        //            //    question.Answers.Add(new AnswerNew()
        //            //    {
        //            //        ClauseRef = string.Empty,
        //            //        Id = ans.Id,
        //            //        Text = ans.Text,
        //            //        DisplayOrder = ques.displayOrder,
        //            //        AnswerHeaderText = ans.AnswerHeaderText
        //            //    });
        //            //}
        //        }

        //        if (!ques.hasChildren)
        //        {
        //            return question;
        //        }
        //    }
        //}
        return question;
    }
#endregion 

}