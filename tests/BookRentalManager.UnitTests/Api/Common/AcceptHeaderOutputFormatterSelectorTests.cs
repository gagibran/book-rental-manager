using BookRentalManager.Api.Common;
using Microsoft.Extensions.Options;

namespace BookRentalManager.UnitTests.Api.Common;

public class AcceptHeaderOutputFormatterSelectorTests
{
    private readonly Mock<IOptions<MvcOptions>> _mvcOptionsStub;
    private readonly Mock<ILoggerFactory> _loggerFactoryStub;
    private readonly AcceptHeaderOutputFormatterSelector _acceptHeaderOutputFormatterSelector;

    public AcceptHeaderOutputFormatterSelectorTests()
    {
        _mvcOptionsStub = new();
        _loggerFactoryStub = new();
        _acceptHeaderOutputFormatterSelector = new(_mvcOptionsStub.Object, _loggerFactoryStub.Object);
    }

    [Fact]
    public void SelectFormatter_WithSelectedFormatterNotNull_ReturnsSelectedFormatter()
    {
        // Arrange:

        // Act:
        // var actualSelectedFormatter = _acceptHeaderOutputFormatterSelector.SelectFormatter();

        // Assert:
    }
}
