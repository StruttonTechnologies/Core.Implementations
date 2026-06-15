using FluentAssertions;
using StruttonTechnologies.Core.Dtos.Person;
using PersonValidatorClass = StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation.PersonValidator;

namespace StruttonTechnologies.Core.Tests.Validators.PersonValidator.InvalidEmail;

public class InvalidEmailTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    [InlineData("invalid.com")]
    public void Should_ReturnFailure_When_EmailIsInvalid(string? email)
    {
        var validator = new PersonValidatorClass();
        var dto = new PersonDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = email!
        };

        var result = validator.Validate(dto);

        result.IsValid.Should().BeFalse();
    }
}
