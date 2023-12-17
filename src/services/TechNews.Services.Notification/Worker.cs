using MailKit.Net.Smtp;
using MimeKit;
using Serilog;
using TechNews.Common.Library.MessageBus;
using TechNews.Common.Library.Messages.Events;
using TechNews.Services.Notification.Configurations;

namespace TechNews.Services.Notification;

public class Worker : BackgroundService
{
    private readonly IMessageBus _bus;

    public Worker(IMessageBus bus)
    {
        _bus = bus;
    }

    //TODO: configurar as filas sem precisar do método consume
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _bus.Consume<UserRegisteredEvent>(EnvironmentVariables.BrokerConfirmEmailQueueName, ExecuteAfterConsumed);
    }

    public void ExecuteAfterConsumed(UserRegisteredEvent? message)
    {
        Log.Information("Message receveived: @{message}", message);

        if (message is not null)
            SendEmail(message);
    }

    private void SendEmail(UserRegisteredEvent userRegisteredDetails)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("TechNews", EnvironmentVariables.SmtpEmail));
        message.To.Add(new MailboxAddress(userRegisteredDetails.UserName, userRegisteredDetails.Email));
        message.Subject = "TechNews - Email confirmation";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"<b>Olá {userRegisteredDetails.UserName}</b>,

<p>Clique no link abaixo para concluir seu cadastro em nosso site e ter acesso às notícias mais quentinhas da web!</p>

<p><a href=""https://localhost:7283/email-confirmation/{userRegisteredDetails.ValidateEmailToken}"">Confirmar e-mail</a></p>

Até logo!"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        client.Connect(EnvironmentVariables.SmtpHost, EnvironmentVariables.SmtpPort, false);
        client.AuthenticationMechanisms.Remove("XOAUTH2");
        client.Authenticate(EnvironmentVariables.SmtpEmail, EnvironmentVariables.SmtpPassword);

        client.Send(message);
        client.Disconnect(true);
    }
}
