using FluentAssertions;
using StruttonTechnologies.Core.Dtos.Person;
using PersonValidatorClass = StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation.PersonValidator;

namespace StruttonTechnologies.Core.Tests.Validators.PersonValidator.MultipleFailures;

public class MultipleValidationFailuresTests
{
    [Fact]
    public void Should_ReturnAllFailures_When_MultipleFieldsAreInvalid()
    {
        var validator = new PersonValidatorClass();
        var dto = new PersonDto
        {
            FirstName = null!,
            LastName = "",
            Email = "invalid"
        };

        var result = validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }
}
