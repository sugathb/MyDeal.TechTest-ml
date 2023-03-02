using MyDeal.TechTest.Models;
using System.Configuration;
using System.Web.Mvc;
using MyDeal.TechTest.Services;

namespace MyDeal.TechTest.Controllers
{
    public class SettingsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return Json(new SettingsVm
            {
                User = UserService.GetUserDetails("2")?.Data,
                Message = ConfigurationManager.AppSettings["Settings:Message"]
            }, JsonRequestBehavior.AllowGet);
        }
    }
}