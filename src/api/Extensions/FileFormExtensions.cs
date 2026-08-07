using Server.Storage;

namespace Api.Extensions;

public static class FileFormExtensions
{
    public static UploadedFile? ToUploadedFile(this IFormFile? formFile)
    {
        if (formFile is null)
        {
            return null;
        }

        var stream = formFile.OpenReadStream();
        return new UploadedFile(formFile.FileName, stream);
    }
}