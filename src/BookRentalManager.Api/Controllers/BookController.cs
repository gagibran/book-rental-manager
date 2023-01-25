using BookRentalManager.Application.Books.Commands;
using BookRentalManager.Application.Books.Queries;

namespace BookRentalManager.Api.Controllers;

[Route("api/bookauthor/{bookAuthorId}/[controller]")]
public sealed class BookController : BaseController
{
    public BookController(IDispatcher dispatcher, ILogger<BookController> bookAuthorControllerLogger)
        : base(dispatcher, bookAuthorControllerLogger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetBookDto>>> GetBooksFromBookAuthorAsync(
        Guid bookAuthorId,
        CancellationToken cancellationToken,
        int pageIndex = 1,
        int totalItemsPerPage = 50,
        [FromQuery(Name = "search")] string searchParameter = "")
    {
        var getBooksBySearchParameterQuery = new GetBooksBySearchParameterQuery(
            bookAuthorId,
            pageIndex,
            totalItemsPerPage,
            searchParameter);
        Result<IReadOnlyList<GetBookDto>> getAllBooksResult = await _dispatcher.DispatchAsync<IReadOnlyList<GetBookDto>>(
                getBooksBySearchParameterQuery,
                cancellationToken);
        if (!getAllBooksResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getAllBooksResult.ErrorMessage);
            return NotFound(getAllBooksResult.ErrorMessage);
        }
        return Ok(getAllBooksResult.Value);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetBookByIdFromBookAuthorAsync))]
    public async Task<ActionResult<GetBookDto>> GetBookByIdFromBookAuthorAsync(Guid bookAuthorId, Guid id, CancellationToken cancellationToken)
    {
        var getBookByIdQuery = new GetBookByIdQuery(bookAuthorId, id);
        Result<GetBookDto> getBookByIdResult = await _dispatcher.DispatchAsync<GetBookDto>(getBookByIdQuery, cancellationToken);
        if (!getBookByIdResult.IsSuccess)
        {
            _baseControllerLogger.LogError(getBookByIdResult.ErrorMessage);
            return NotFound(getBookByIdResult.ErrorMessage);
        }
        return Ok(getBookByIdResult.Value);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBookForBookAuthorAsync(
        Guid bookAuthorId,
        CreateBookDto createBookDto,
        CancellationToken cancellationToken)
    {
        Result<Edition> editionResult = Edition.Create(createBookDto.Edition);
        Result<Isbn> isbnResult = Isbn.Create(createBookDto.Isbn);
        Result combinedResults = Result.Combine(editionResult, isbnResult);
        if (!combinedResults.IsSuccess)
        {
            _baseControllerLogger.LogError(combinedResults.ErrorMessage);
            return BadRequest(combinedResults.ErrorMessage);
        }
        var newBook = new Book(createBookDto.BookTitle, editionResult.Value!, isbnResult.Value!);
        Result createBookResult = await _dispatcher.DispatchAsync(new CreateBookCommand(bookAuthorId, newBook), cancellationToken);
        if (!createBookResult.IsSuccess)
        {
            _baseControllerLogger.LogError(createBookResult.ErrorMessage);
            return BadRequest(createBookResult.ErrorMessage);
        }
        var bookCreatedDto = new BookCreatedDto(
            newBook.Id,
            bookAuthorId,
            newBook.BookTitle,
            newBook.Edition.EditionNumber,
            newBook.Isbn.IsbnValue);
        return CreatedAtAction(nameof(GetBookByIdFromBookAuthorAsync), new { bookAuthorId, id = newBook.Id }, bookCreatedDto);
    }
}
