using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace BookRentalManager.Api.Common;

/// <summary>
/// Class responsible for dealing with unrecognized media types in Swagger documentation.
/// Should be added as a singleton service in the <see cref="Program"/> class to replace
/// <see cref="OutputFormatterSelector"/>'s existing implementation.
/// </summary>
/// <param name="mvcOptions">Options for the MVC framework.</param>
/// <param name="loggerFactory"><inheritdoc/></param>
public sealed class AcceptHeaderOutputFormatterSelector(
    IOptions<MvcOptions> mvcOptions,
    ILoggerFactory loggerFactory)
    : OutputFormatterSelector
{
    /// <summary>
    /// Returns an existing formatter for the passed media types if it exists,
    /// otherwise selects the first one that can deals with the passed media types.
    /// </summary>
    /// <param name="outputFormatterCanWriteContext"><inheritdoc/></param>
    /// <param name="outputFormatters">The existing media type formatters that are sent to the method.</param>
    /// <param name="mediaTypeCollection"><inheritdoc/></param>
    /// <returns>The formatter responsible for processing the passed media types.</returns>
    public override IOutputFormatter? SelectFormatter(
        OutputFormatterCanWriteContext outputFormatterCanWriteContext,
        IList<IOutputFormatter> outputFormatters,
        MediaTypeCollection mediaTypeCollection)
    {
        IOutputFormatter? selectedFormatter = new DefaultOutputFormatterSelector(mvcOptions, loggerFactory)
            .SelectFormatter(outputFormatterCanWriteContext, outputFormatters, mediaTypeCollection);
        if (selectedFormatter is not null)
        {
            return selectedFormatter;
        }
        return mvcOptions.Value.OutputFormatters.FirstOrDefault(outputFormatter =>
        {
            return outputFormatter.CanWriteResult(outputFormatterCanWriteContext);
        });
    }
}
