using FluentValidation;
using SimpleApi.Shared.Requests;

namespace SimpleApi.BusinessLayer;

public class SavePersonValidator : AbstractValidator<SavePersonRequest>
{
    public SavePersonValidator()
    {
        RuleFor(p => p.FirstName)
            .NotNull()
            .NotEmpty()
            .WithMessage("insert a valid first name");

        RuleFor(p => p.LastName)
            .NotNull()
            .NotEmpty()
            .WithMessage("insert a valid last name");
    }
}