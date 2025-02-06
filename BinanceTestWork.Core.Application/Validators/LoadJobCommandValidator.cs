using BinanceTestWork.Core.Application.Commands;
using FluentValidation;

namespace BinanceTestWork.Core.Application.Validators
{
    /// <summary>
    /// Класс валидатора для команды LoadJobCommand.
    /// </summary>
    public class LoadJobCommandValidator : AbstractValidator<LoadJobCommand>
    {
        /// <summary>
        /// Конструктор класса LoadJobCommandValidator.
        /// </summary>
        public LoadJobCommandValidator()
        {
            /// <summary>
            /// Правило валидации для свойства Pairs.
            /// </summary>
            RuleFor(x => x.Pairs)
                .NotEmpty()
                .WithMessage("Валютная пара не должна быть пустой");

            /// <summary>
            /// Проверку на допустимые символы для свойства Pairs.
            /// </summary>
            RuleForEach(x => x.Pairs)
                .Matches(@"^[A-Z0-9]+$")
                .WithMessage("Валютная пара должна содержать только буквы и цифры.");

            /// <summary>
            /// Правило валидации для свойства StartDate.
            /// </summary>
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Дата начала не должна быть пустой")
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Дата начала не должна превышать конечную дату");
        }
    }
}