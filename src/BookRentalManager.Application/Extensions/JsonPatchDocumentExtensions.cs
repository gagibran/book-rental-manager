using Microsoft.AspNetCore.JsonPatch.Operations;

namespace BookRentalManager.Application.Extensions;

public static class JsonPatchDocumentExtensions
{
    public static Result ApplyTo<T>(this JsonPatchDocument<T> jsonPatchDocument, T itemToApplyTo, params string[] operationsToExclude) where T : class
    {
        foreach (Operation<T> operation in jsonPatchDocument.Operations)
        {
            if (string.IsNullOrWhiteSpace(operation.op))
            {
                return Result.Fail("jsonPatch", "'operation' cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(operation.path))
            {
                return Result.Fail("jsonPatch", "'path' cannot be empty.");
            }
            if (operation.value is null)
            {
                return Result.Fail("jsonPatch", "'value' cannot be empty.");
            }
            if (operationsToExclude is not null && operationsToExclude.Contains(operation.op))
            {
                return Result.Fail("jsonPatch", $"'{operation.op}' operation not allowed in this context.");
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
