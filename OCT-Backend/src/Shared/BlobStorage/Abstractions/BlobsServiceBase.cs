using Exceptions.UserService;

namespace BlobStorage.Abstractions;

public abstract class BlobsServiceBase<T>
{
    public abstract Task<Stream> GetBlobAsync(string container, Guid id);

    public abstract Task DeleteBlobAsync(string container, Guid id);

    public async Task<string> UploadBlobAsync(string container, T image, Guid id)
    {
        try
        {
            ValidateBlob(image);

            return await UploadBlobCoreAsync(container, image, id);
        }
        catch (ArgumentException exception)
        {
            throw new BlobOperationException(exception.Message);
        }
    }

    protected abstract Task<string> UploadBlobCoreAsync(string container, T image, Guid id);

    protected abstract void ValidateBlob(T image);
}
