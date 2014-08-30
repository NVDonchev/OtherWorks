using SimpleSurveyWebsite.ViewModels;
using System.Collections.Generic;

namespace SimpleSurveyWebsite.Repository
{
    public interface ISurveyRepository
    {
        IList<SurveyQuestion> GetAll();
        SurveyQuestion GetQuestionById(int id);
        void SetQuestionStats(IList<SurveyQuestion> surveyQuestions);
        void ResetQuestionStats();
        void Save();
    }
}