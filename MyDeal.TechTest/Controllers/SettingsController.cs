using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyDeal.TechTest.Core.Queries;

namespace MyDeal.TechTest.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IMediator _mediator;

        public SettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var getUserDetailsQuery = new GetUserSettingsAsyncQuery { UserId = "2" };
            var response = await _mediator.Send(getUserDetailsQuery);
            return Ok(response);
        }
    }
}