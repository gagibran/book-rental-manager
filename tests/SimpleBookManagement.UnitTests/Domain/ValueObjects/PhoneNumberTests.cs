namespace SimpleBookManagement.UnitTests.Domain.ValueObjects;

public sealed class PhoneNumberTests
{
    private readonly string _expectedAreaCodeErrorMessage;
    private readonly string _expectedActualPhoneNumberErrorMessage;

    public PhoneNumberTests()
    {
        _expectedAreaCodeErrorMessage = "Invalid area code.";
        _expectedActualPhoneNumberErrorMessage = "Invalid phone number.";
    }

    [Fact]
    public void Create_WithAreaCodeSmallerThanTheAllowedMinimum_ReturnErrorMessage()
    {
        // Act:
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(39, 3345679);

        // Assert:
        Assert.Equal(_expectedAreaCodeErrorMessage, phoneNumberResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithAreaCodeSmallerThanTheAllowedMaximum_ReturnErrorMessage()
    {
        // Act:
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(3432, 3345679);

        // Assert:
        Assert.Equal(_expectedAreaCodeErrorMessage, phoneNumberResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithActualPhoneNumberSmallerThanTheAllowedMinimum_ReturnErrorMessage()
    {
        // Act:
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(564, 33456);

        // Assert:
        Assert.Equal(_expectedActualPhoneNumberErrorMessage, phoneNumberResult.ErrorMessage);
    }

    [Fact]
    public void Create_WithActualPhoneNumberSmallerThanTheAllowedMaximum_ReturnErrorMessage()
    {
        // Act:
        Result<PhoneNumber> phoneNumberResult = PhoneNumber.Create(564, 34533456);

        // Assert:
        Assert.Equal(_expectedActualPhoneNumberErrorMessage, phoneNumberResult.ErrorMessage);
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
