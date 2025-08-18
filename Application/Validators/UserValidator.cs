using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator()
        {
            RuleFor(user => user.FirstName)
                .Length(1, 50).WithMessage("Имя должно содержать от 1 до 50 символов.");

            RuleFor(user => user.LastName)
                .Length(1, 50).WithMessage("Фамилия должна содержать от 1 до 50 символов.");

            RuleFor(user => user.Email)
                .Length(1, 50).WithMessage("Email должен содержать от 1 до 50 символов.");

            RuleFor(user => user.Nickname)
                .NotEmpty().WithMessage("Никнейм обязателен.")
                .Length(3, 20).WithMessage("Никнейм должен содержать от 3 до 20 символов.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Пароль обязателен.")
                .MinimumLength(6).WithMessage("Пароль должен содержать не менее 6 символов.");
        }
    }
}
