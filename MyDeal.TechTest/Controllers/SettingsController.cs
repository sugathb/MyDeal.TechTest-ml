using MyDeal.TechTest.Models;
using MyDeal.TechTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace MyDeal.TechTest.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IUserService _userService;

        public SettingsController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var response = new SettingsVm
            {
                User = _userService.GetUserDetails("2")?.Data,
                Message = "To be read from app settings"
            };
            return Ok(response);
        }
    }
}