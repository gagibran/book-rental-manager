using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace BookRentalManager.Api.Common;

#pragma warning disable CS1591
public class AcceptHeaderOutputFormatterSelector(
    IOptions<MvcOptions> mvcOptions,
    ILoggerFactory loggerFactory)
    : OutputFormatterSelector
{
    private readonly IOptions<MvcOptions> _mvcOptions = mvcOptions;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;

    public override IOutputFormatter? SelectFormatter(
        OutputFormatterCanWriteContext outputFormatterCanWriteContext,
        IList<IOutputFormatter> outputFormatters,
        MediaTypeCollection mediaTypes)
    {
        IOutputFormatter? selectedFormatter = new DefaultOutputFormatterSelector(_mvcOptions, _loggerFactory)
            .SelectFormatter(outputFormatterCanWriteContext, outputFormatters, mediaTypes);
        if (selectedFormatter is not null)
        {
            return selectedFormatter;
        }
        return _mvcOptions.Value.OutputFormatters.FirstOrDefault(outputFormatter =>
        {
            return outputFormatter.CanWriteResult(outputFormatterCanWriteContext);
        });
    }
}
#pragma warning restore CS1591
