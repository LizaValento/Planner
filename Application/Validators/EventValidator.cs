using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class EventValidator : AbstractValidator<EventModel>
    {
        public EventValidator()
        {
            RuleFor(e => e.Title)
                .NotEmpty().WithMessage("Название события обязательно.")
                .Length(1, 100).WithMessage("Название должно быть от 1 до 100 символов.");

            RuleFor(e => e.Description)
                .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов.");

            RuleFor(e => e.Date)
                .GreaterThan(DateTime.Now).WithMessage("Дата события должна быть в будущем.");

            RuleFor(e => e.Location)
                .NotEmpty().WithMessage("Место проведения обязательно.")
                .Length(1, 200).WithMessage("Место проведения должно быть от 1 до 200 символов.");

            RuleFor(e => e.CreatedBy)
                .NotEmpty().WithMessage("Создатель события обязателен.")
                .Length(1, 100).WithMessage("Имя создателя должно быть от 1 до 100 символов.");

            RuleFor(e => e.CreatedAt)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Дата создания не может быть в будущем.");
        }
    }
}
