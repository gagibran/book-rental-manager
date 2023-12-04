namespace BookRentalManager.Application.Dtos;

public sealed class AllowedRestMethodsDto(string method, string methodName, string rel)
{
    public string Method { get; } = method;
    public string MethodName { get; } = methodName;
    public string Rel { get; } = rel;
}
