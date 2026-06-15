using FluentAssertions;
using StruttonTechnologies.Core.Dtos.Person;
using PersonValidatorClass = StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation.PersonValidator;

namespace StruttonTechnologies.Core.Tests.Validators.PersonValidator.MissingLastName;

public class MissingLastNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_ReturnFailure_When_LastNameIsNullOrWhitespace(string? lastName)
    {
        var validator = new PersonValidatorClass();
        var dto = new PersonDto
        {
            FirstName = "John",
            LastName = lastName!,
            Email = "john.doe@test.com"
        };

        var result = validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Last name is required");
    }
}
