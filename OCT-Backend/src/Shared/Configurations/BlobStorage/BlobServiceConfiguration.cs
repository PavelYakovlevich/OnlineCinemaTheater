namespace Configurations.BlobStorage;

public class BlobServiceConfiguration
{
    public int MaxFileSize { get; set; }

    public string[] AllowedExtensions { get; set; }
}
