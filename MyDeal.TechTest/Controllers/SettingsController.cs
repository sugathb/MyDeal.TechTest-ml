using MyDeal.TechTest.Models;
using Microsoft.AspNetCore.Mvc;
using MyDeal.TechTest.Core.Services;

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
        public async Task<ActionResult> Index()
        {
            var userData = await _userService.GetUserDetails("2");

            var response = new SettingsVm
            {
                User = userData.Data,
                Message = "To be read from app settings"
            };

            return Ok(response);
        }
    }
}