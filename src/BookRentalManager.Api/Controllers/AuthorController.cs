using BookRentalManager.Application.Authors.Commands;
using BookRentalManager.Application.Authors.Queries;

namespace BookRentalManager.Api.Controllers;

[Route("api/[controller]")]
public sealed class AuthorController : ApiController
{
    public AuthorController(IDispatcher dispatcher, ILogger<AuthorController> authorControllerLogger)
        : base(dispatcher, authorControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<GetAuthorDto>>> GetAuthorsByQueryParametersAsync(
        [FromQuery] GetAllItemsQueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var getAuthorsByQueryParametersQuery = new GetAuthorsByQueryParametersQuery(
            queryParameters.PageIndex,
            queryParameters.PageSize,
            queryParameters.SearchQuery,
            queryParameters.SortBy);
        Result<PaginatedList<GetAuthorDto>> getAllAuthorsResult = await _dispatcher.DispatchAsync<PaginatedList<GetAuthorDto>>(
                getAuthorsByQueryParametersQuery,
                cancellationToken);
        if (!getAllAuthorsResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getAllAuthorsResult.ErrorMessage);
            return CustomHttpErrorResponse(getAllAuthorsResult.ErrorType, getAllAuthorsResult.ErrorMessage, HttpStatusCode.BadRequest);
        }
        CreatePagingMetadata(
            nameof(GetAuthorsByQueryParametersAsync),
            queryParameters.SearchQuery,
            queryParameters.SortBy,
            getAllAuthorsResult.Value!);
        return Ok(getAllAuthorsResult.Value);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetAuthorByIdAsync))]
    public async Task<ActionResult<GetAuthorDto>> GetAuthorByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<GetAuthorDto> getAuthorByIdResult = await _dispatcher.DispatchAsync<GetAuthorDto>(
            new GetAuthorByIdQuery(id),
            cancellationToken);
        if (!getAuthorByIdResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getAuthorByIdResult.ErrorMessage);
            return CustomHttpErrorResponse(getAuthorByIdResult.ErrorType, getAuthorByIdResult.ErrorMessage, HttpStatusCode.NotFound);
        }
        return Ok(getAuthorByIdResult.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAuthorAsync(CreateAuthorCommand createAuthorCommand, CancellationToken cancellationToken)
    {
        Result<AuthorCreatedDto> createAuthorResult = await _dispatcher.DispatchAsync<AuthorCreatedDto>(
            createAuthorCommand,
            cancellationToken);
        if (!createAuthorResult.IsSuccess)
        {
            _baseControllerLogger.LogError(createAuthorResult.ErrorMessage);
            return CustomHttpErrorResponse(
                createAuthorResult.ErrorType,
                createAuthorResult.ErrorMessage,
                HttpStatusCode.UnprocessableEntity);
        }
        return CreatedAtAction(nameof(GetAuthorByIdAsync), new { id = createAuthorResult.Value!.Id }, createAuthorResult.Value);
    }
}
