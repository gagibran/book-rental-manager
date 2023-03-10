namespace BookRentalManager.Application.Authors.CommandHandlers;

internal sealed class CreateAuthorCommandHandler : ICommandHandler<CreateAuthorCommand, AuthorCreatedDto>
{
    private readonly IRepository<Author> _authorRepository;
    private readonly IMapper<Author, AuthorCreatedDto> _authorToAuthorCreatedDtoMapper;

    public CreateAuthorCommandHandler(
        IRepository<Author> authorRepository,
        IMapper<Author, AuthorCreatedDto> authorToAuthorCreatedDtoMapper)
    {
        _authorRepository = authorRepository;
        _authorToAuthorCreatedDtoMapper = authorToAuthorCreatedDtoMapper;
    }

    public async Task<Result<AuthorCreatedDto>> HandleAsync(
        CreateAuthorCommand createAuthorCommand,
        CancellationToken cancellationToken)
    {
        var authorByFullNameSpecification = new AuthorByFullNameSpecification(createAuthorCommand.FirstName, createAuthorCommand.LastName);
        Author? existingAuthor = await _authorRepository.GetFirstOrDefaultBySpecificationAsync(authorByFullNameSpecification);
        Result existingAuthorResult = Result.Success();
        if (existingAuthor is not null)
        {
            existingAuthorResult = Result.Fail(
                "authorAlreadyExists",
                $"An author named '{existingAuthor.FullName.ToString()}' already exists.");
        }
        Result<FullName> authorFullNameResult = FullName.Create(createAuthorCommand.FirstName, createAuthorCommand.LastName);
        Result combinedResults = Result.Combine(existingAuthorResult, authorFullNameResult);
        if (!combinedResults.IsSuccess)
        {
            return Result.Fail<AuthorCreatedDto>(combinedResults.ErrorType, combinedResults.ErrorMessage);
        }
        var newAuthor = new Author(authorFullNameResult.Value!);
        await _authorRepository.CreateAsync(newAuthor, cancellationToken);
        return Result.Success(_authorToAuthorCreatedDtoMapper.Map(newAuthor));
    }
}
