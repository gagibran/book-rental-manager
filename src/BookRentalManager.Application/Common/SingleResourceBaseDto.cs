namespace BookRentalManager.Application.Common;

public class SingleResourceBaseDto
{
    public Guid Id { get; }
    public List<HateoasLinkDto>? Links { get; set; }

    public SingleResourceBaseDto(Guid id)
    {
        Id = id;
    }
}
