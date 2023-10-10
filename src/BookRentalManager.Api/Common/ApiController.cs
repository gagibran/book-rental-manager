using System.Dynamic;
using System.Reflection;
using System.Text.Json;

namespace BookRentalManager.Api.Common;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ApiController : ControllerBase
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

    protected void CreatePaginationMetadata<TItem>(string routeName, PaginatedList<TItem> paginatedList)
    {
        int totalAmountOfPages = paginatedList.TotalAmountOfPages == int.MinValue ? 0 : paginatedList.TotalAmountOfPages;
        string serializedMetadata = JsonSerializer.Serialize(
            new
            {
                paginatedList.TotalAmountOfItems,
                paginatedList.PageIndex,
                paginatedList.PageSize,
                TotalAmountOfPages = totalAmountOfPages,
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

    protected ExpandoObject AddHateoasLinks(List<AllowedRestMethodsDto> allowedRestMethodDtos, IdentifiableDto identifiableDto)
    {
        if (!allowedRestMethodDtos.Any())
        {
            throw new ArgumentException($"{allowedRestMethodDtos} cannot be empty.");
        }
        var hateoasLinkDtos = new List<HateoasLinkDto>();
        foreach (AllowedRestMethodsDto allowedRestMethodDto in allowedRestMethodDtos)
        {
            string? href = Url.Link(allowedRestMethodDto.Method, new { Id = identifiableDto.Id });
            var hateoasLinkDto = new HateoasLinkDto(href!, allowedRestMethodDto.Rel, allowedRestMethodDto.MethodName);
            hateoasLinkDtos.Add(hateoasLinkDto);
        }
        IDictionary<string, object?> expandoObject = new ExpandoObject();
        foreach (PropertyInfo property in identifiableDto.GetType().GetProperties())
        {
            var propertyFirstLetterLowercase = char.ToLower(property.Name[0]).ToString() + property.Name[1..];
            expandoObject.Add(propertyFirstLetterLowercase, property.GetValue(identifiableDto));
        }
        expandoObject["links"] = hateoasLinkDtos;
        return (ExpandoObject)expandoObject;
    }

    protected CollectionWithHateoasLinksDto AddHateoasLinksToPaginatedCollection<TDto>(
        string routeName,
        GetAllItemsQueryParameters queryParameters,
        List<AllowedRestMethodsDto> allowedRestMethodDtos,
        PaginatedList<TDto> paginatedBaseDtos)
        where TDto : IdentifiableDto
    {
        var dtosWithHateoasLinks = new List<ExpandoObject>();
        foreach (IdentifiableDto paginatedBaseDto in paginatedBaseDtos)
        {
            ExpandoObject dtoWithHateoasLinks = AddHateoasLinks(allowedRestMethodDtos, paginatedBaseDto);
            dtosWithHateoasLinks.Add(dtoWithHateoasLinks);
        }
        var values =  new PaginatedList<ExpandoObject>(
            dtosWithHateoasLinks,
            paginatedBaseDtos.TotalAmountOfItems,
            paginatedBaseDtos.TotalAmountOfPages,
            paginatedBaseDtos.PageIndex,
            paginatedBaseDtos.PageSize);
        List<HateoasLinkDto> links = CreatePreviousAndNextPagesLinks(routeName, queryParameters, values);
        return new CollectionWithHateoasLinksDto(values, links);
    }

    protected bool IsMediaTypeVendorSpecific(string? mediaType)
    {
        return MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? mediaTypeHeaderValue)
            && mediaTypeHeaderValue.MediaType.Equals(MediaTypeConstants.BookRentalManagerHateoasMediaType);
    }

    private List<HateoasLinkDto> CreatePreviousAndNextPagesLinks(
        string routeName,
        GetAllItemsQueryParameters queryParameters,
        PaginatedList<ExpandoObject> paginatedItems)
    {
        var hateoasLinkDtos = new List<HateoasLinkDto>();
        if (paginatedItems.HasPreviousPage)
        {
            string? previousPageLink = Url.Link(routeName, new
            {
                PageIndex = paginatedItems.PageIndex - 1,
                paginatedItems.PageSize,
                queryParameters.SearchQuery,
                queryParameters.SortBy
            });
            hateoasLinkDtos.Add(new HateoasLinkDto(previousPageLink!, "previous_page", "GET"));
        }
        if (paginatedItems.HasNextPage)
        {
            string? nextPageLink = Url.Link(routeName, new
            {
                PageIndex = paginatedItems.PageIndex + 1,
                paginatedItems.PageSize,
                queryParameters.SearchQuery,
                queryParameters.SortBy
            });
            hateoasLinkDtos.Add(new HateoasLinkDto(nextPageLink!, "next_page", "GET"));
        }
        return hateoasLinkDtos;
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
