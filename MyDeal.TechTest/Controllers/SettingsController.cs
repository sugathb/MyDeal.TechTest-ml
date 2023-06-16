using MyDeal.TechTest.Models;
using MyDeal.TechTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace MyDeal.TechTest.Controllers
{
    public class SettingsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var user = UserService.GetUserDetails("2")?.Data;
            var response = new SettingsVm
            {
                User = UserService.GetUserDetails("2")?.Data,
                Message = "To be read from app settings"
            };
            return Ok(response);
        }
    }
}