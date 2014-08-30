using SimpleSurveyWebsite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SimpleSurveyWebsite.Repository
{
    public class SurveyRepository : ISurveyRepository
    {
        private XDocument document;
        private IEnumerable<XElement> documentContent;
        private readonly string pathToFile;

        public SurveyRepository()
        {
            // getting the app path
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            pathToFile = appDir + @"/DataXml/SurveyData.xml";

            // getting the XML document
            document = XDocument.Load(pathToFile);
            documentContent = document.Elements();
        }

        public IList<SurveyQuestion> GetAll()
        {
            var surveyQuestions = new List<SurveyQuestion>();

            // iterating through the question nodes and getting the question info
            foreach (var questionNode in documentContent.Elements("question"))
            {
                var id = int.Parse(questionNode.Attribute("id").Value);
                var question = questionNode.Element("questionText").Value;
                var answers = new List<string>();
                var answersStats = new List<int>();

                // iterating through the answers and getting the answer info
                foreach (var answerNode in questionNode.Elements("answers").Elements("answerText"))
                {
                    answers.Add(answerNode.Value);
                    answersStats.Add(int.Parse(answerNode.Attribute("choiceCounter").Value));
                }

                // initializing a SurveyQuestion object and setting it's properties using the values extracted from the XML
                surveyQuestions.Add(new SurveyQuestion() { Id = id, Text = question, Answers = answers, AnswersStats = answersStats });
            }

            return surveyQuestions;
        }

        public SurveyQuestion GetQuestionById(int id)
        {
            // getting the appropriate question node and it's text from the XML based on the given id
            var questionNode = document.Element("questions").Elements("question").First(x => x.Attribute("id").Value == id.ToString());
            var questionText = questionNode.Element("questionText").Value;

            var answers = new List<string>();
            var answersStats = new List<int>();

            // iterating through the answers and getting the answer info
            foreach (var answerNode in questionNode.Elements("answers").Elements("answerText"))
            {
                answers.Add(answerNode.Value);
                answersStats.Add(int.Parse(answerNode.Attribute("choiceCounter").Value));
            }

            // initializing a SurveyQuestion object and setting it's properties using the values extracted from the XML
            return new SurveyQuestion() { Id = id, Text = questionText, Answers = answers, AnswersStats = answersStats };
        }

        public void SetQuestionStats(IList<SurveyQuestion> surveyQuestions)
        {
            // current question counter
            var currentQuestion = 0;

            // iterating through the XML question nodes
            foreach (var questionNode in documentContent.Elements("question"))
            {
                // current answer counter
                var currentAnswer = 0;

                // iterating through the answers and setting statistical information
                foreach (var answerNode in questionNode.Elements("answers").Elements("answerText"))
                {
                    answerNode.Attribute("choiceCounter").Value = surveyQuestions[currentQuestion].AnswersStats[currentAnswer].ToString();
                    currentAnswer++;
                }

                currentQuestion++;
            }
        }

        public void ResetQuestionStats()
        {
            // iterating through the XML question nodes
            foreach (var questionNode in documentContent.Elements("question"))
            {
                // iterating through the answers and setting statistical information
                foreach (var answerNode in questionNode.Elements("answers").Elements("answerText"))
                {
                    answerNode.Attribute("choiceCounter").Value = "0";
                }
            }
        }

        public void Save()
        {
            // saving the changes to an XML document
            document.Save(pathToFile);
        }
    }
}