using Application.DTOs;
using Application.UseCases.Interfaces;
using Application.UseCases.TokenCase;
using Application.UseCases.UserCase;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserUseCase _userUseCase;
        private readonly ITokenUseCase _tokenUseCase;

        public AccountController(ITokenUseCase tokenUseCase, IUserUseCase userUseCase)
        {
            _userUseCase = userUseCase;
            _tokenUseCase = tokenUseCase;
        }

        [HttpGet("Account/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateModelAttribute<RegisterModel>))]
        public IActionResult Register(RegisterModel userModel)
        {
            try
            {
                _userUseCase.Register(userModel);
                return RedirectToAction("Login");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(userModel);
            }
        }

        [HttpGet("Account/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateModelAttribute<LoginModel>))]
        public IActionResult Login(LoginModel userModel)
        {
            try
            {
                var token = _userUseCase.Authenticate(userModel, HttpContext);
                return RedirectToAction("Main", "Book");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            try
            {
                var tokenModel = await _tokenUseCase.RefreshToken(refreshToken, HttpContext); 
                return Ok(tokenModel);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _logoutUseCase.Execute(Response);
            return RedirectToAction("Main", "Book");
        }
    }
}
