using System.Text;
using dotenv.net;

namespace TechNews.Services.Notification.Configurations;

public static class EnvironmentVariables
{
    public static string BrokerConfirmEmailQueueName { get; private set; } = string.Empty;
    public static string BrokerHostName { get; private set; } = string.Empty;
    public static string BrokerVirtualHost { get; private set; } = string.Empty;
    public static string BrokerUserName { get; private set; } = string.Empty;
    public static string BrokerPassword { get; private set; } = string.Empty;
    public static string? DiscordWebhookId { get; private set; }
    public static string? DiscordWebhookToken { get; private set; }

    public static IServiceCollection AddEnvironmentVariables(this IServiceCollection services, IHostEnvironment environment)
    {
        try
        {
            DotEnv.Fluent()
                .WithExceptions()
                .WithEnvFiles()
                .WithTrimValues()
                .WithEncoding(Encoding.UTF8)
                .WithOverwriteExistingVars()
                .WithProbeForEnv(probeLevelsToSearch: 6)
                .Load();
        }
        catch (Exception)
        {
            if (environment.IsEnvironment("Local"))
            {
                throw new ApplicationException("Environment File (.env) not found. The application needs a .env file to run locally.\nPlease check the section Environment Variables of the README.");
            }

            // Ignored if other environments because it is using runtime environment variables
        }

        LoadVariables();

        return services;
    }

    private static void LoadVariables()
    {
        BrokerConfirmEmailQueueName = Environment.GetEnvironmentVariable("TECHNEWS_SERVICES_NOTIFICATION_BROKER_CONFIRM_EMAIL_QUEUE_NAME") ?? string.Empty;

        BrokerHostName = Environment.GetEnvironmentVariable("TECHNEWS_AUTH_API_BROKER_HOST_NAME") ?? string.Empty;
        BrokerVirtualHost = Environment.GetEnvironmentVariable("TECHNEWS_SERVICES_NOTIFICATION_BROKER_VIRTUAL_HOST") ?? string.Empty;
        BrokerUserName = Environment.GetEnvironmentVariable("TECHNEWS_SERVICES_NOTIFICATION_BROKER_USER_NAME") ?? string.Empty;
        BrokerPassword = Environment.GetEnvironmentVariable("TECHNEWS_SERVICES_NOTIFICATION_BROKER_PASSWORD") ?? string.Empty;

        DiscordWebhookId = Environment.GetEnvironmentVariable("TECHNEWS_SERVICES_NOTIFICATION_DISCORD_WEBHOOK_ID");
        DiscordWebhookToken = Environment.GetEnvironmentVariable("TECHNEWS_SERVICES_NOTIFICATION_DISCORD_WEBHOOK_TOKEN");
    }
}