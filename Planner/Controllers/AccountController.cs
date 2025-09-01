using Application.DTOs;
using Application.UseCases.TokenCase;
using Application.UseCases.UserCase;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly RegisterUserUseCase _registerUserUseCase;
        private readonly AuthenticateUserUseCase _authenticateUserUseCase;
        private readonly RefreshTokenUseCase _refreshTokenUseCase;
        private readonly LogoutUseCase _logoutUseCase;

        public AccountController(
            RegisterUserUseCase registerUserUseCase,
            AuthenticateUserUseCase authenticateUserUseCase,
            RefreshTokenUseCase refreshTokenUseCase,
            LogoutUseCase logoutUseCase)
        {
            _registerUserUseCase = registerUserUseCase;
            _authenticateUserUseCase = authenticateUserUseCase;
            _refreshTokenUseCase = refreshTokenUseCase;
            _logoutUseCase = logoutUseCase;
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
                _registerUserUseCase.Execute(userModel);
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
                var token = _authenticateUserUseCase.Execute(userModel, HttpContext);
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
                var tokenModel = await _refreshTokenUseCase.ExecuteAsync(refreshToken, HttpContext); 
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
