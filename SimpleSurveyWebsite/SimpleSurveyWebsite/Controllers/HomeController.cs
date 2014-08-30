using SimpleSurveyWebsite.Repository;
using SimpleSurveyWebsite.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SimpleSurveyWebsite.Controllers
{
    public class HomeController : Controller
    {
        private ISurveyRepository xmlManager = new SurveyRepository();
        private IList<SurveyQuestion> model = new List<SurveyQuestion>();

        //
        // GET: /Home/
        [HttpGet]
        public ActionResult Index()
        {
            // getting question info from XML file
            var model = xmlManager.GetAll();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            // getting question info from XML file
            var model = xmlManager.GetAll();
            int questionId = 0;

            // updating model stats according to the form content (radio buttons)
            foreach (var key in form.AllKeys)
            {
                var answerId = int.Parse(form[key]);
                model[questionId].AnswersStats[answerId]++;
                questionId++;
            }

            // saving the updated model to the XML file
            xmlManager.SetQuestionStats(model);
            xmlManager.Save();

            return View("SurveyStats", model);
        }

        public ActionResult ResetStats()
        {            
            // reseting the question statistics
            xmlManager.ResetQuestionStats();
            xmlManager.Save();

            return RedirectToAction("Index");
        }
    }
}