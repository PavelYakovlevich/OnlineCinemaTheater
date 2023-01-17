using Azure.Storage;
using Azure.Storage.Blobs;
using BlobStorage.Abstractions;
using Configurations.BlobStorage;
using Exceptions.UserService;
using Microsoft.AspNetCore.Http;

namespace BlobStorage.Implementations;

public class AzureBlobsService : BlobsServiceBase<IFormFile>
{
    private readonly BlobServiceConfiguration _configuration;
    private readonly AzureStorageConfiguration _azureConfiguration;
    private readonly string _containerAddress;

    public AzureBlobsService(BlobServiceConfiguration configuration, AzureStorageConfiguration azureConfiguration)
    {
        _configuration = configuration;
        _azureConfiguration = azureConfiguration;
        _containerAddress = $"{_azureConfiguration.Address}/{_azureConfiguration.Account}";
    }

    public override async Task DeleteBlobAsync(string container, Guid id)
    {
        var client = GetContainer(container);

        var blobName = id.ToString();

        if (!await client.DeleteBlobIfExistsAsync(blobName))
        {
            throw new BlobOperationException($"File with id {id} doesn't exist");
        }
    }

    public override async Task<Stream> GetBlobAsync(string container, Guid id)
    {
        var client = GetContainer(container);

        var targetBlobName = id.ToString();
        var targetBlob = client.GetBlobClient(targetBlobName);
        
        if (!await targetBlob.ExistsAsync())
        {
            return null;
        }

        return await targetBlob.OpenReadAsync();
    }

    protected override async Task<string> UploadBlobCoreAsync(string container, IFormFile image, Guid id)
    {
        var client = GetContainer(container);

        var blodName = id.ToString();

        await client.DeleteBlobIfExistsAsync(blodName);

        await client.UploadBlobAsync(blodName, image.OpenReadStream());

        return $"{_containerAddress}/{container}/{blodName}";
    }

    protected override void ValidateBlob(IFormFile image)
    {
        if (image.Length > _configuration.MaxFileSize)
        {
            throw new ArgumentException($"File size must be less than {_configuration.MaxFileSize}");
        }

        var fileExtension = Path.GetExtension(image.FileName)?.ToLower();
        if (!_configuration.AllowedExtensions.Any(e => e == fileExtension))
        {
            throw new ArgumentException($"Extension {fileExtension} is not supported");
        }
    }

    private BlobContainerClient GetContainer(string container) =>
        new(new Uri($"{_containerAddress}/{container}"), 
            new StorageSharedKeyCredential(_azureConfiguration.Account, _azureConfiguration.AccountKey));
}
