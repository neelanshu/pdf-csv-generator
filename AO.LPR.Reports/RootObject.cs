using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AO.LPR.Reports
{
    public class Option
    {
        public int id { get; set; }
        public int optionGroupId { get; set; }
        public bool isSelected { get; set; }
    }

    public class Option2
    {
        public int id { get; set; }
        public int optionGroupId { get; set; }
        public bool isSelected { get; set; }
    }


    public class Option3
    {
        public int id { get; set; }
        public int optionGroupId { get; set; }
        public bool isSelected { get; set; }
    }

    public class Option4
    {
        public int id { get; set; }
        public int optionGroupId { get; set; }
        public bool isSelected { get; set; }
    }

    public class Options
    {
        public string optionGroupText { get; set; }
        public int optionGroupId { get; set; }
        public List<Value> values { get; set; }
    }

    public class Option5
    {
        public int id { get; set; }
        public string text { get; set; }
        public string optionGroupText { get; set; }
        public int optionGroupId { get; set; }
        public List<string> emailAddresses { get; set; }
        public bool isSelected { get; set; }
    }

    public class Options2
    {
        public string optionGroupText { get; set; }
        public int optionGroupId { get; set; }
        public List<Value2> values { get; set; }
    }

    public class Option6
    {
        public int id { get; set; }
        public string text { get; set; }
        public string optionGroupText { get; set; }
        public int optionGroupId { get; set; }
        public bool isSelected { get; set; }
    }


    //public class Answer
    //{
    //    public int id { get; set; }
    //    public int questionId { get; set; }
    //    public string answerText { get; set; }
    //    public Option option { get; set; }
    //    public bool isDirty { get; set; }
    //    public List<object> emailList { get; set; }
    //}

    public class FormName
    {
        public int id { get; set; }
        public string questionText { get; set; }
        public string questionType { get; set; }
        public int displayOrder { get; set; }
        public bool showClauseRef { get; set; }
        public int maxLength { get; set; }
        public bool isMandatory { get; set; }
        public bool isComposite { get; set; }
        public bool hasChildren { get; set; }
        //public List<Answer> answers { get; set; }
        public List<object> validations { get; set; }
        public string mnemonic { get; set; }
    }



    //public class Answer2
    //{
    //    public int id { get; set; }
    //    public int questionId { get; set; }
    //    public string answerText { get; set; }
    //    public Option2 option { get; set; }
    //    public bool isDirty { get; set; }
    //    public List<object> emailList { get; set; }
    //}

    public class PrincipalBorrower
    {
        public int id { get; set; }
        public string questionText { get; set; }
        public string questionType { get; set; }
        public int displayOrder { get; set; }
        public bool showClauseRef { get; set; }
        public int maxLength { get; set; }
        public bool isMandatory { get; set; }
        public bool isComposite { get; set; }
        public bool hasChildren { get; set; }
      //  public List<Answer2> answers { get; set; }
        public List<object> validations { get; set; }
        public string mnemonic { get; set; }
    }

    //public class Answer3
    //{
    //    public int id { get; set; }
    //    public int questionId { get; set; }
    //    public string answerText { get; set; }
    //    public Option3 option { get; set; }
    //    public bool isDirty { get; set; }
    //    public List<object> emailList { get; set; }
    //}

    public class ClientNumber
    {
        public int id { get; set; }
        public string questionText { get; set; }
        public string questionType { get; set; }
        public int displayOrder { get; set; }
        public bool showClauseRef { get; set; }
        public int maxLength { get; set; }
        public bool isMandatory { get; set; }
        public bool isComposite { get; set; }
        public bool hasChildren { get; set; }
     //   public List<Answer3> answers { get; set; }
        public List<object> validations { get; set; }
        public string mnemonic { get; set; }
    }



    //public class Answer4
    //{
    //    public int id { get; set; }
    //    public int questionId { get; set; }
    //    public string answerText { get; set; }
    //    public Option4 option { get; set; }
    //    public bool isDirty { get; set; }
    //    public List<object> emailList { get; set; }
    //}

    public class MatterNumber
    {
        public int id { get; set; }
        public string questionText { get; set; }
        public string questionType { get; set; }
        public int displayOrder { get; set; }
        public bool showClauseRef { get; set; }
        public int maxLength { get; set; }
        public bool isMandatory { get; set; }
        public bool isComposite { get; set; }
        public bool hasChildren { get; set; }
       // public List<Answer4> answers { get; set; }
        public List<object> validations { get; set; }
        public string mnemonic { get; set; }
    }

    //public class Answer5
    //{
    //    public int id { get; set; }
    //    public string answerText { get; set; }
    //    public bool isDirty { get; set; }
    //}

    public class TransactionDescription
    {
        public int id { get; set; }
        public string questionText { get; set; }
        public string helpText { get; set; }
        public string questionType { get; set; }
        public int displayOrder { get; set; }
        public bool showClauseRef { get; set; }
        public int maxLength { get; set; }
        public bool isMandatory { get; set; }
        public bool isComposite { get; set; }
        public bool hasChildren { get; set; }
        //public List<Answer5> answers { get; set; }
        public List<object> validations { get; set; }
        public string mnemonic { get; set; }
    }

    public class StaticQuestions
    {
        public FormName formName { get; set; }
        public PrincipalBorrower principalBorrower { get; set; }
        public ClientNumber clientNumber { get; set; }
        public MatterNumber matterNumber { get; set; }
        public TransactionDescription transactionDescription { get; set; }
    }

    public class Value
    {
        public string text { get; set; }
        public int id { get; set; }
    }


    public class UserList
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
        public string feeEarnerInitials { get; set; }
    }

    public class Answer6
    {
        private string _answerHeader;
        public string answerHeader
        {
            get { return _answerHeader; }
            set
            {
                if (option != null)
                {
                    if (option.optionGroupId > 1)
                    {
                        _answerHeader = option.optionGroupText;
                    }
                    else
                    {
                        _answerHeader = value;
                    }
                }
            }
        }

        private string _answerText;
        public string answerText
        {
            get { return _answerText; }
            set
            { _answerText = value; }
        }
        public int id { get; set; }
        public int questionId { get; set; }
        public string clauseRef { get; set; }
        public bool isDirty { get; set; }
        public Option5 option { get; set; }
        public List<Value2> emailList { get; set; }
        public DateTime answerDate { get; set; }
        public List<UserList> userList { get; set; }
    }

    public class Value2
    {
        public string text { get; set; }
        public int id { get; set; }
    }


    public class Answer7
    {
        public string answerHeader { get; set; }
        public int id { get; set; }
        public int questionId { get; set; }
        public Option6 option { get; set; }
        public bool isDirty { get; set; }
        public List<Value2> emailList { get; set; }
        public DateTime answerDate { get; set; }
        public string answerText { get; set; }
        public string clauseRef { get; set; }
    }

    public class Value3
    {
        public string text { get; set; }
        public int id { get; set; }
    }

    public class Options3
    {
        public string optionGroupText { get; set; }
        public int optionGroupId { get; set; }
        public List<Value3> values { get; set; }
    }

    public class Option7
    {
        public int id { get; set; }
        public string text { get; set; }
        public string optionGroupText { get; set; }
        public int optionGroupId { get; set; }
        public bool isSelected { get; set; }
    }

    public class Answer8
    {
        public int id { get; set; }
        public int questionId { get; set; }
        public Option7 option { get; set; }
        public bool isDirty { get; set; }
        public List<object> emailList { get; set; }
        public string answerText { get; set; }
        public string clauseRef { get; set; }
    }

    public class ChildQuestion2
    {
        public int id { get; set; }
        public string questionText { get; set; }
        public string questionType { get; set; }
        public int displayOrder { get; set; }
        public bool showClauseRef { get; set; }
        public bool isMandatory { get; set; }
        public bool isComposite { get; set; }
        public bool hasChildren { get; set; }
        public Options3 options { get; set; }
        public List<Answer8> answers { get; set; }
        public List<object> validations { get; set; }
        public string mnemonic { get; set; }
    }

    public class ChildQuestion
    {
        public int id { get; set; }
        public string questionText { get; set; }
        public string questionType { get; set; }
        public int displayOrder { get; set; }
        public bool showClauseRef { get; set; }
        public bool isMandatory { get; set; }
        public bool isComposite { get; set; }
        public bool hasChildren { get; set; }
        public Options2 options { get; set; }
        public List<Answer7> answers { get; set; }
        public List<object> validations { get; set; }
        public List<ChildQuestion2> childQuestions { get; set; }
        public string mnemonic { get; set; }
        public int? showChildrenIfOptionId { get; set; }
        
    }

    public class Question
    {
        public int id { get; set; }
        public string questionText { get; set; }
        public string questionType { get; set; }
        public int displayOrder { get; set; }
        public bool showClauseRef { get; set; }
        public bool isMandatory { get; set; }
        public bool isComposite { get; set; }
        public bool hasChildren { get; set; }
        //public Options options { get; set; }
        public List<Answer6> answers { get; set; }
        //public List<object> validations { get; set; }
        public string mnemonic { get; set; }
        //public string helpText { get; set; }
        //public int? maxLength { get; set; }
        public List<ChildQuestion> childQuestions { get; set; }
        public int? showChildrenIfOptionId { get; set; }
    }

    public class QuestionContainer
    {
        public int id { get; set; }
        public string number { get; set; }
        public string containerText { get; set; }
        public List<Question> questions { get; set; }
    }

    public class Section
    {
        public int id { get; set; }
        public string name { get; set; }
        public int displayOrder { get; set; }
        public List<QuestionContainer> questionContainers { get; set; }
    }

    public class RootObject
    {
        public int id { get; set; }
        public int templateId { get; set; }
        public string name { get; set; }
        public string clientNumber { get; set; }
        public string matterNumber { get; set; }
       // public StaticQuestions staticQuestions { get; set; }
        public List<Section> sections { get; set; }
        public bool isDeprecated { get; set; }
    }
}


//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;

//namespace AO.LPR.Reports
//{
//    [Serializable]
//    public class Option
//    {
//        public int id { get; set; }
//        public int optionGroupId { get; set; }
//        public bool isSelected { get; set; }
//        public string text { get; set; }
//        public string optionGroupText { get; set; }
//        public List<string> emailAddresses { get; set; }


//    }

//    [Serializable]
//    public class Options
//    {
//        public string optionGroupText { get; set; }
//        public int optionGroupId { get; set; }
//        public List<Value> values { get; set; }

//    }






//    [Serializable]
//    public class Answer
//    {
//        public int id { get; set; }

//        public int questionId
//        {
//            get; set;

//        }




//        //"optionGroupId":
//        private string _answerHeader;
//        public string answerHeader
//        {
//            get { return _answerHeader; }
//            set
//            {
//                if (option != null)
//                {
//                    if (option.optionGroupId > 1)
//                    {
//                        _answerHeader = option.optionGroupText;
//                    }
//                    else
//                    {
//                        _answerHeader = value;
//                    }
//                }
//            }
//        }


//        private string _answerText;
//        public string answerText
//        {
//            get { return _answerText; }
//            set
//            {
//                if (!string.IsNullOrEmpty(answerDate))
//                {
//                    _answerText = answerDate;
//                }
//                else if (userList != null && userList.Count > 0)
//                {
//                    _answerText = string.Join(",", userList.Select(x => x.emailAddress));
//                }
//                else if (emailList != null && emailList.Count > 0)
//                {
//                    _answerText = string.Join(",", emailList.Select(x => x.text));
//                }
//                else if (option != null)
//                {
//                    if (option.isSelected)
//                    {
//                        if (!string.IsNullOrEmpty(option.text))
//                        {
//                            _answerText = option.text;
//                        }
//                    }
//                }
//                else
//                {
//                    _answerText = value;
//                }
//            }
//        }
//        public Option option { get; set; }
//        public bool isDirty { get; set; }
//        public string clauseRef { get; set; }
//        public List<Value> emailList { get; set; }
//        public string answerDate { get; set; }
//        public List<UserList> userList { get; set; }


//    }
//    [Serializable]
//    public class FormName
//    {
//        public int id { get; set; }
//        public string questionText { get; set; }
//        public string questionType { get; set; }
//        public int displayOrder { get; set; }
//        public bool showClauseRef { get; set; }
//        public int maxLength { get; set; }
//        public bool isMandatory { get; set; }
//        public bool isComposite { get; set; }
//        public bool hasChildren { get; set; }
//        public List<Answer> answers { get; set; }
//        public List<object> validations { get; set; }
//        public string mnemonic { get; set; }
//    }




//    [Serializable]
//    public class PrincipalBorrower
//    {
//        public int id { get; set; }
//        public string questionText { get; set; }
//        public string questionType { get; set; }
//        public int displayOrder { get; set; }
//        public bool showClauseRef { get; set; }
//        public int maxLength { get; set; }
//        public bool isMandatory { get; set; }
//        public bool isComposite { get; set; }
//        public bool hasChildren { get; set; }
//        public List<Answer> answers { get; set; }
//        public List<object> validations { get; set; }
//        public string mnemonic { get; set; }
//    }


//    [Serializable]
//    public class ClientNumber
//    {
//        public int id { get; set; }
//        public string questionText { get; set; }
//        public string questionType { get; set; }
//        public int displayOrder { get; set; }
//        public bool showClauseRef { get; set; }
//        public int maxLength { get; set; }
//        public bool isMandatory { get; set; }
//        public bool isComposite { get; set; }
//        public bool hasChildren { get; set; }
//        public List<Answer> answers { get; set; }
//        public List<object> validations { get; set; }
//        public string mnemonic { get; set; }
//    }




//    [Serializable]
//    public class MatterNumber
//    {
//        public int id { get; set; }
//        public string questionText { get; set; }
//        public string questionType { get; set; }
//        public int displayOrder { get; set; }
//        public bool showClauseRef { get; set; }
//        public int maxLength { get; set; }
//        public bool isMandatory { get; set; }
//        public bool isComposite { get; set; }
//        public bool hasChildren { get; set; }
//        public List<Answer> answers { get; set; }
//        public List<object> validations { get; set; }
//        public string mnemonic { get; set; }
//    }


//    [Serializable]
//    public class TransactionDescription
//    {
//        public int id { get; set; }
//        public string questionText { get; set; }
//        public string helpText { get; set; }
//        public string questionType { get; set; }
//        public int displayOrder { get; set; }
//        public bool showClauseRef { get; set; }
//        public int maxLength { get; set; }
//        public bool isMandatory { get; set; }
//        public bool isComposite { get; set; }
//        public bool hasChildren { get; set; }
//        public List<Answer> answers { get; set; }
//        public List<object> validations { get; set; }
//        public string mnemonic { get; set; }
//    }

//    [Serializable]
//    public class StaticQuestions
//    {
//        public FormName formName { get; set; }
//        public PrincipalBorrower principalBorrower { get; set; }
//        public ClientNumber clientNumber { get; set; }
//        public MatterNumber matterNumber { get; set; }
//        public TransactionDescription transactionDescription { get; set; }
//    }

//    [Serializable]
//    public class Value
//    {
//        public string text { get; set; }
//        public int id { get; set; }
//    }

//    [Serializable]
//    public class UserList
//    {
//        public int id { get; set; }
//        public string firstName { get; set; }
//        public string lastName { get; set; }
//        public string emailAddress { get; set; }
//        public string feeEarnerInitials { get; set; }
//    }







//    [Serializable]

//    public class ChildQuestion
//    {
//        public int id { get; set; }
//        public string questionText { get; set; }
//        public string questionType { get; set; }
//        public int displayOrder { get; set; }
//        public bool showClauseRef { get; set; }
//        public bool isMandatory { get; set; }
//        public bool isComposite { get; set; }
//        public bool hasChildren { get; set; }
//        public Options options { get; set; }
//        public List<Answer> answers { get; set; }
//        public List<object> validations { get; set; }
//        public List<ChildQuestion> childQuestions { get; set; }
//        public string mnemonic { get; set; }
//        public int? showChildrenIfOptionId { get; set; }


//    }
//    [Serializable]
//    public class Question
//    {
//        public int id { get; set; }
//        public string questionText { get; set; }
//        public string questionType { get; set; }
//        public int displayOrder { get; set; }
//        public bool showClauseRef { get; set; }
//        public bool isMandatory { get; set; }
//        public bool isComposite { get; set; }
//        public bool hasChildren { get; set; }
//        public Options options { get; set; }
//        public List<Answer> answers { get; set; }
//        public List<object> validations { get; set; }
//        public string mnemonic { get; set; }
//        public string helpText { get; set; }
//        public int? maxLength { get; set; }
//        public List<Question> childQuestions { get; set; }
//        public int? showChildrenIfOptionId { get; set; }
//    }
//    [Serializable]
//    public class QuestionContainer
//    {
//        public int id { get; set; }
//        public string number { get; set; }
//        public string containerText { get; set; }
//        public List<Question> questions { get; set; }
//    }
//    [Serializable]
//    public class Section
//    {
//        public int id { get; set; }
//        public string name { get; set; }
//        public int displayOrder { get; set; }
//        public List<QuestionContainer> questionContainers { get; set; }
//    }
//    [Serializable]
//    public class RootObject
//    {
//        public int id { get; set; }
//        public int templateId { get; set; }
//        public string name { get; set; }
//        public string clientNumber { get; set; }
//        public string matterNumber { get; set; }
//        public StaticQuestions staticQuestions { get; set; }
//        public List<Section> sections { get; set; }
//        public bool isDeprecated { get; set; }
//    }
//}