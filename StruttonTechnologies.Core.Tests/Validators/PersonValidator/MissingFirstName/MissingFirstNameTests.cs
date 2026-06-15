using FluentAssertions;
using StruttonTechnologies.Core.Dtos.Person;
using PersonValidatorClass = StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation.PersonValidator;

namespace StruttonTechnologies.Core.Tests.Validators.PersonValidator.MissingFirstName;

public class MissingFirstNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_ReturnFailure_When_FirstNameIsNullOrWhitespace(string? firstName)
    {
        var validator = new PersonValidatorClass();
        var dto = new PersonDto
        {
            FirstName = firstName!,
            LastName = "Doe",
            Email = "john.doe@test.com"
        };

        var result = validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("First name is required");
    }
}
