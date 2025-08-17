using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class LoginValidator : AbstractValidator<LoginModel>
    {
        public LoginValidator()
        {
            RuleFor(user => user.Nickname)
                .NotEmpty().WithMessage("Никнейм обязателен.")
                .Length(3, 20).WithMessage("Никнейм должен содержать от 3 до 20 символов.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Пароль обязателен.")
                .MinimumLength(6).WithMessage("Пароль должен содержать не менее 6 символов.");

        }
    }
}
