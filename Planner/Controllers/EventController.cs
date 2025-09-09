using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Application.DTOs;
using Application.UseCases.Interfaces;

namespace Presentation.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventUseCase _EventUseCase;

        public EventController(
            IEventUseCase EventUseCase)
        {
            _EventUseCase = EventUseCase;
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        [ServiceFilter(typeof(ValidateModelAttribute<EventModel>))]
        public async Task<ActionResult> AddEvent(EventModel Event)
        {
            var (success, errors) = await _EventUseCase.AddAsync(Event);

            if (!success)
            {
                return BadRequest(new { Errors = errors });
            }

            return RedirectToAction("Main", "Event");
        }

        [HttpGet("Event/AddEvent")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult AddEvent()
        {
            return View();
        }

        public ViewResult Main()
        {
            return View();
        }
    }
}
