using FluentAssertions;
using StruttonTechnologies.Core.Dtos.Person;
using PersonValidatorClass = StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation.PersonValidator;

namespace StruttonTechnologies.Core.Tests.Validators.PersonValidator.ValidInput;

public class ValidPersonTests
{
    [Fact]
    public void Should_ReturnSuccess_When_AllFieldsAreValid()
    {
        var validator = new PersonValidatorClass();
        var dto = new PersonDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com"
        };

        var result = validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Message.Should().BeNull();
    }
}
