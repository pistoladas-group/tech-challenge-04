using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;

namespace TechNews.Services.Notification.Configurations;

public static class Logging
{
    public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services)
    {
        var webhookId = Convert.ToUInt64(EnvironmentVariables.DiscordWebhookId);
        var webhookToken = EnvironmentVariables.DiscordWebhookToken;
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)
            .WriteTo.Discord(webhookId: webhookId, webhookToken: webhookToken, restrictedToMinimumLevel: LogEventLevel.Warning)
            .CreateLogger();
        
        return services;
    }
}