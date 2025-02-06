using BinanceTestWork.Core.Application.Querries;
using FluentValidation;

namespace BinanceTestWork.Core.Application.Validators
{
    /// <summary>
    /// Класс валидатора для запроса CheckStatusJobQuery.
    /// </summary>
    public class CheckStatusJobQueryValidator : AbstractValidator<CheckStatusJobQuery>
    {
        /// <summary>
        /// Конструктор класса CheckStatusJobQueryValidator.
        /// </summary>
        public CheckStatusJobQueryValidator()
        {
            /// <summary>
            /// Правило валидации для свойства JobId.
            /// </summary>
            RuleFor(x => x.JobId)
                .NotNull()
                .NotEmpty()
                .WithMessage("JobId не должен быть пустым");
        }
    }
}