using Configurations.Logging;
using Ocelot.DependencyInjection;
using Serilog;

namespace Gateway.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static void ConfigureCORS(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("allAllowed", builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()                 
                    .AllowCredentials();
            });
        });
    }

    public static void ConfigureOcelot(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("ocelot.json", false, true)
            .AddEnvironmentVariables();

        builder.Services
            .AddOcelot(builder.Configuration);
            //.AddConsul()
            //.AddConfigStoredInConsul();
    }

    public static void ConfigureLogger(this WebApplicationBuilder builder)
    {
        var serilogConfiguration = new SerilogConfiguration();

        builder.Configuration.GetSection(nameof(SerilogConfiguration)).Bind(serilogConfiguration);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
               Path.Combine("/", serilogConfiguration.LogsPath, serilogConfiguration.LogFileName),
               rollingInterval: serilogConfiguration.RollingInterval,
               fileSizeLimitBytes: serilogConfiguration.FileSizeLimitBytes,
               retainedFileCountLimit: serilogConfiguration.RetainedFileCountLimit,
               rollOnFileSizeLimit: serilogConfiguration.RollOnFileSizeLimit,
               shared: serilogConfiguration.Shared)
            .CreateLogger();

        builder.Services.AddSingleton(serilogConfiguration);
    }
}
