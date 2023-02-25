using BookRentalManager.Application.Extensions;

namespace BookRentalManager.UnitTests.Application.Extensions;

public sealed class JsonPatchDocumentExtensionsTests
{
    private readonly PatchCustomerNameAndPhoneNumberDto _patchCustomerNameAndPhoneNumberDto;

    public JsonPatchDocumentExtensionsTests()
    {
        Customer customer = TestFixtures.CreateDummyCustomer();
        _patchCustomerNameAndPhoneNumberDto = new(
            customer.FullName.FirstName,
            customer.FullName.LastName,
            customer.PhoneNumber.AreaCode,
            customer.PhoneNumber.PrefixAndLineNumber);
    }

    [Theory]
    [InlineData("nonexistingOperation", "/areaCode", "Invalid JsonPatch operation 'nonexistingOperation'.")]
    [InlineData("replace", "/nonexistingPath", "The target location specified by path segment 'nonexistingPath' was not found.")]
    [InlineData(" ", "/areaCode", "'operation' cannot be empty.")]
    [InlineData("replace", "", "'path' cannot be empty.")]
    [InlineData("add", "/areaCode", "'add' operation not allowed in this context.")]
    [InlineData("remove", "/areaCode", "'remove' operation not allowed in this context.")]
    public void ApplyToResult_WithIncorrectOperationOrPath_ReturnsErrorMessage(string operation, string path, string expectedErrorMessage)
    {
        // Arrange:
        var operations = new List<Operation<PatchCustomerNameAndPhoneNumberDto>>
        {
            new Operation<PatchCustomerNameAndPhoneNumberDto>(operation, path, It.IsAny<string>(), 222)
        };
        var patchCustomerNameAndPhoneNumberDtoJsonPatchDocument = new JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto>(
            operations,
            new DefaultContractResolver());

        // Act:
        Result applyToResult = JsonPatchDocumentExtensions.ApplyTo(
            patchCustomerNameAndPhoneNumberDtoJsonPatchDocument,
            _patchCustomerNameAndPhoneNumberDto,
            new string[] { "add", "remove" });

        // Assert:
        Assert.Equal(expectedErrorMessage, applyToResult.ErrorMessage);
    }

    [Fact]
    public void ApplyToResult_WithValidOperationOrPath_ReturnsSuccess()
    {
        // Arrange:
        var operations = new List<Operation<PatchCustomerNameAndPhoneNumberDto>>
        {
            new Operation<PatchCustomerNameAndPhoneNumberDto>("replace", "/areaCode", It.IsAny<string>(), 222)
        };
        var patchCustomerNameAndPhoneNumberDtoJsonPatchDocument = new JsonPatchDocument<PatchCustomerNameAndPhoneNumberDto>(
            operations,
            new DefaultContractResolver());

        // Act:
        Result applyToResult = JsonPatchDocumentExtensions.ApplyTo(patchCustomerNameAndPhoneNumberDtoJsonPatchDocument, _patchCustomerNameAndPhoneNumberDto);

        // Assert:
        Assert.True(applyToResult.IsSuccess);
    }
}
