namespace BookRentalManager.Application.Authors.CommandHandlers;

internal sealed class CreateAuthorCommandHandler(IRepository<Author> authorRepository) : IRequestHandler<CreateAuthorCommand, AuthorCreatedDto>
{
    private readonly IRepository<Author> _authorRepository = authorRepository;

    public async Task<Result<AuthorCreatedDto>> HandleAsync(
        CreateAuthorCommand createAuthorCommand,
        CancellationToken cancellationToken)
    {
        var authorByFullNameSpecification = new AuthorByFullNameSpecification(createAuthorCommand.FirstName, createAuthorCommand.LastName);
        Author? existingAuthor = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByFullNameSpecification, cancellationToken);
        Result existingAuthorResult = Result.Success();
        if (existingAuthor is not null)
        {
            existingAuthorResult = Result.Fail(
                "authorAlreadyExists",
                $"An author named '{existingAuthor.FullName}' already exists.");
        }
        Result<FullName> authorFullNameResult = FullName.Create(createAuthorCommand.FirstName, createAuthorCommand.LastName);
        Result combinedResults = Result.Combine(existingAuthorResult, authorFullNameResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<AuthorCreatedDto>(combinedResults.ErrorType, combinedResults.ErrorMessage);
        }
        var newAuthor = new Author(authorFullNameResult.Value!);
        await _authorRepository.CreateAsync(newAuthor, cancellationToken);
        return Result.Success(new AuthorCreatedDto(newAuthor));
    }
}
