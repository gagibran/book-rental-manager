namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class PhoneNumberTests
{
    private const string ExpectedAreaCodeErrorMessage = "Invalid area code.";
    private const string ExpectedPrefixAndLineNumberErrorMessage = "Invalid phone number.";

    [Theory]
    [InlineData(39, 3345679, ExpectedAreaCodeErrorMessage)]
    [InlineData(3432, 3345679, ExpectedAreaCodeErrorMessage)]
    [InlineData(564, 33456, ExpectedPrefixAndLineNumberErrorMessage)]
    [InlineData(564, 34533456, ExpectedPrefixAndLineNumberErrorMessage)]
    public void Create_WithInvalidPhoneNumber_ReturnsErrorMessage(
        int areaCode,
        int prefixAndLineNumber,
        string expectedErrorMessage)
    {
        // Act:
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(areaCode, prefixAndLineNumber);

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
