using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Application.DTOs;
using Application.UseCases.Interfaces;

namespace Presentation.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserUseCase _userUseCase;

        public UserController(
            IUserUseCase userUseCase)
        {
            _userUseCase = userUseCase;
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(UserModel user)
        {
            _userUseCase.AddAsync(user);
            return RedirectToAction("Index");
        }

        [HttpGet("User/AddUser")]
        public ActionResult AddUser()
        {
            return View();
        }
    }
}
