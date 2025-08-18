using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Application.DTOs;
using Application.UseCases.UserCase;

namespace Presentation.Controllers
{
    public class UserController : Controller
    {
        private readonly AddUserUseCase _addUserUseCase;
        private readonly GetUserBooksUseCase _getUserBooksUseCase;

        public UserController(
            AddUserUseCase addUserUseCase,
            GetUserBooksUseCase getUserBooksUseCase)
        {
            _addUserUseCase = addUserUseCase;
            _getUserBooksUseCase = getUserBooksUseCase;
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(UserModel user)
        {
            _addUserUseCase.Execute(user);
            return RedirectToAction("Index");
        }

        [HttpGet("User/AddUser")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpGet("User/ViewUserBooks")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult ViewUserBooks(int page = 1, int pageSize = 2)
        {
            var model = _getUserBooksUseCase.Execute(User, page, pageSize);
            return View(model);
        }
    }
}
