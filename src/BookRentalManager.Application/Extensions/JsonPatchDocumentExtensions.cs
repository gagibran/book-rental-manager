using Microsoft.AspNetCore.JsonPatch.Operations;

namespace BookRentalManager.Application.Extensions;

public static class JsonPatchDocumentExtensions
{
    public static Result ApplyTo<T>(this JsonPatchDocument<T> jsonPatchDocument, T itemToApplyTo) where T : class
    {
        foreach (Operation<T> operation in jsonPatchDocument.Operations)
        {
            if (string.IsNullOrWhiteSpace(operation.op))
            {
                return Result.Fail("jsonPatch", "'Operation' cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(operation.path))
            {
                return Result.Fail("jsonPatch", "'Path' cannot be empty.");
            }
        }
        Result patchAppliedResult = Result.Success();
        jsonPatchDocument.ApplyTo(itemToApplyTo, jsonPatchError =>
        {
            patchAppliedResult = Result.Fail("jsonPatch", jsonPatchError.ErrorMessage);
        });
        return patchAppliedResult;
    }
}
