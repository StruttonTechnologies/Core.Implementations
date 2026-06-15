using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.ToolKit.Validation.Abstractions;
using StruttonTechnologies.Core.ToolKit.Validation.Models;
using StruttonTechnologies.Core.ToolKit.Validation.Validators.Format;

namespace StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation;

public class PersonValidator : IValidator<PersonDto>
{
    private readonly EmailFormatValidator _emailValidator = new();

    public ValidationResult Validate(PersonDto input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (string.IsNullOrWhiteSpace(input.FirstName))
        {
            return ValidationResult.Failure(
                "First name is required.",
                "MissingFirstName",
                nameof(input.FirstName));
        }

        if (string.IsNullOrWhiteSpace(input.LastName))
        {
            return ValidationResult.Failure(
                "Last name is required.",
                "MissingLastName",
                nameof(input.LastName));
        }

        ValidationResult emailResult = _emailValidator.Validate(input.Email);

        if (!emailResult.IsValid)
        {
            return emailResult;
        }

        return ValidationResult.Success();
    }
}
