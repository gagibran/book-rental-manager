namespace BookRentalManager.Application.Dtos;

public sealed class AllowedRestMethodsDto
{
    public string Method { get; }
    public string MethodName { get; }
    public string Rel { get; }

    public AllowedRestMethodsDto (string method, string methodName, string rel)
    {
        Method = method;
        MethodName = methodName;
        Rel = rel;
    }
}
