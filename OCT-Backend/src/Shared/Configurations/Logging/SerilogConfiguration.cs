using Serilog;

namespace Configurations.Logging;

public class SerilogConfiguration
{
    public string LogsPath { get; set; }

    public string LogFileName { get; set; }

    public int FileSizeLimitBytes { get; set; }

    public int RetainedFileCountLimit { get; set; }

    public bool RollOnFileSizeLimit { get; set; }

    public bool Shared { get; set; }

    public RollingInterval RollingInterval { get; set; }
}
