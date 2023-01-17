using Azure.Storage.Blobs;
using Azure.Storage;
using Configurations.BlobStorage;

namespace User.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task IntializeAzureStorage(this WebApplication app)
    {
        await app.UserPhotoCotainerEnsureCreated();
    }

    private static async Task UserPhotoCotainerEnsureCreated(this WebApplication app)
    {
        var azureConfiguration = app.Services.GetRequiredService<AzureStorageConfiguration>();

        var containerAddress = $"{azureConfiguration.Address}/{azureConfiguration.Account}/{BlobContainersNames.UsersProfilePictures}";

        var client = new BlobContainerClient(
            new Uri(containerAddress),
            new StorageSharedKeyCredential(azureConfiguration.Account, azureConfiguration.AccountKey));

        await client.CreateIfNotExistsAsync();

        await client.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
    }
}
