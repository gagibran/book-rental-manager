using System.Text.Json;
using BookRentalManager.Application.Common;

namespace BookRentalManager.Api.Common;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected ActionResult HandleError(Result result)
    {
        switch (result.ErrorType)
        {
            case string error when error.ToLower().Contains("id"):
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.NotFound);
            case "jsonPatch":
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.BadRequest);
            default:
                return CustomHttpErrorResponse(result.ErrorType, result.ErrorMessage, HttpStatusCode.UnprocessableEntity);
        }
    }

    protected void CreatePagingMetadata<TItem>(
        string routeName,
        string searchQuery,
        string sortQuery,
        PaginatedList<TItem> paginatedList)
    {
        string? previousPageLink = null;
        string? nextPageLink = null;
        if (paginatedList.HasPreviousPage)
        {
            previousPageLink = Url.Link(routeName, new
            {
                PageIndex = paginatedList.PageIndex - 1,
                PageSize = paginatedList.PageSize,
                SearchQuery = searchQuery,
                SortBy = sortQuery
            });
        }
        if (paginatedList.HasNextPage)
        {
            nextPageLink = Url.Link(routeName, new
            {
                PageIndex = paginatedList.PageIndex + 1,
                PageSize = paginatedList.PageSize,
                SearchQuery = searchQuery,
                SortBy = sortQuery
            });
        }
        int totalAmountOfPages = paginatedList.TotalAmountOfPages == int.MinValue ? 0 : paginatedList.TotalAmountOfPages;
        string serializedMetadata = JsonSerializer.Serialize(
            new
            {
                TotalAmountOfItems = paginatedList.TotalAmountOfItems,
                PageIndex = paginatedList.PageIndex,
                PageSize = paginatedList.PageSize,
                TotalAmountOfPages = totalAmountOfPages,
                PreviousPageLink = previousPageLink,
                NextPageLink = nextPageLink
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        Response.Headers.Add("X-Pagination", serializedMetadata);
    }

    protected ActionResult CustomHttpErrorResponse(string errorTypes, string errorMessages, HttpStatusCode httpStatusCode)
    {
        AddErrorsToModelState(errorTypes, errorMessages);
        return ValidationProblem(modelStateDictionary: ModelState, statusCode: (int)httpStatusCode);
    }

    protected SingleResourceBaseDto AddHateoasLinks(List<AllowedRestMethodsDto> allowedRestMethodDtos, SingleResourceBaseDto singleResourceBaseDto)
    {
        if (!allowedRestMethodDtos.Any())
        {
            throw new ArgumentException($"{allowedRestMethodDtos} cannot be empty.");
        }
        var hateoasLinkDtos = new List<HateoasLinkDto>();
        foreach (AllowedRestMethodsDto allowedRestMethodDto in allowedRestMethodDtos)
        {
            string? href = Url.Link(allowedRestMethodDto.Method, new { Id = singleResourceBaseDto.Id });
            var hateoasLinkDto = new HateoasLinkDto(href!, allowedRestMethodDto.Rel, allowedRestMethodDto.MethodName);
            hateoasLinkDtos.Add(hateoasLinkDto);
        }
        singleResourceBaseDto.Links = hateoasLinkDtos;
        return singleResourceBaseDto;
    }

    protected PaginatedList<SingleResourceBaseDto> AddHateoasLinksToPaginatedCollection<TDto>(List<AllowedRestMethodsDto> allowedRestMethodDtos, PaginatedList<TDto> paginatedBaseDtos)
        where TDto : SingleResourceBaseDto
    {
        var dtosWithHateoas = new List<SingleResourceBaseDto>();
        foreach (SingleResourceBaseDto paginatedBaseDto in paginatedBaseDtos)
        {
            SingleResourceBaseDto dtoWithHateoas = AddHateoasLinks(allowedRestMethodDtos, paginatedBaseDto);
            dtosWithHateoas.Add(dtoWithHateoas);
        }
        return new PaginatedList<SingleResourceBaseDto>(
            dtosWithHateoas,
            paginatedBaseDtos.TotalAmountOfItems,
            paginatedBaseDtos.TotalAmountOfPages,
            paginatedBaseDtos.PageIndex,
            paginatedBaseDtos.PageSize);
    }

    private void AddErrorsToModelState(string errorTypes, string errorMessages)
    {
        string[] splitErrorTypes = errorTypes.Split('|');
        string[] splitErrorMessages = errorMessages.Split('|');
        for (int i = 0; i < splitErrorMessages.Length; i++)
        {
            ModelState.AddModelError(splitErrorTypes[i], splitErrorMessages[i]);
        }
    }
}
