namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class PhoneNumberTests
{
    private const string ExpectedAreaCodeErrorMessage = "Invalid area code.";
    private const string ExpectedActualPhoneNumberErrorMessage = "Invalid phone number.";

    [Theory]
    [InlineData(39, 3345679, ExpectedAreaCodeErrorMessage)]
    [InlineData(3432, 3345679, ExpectedAreaCodeErrorMessage)]
    [InlineData(564, 33456, ExpectedActualPhoneNumberErrorMessage)]
    [InlineData(564, 34533456, ExpectedActualPhoneNumberErrorMessage)]
    public void Create_WithInvalidPhoneNumber_ReturnErrorMessage(
        int areaCode,
        int actualPhoneNumber,
        string expectedErrorMessage)
    {
        // Act:
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(areaCode, actualPhoneNumber);

        // Assert:
        Assert.Equal(expectedErrorMessage, phoneNumberResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithValidData_ReturnSuccess()
    {
        // Act:
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(564, 3345679);

        // Assert:
        Assert.True(phoneNumberResult.IsSuccess);
    }
}
