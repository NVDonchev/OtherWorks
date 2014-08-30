using System.Collections.Generic;

namespace SimpleSurveyWebsite.ViewModels
{
    public class SurveyQuestion
    {
        public SurveyQuestion()
        {
            AnswersStats = new List<int>() { 0, 0, 0 };
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public IEnumerable<string> Answers { get; set; }
        public IList<int> AnswersStats { get; set; }
    }
}