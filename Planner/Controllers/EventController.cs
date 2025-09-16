using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.UseCases.Interfaces;

namespace Presentation.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventUseCase _EventUseCase;

        public EventController(IEventUseCase EventUseCase)
        {
            _EventUseCase = EventUseCase;
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        [ServiceFilter(typeof(ValidateModelAttribute<EventModel>))]
        public ActionResult AddEvent(EventModel Event)
        {
            var (success, errors) = _EventUseCase.Add(Event);

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
