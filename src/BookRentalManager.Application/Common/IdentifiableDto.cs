namespace BookRentalManager.Application.Common;

[method: JsonConstructor]
public abstract record IdentifiableDto(Guid Id);
