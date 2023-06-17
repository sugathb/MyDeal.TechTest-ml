using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyDeal.TechTest.Core.Queries;

namespace MyDeal.TechTest.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IMediator _mediator;
        private const string UserId = "2";

        public SettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var getUserDetailsQuery = new GetUserSettingsAsyncQuery { UserId = UserId };
            var response = await _mediator.Send(getUserDetailsQuery);

            if (response.User == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}