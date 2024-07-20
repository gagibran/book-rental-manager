namespace BookRentalManager.UnitTests.Domain.ValueObjects;

public sealed class PhoneNumberTests
{
    private const string ExpectedAreaCodeErrorMessage = "Invalid area code.";
    private const string ExpectedPrefixAndLineNumberErrorMessage = "Invalid phone number.";
    private const string ExpectedAreaCodeErrorType = "invalidAreaCode";
    private const string ExpectedPrefixAndLineNumberErrorType = "invalidPhoneNumber";

    [Theory]
    [InlineData(39, 3345679, ExpectedAreaCodeErrorType, ExpectedAreaCodeErrorMessage)]
    [InlineData(3432, 3345679, ExpectedAreaCodeErrorType, ExpectedAreaCodeErrorMessage)]
    [InlineData(564, 33456, ExpectedPrefixAndLineNumberErrorType, ExpectedPrefixAndLineNumberErrorMessage)]
    [InlineData(564, 34533456, ExpectedPrefixAndLineNumberErrorType, ExpectedPrefixAndLineNumberErrorMessage)]
    public void Create_WithInvalidPhoneNumber_ReturnsErrorMessage(
        int areaCode,
        int prefixAndLineNumber,
        string expectedErrorType,
        string expectedErrorMessage)
    {
        // Act:
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(areaCode, prefixAndLineNumber);

        // Assert:
        Assert.Equal(expectedErrorType, phoneNumberResult.ErrorType);
        Assert.Equal(expectedErrorMessage, phoneNumberResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Act:
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(564, 3345679);

        // Assert:
        Assert.True(phoneNumberResult.IsSuccess);
    }
}
