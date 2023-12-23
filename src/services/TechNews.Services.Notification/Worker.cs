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
        Log.Information("Worker has started");
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
            HtmlBody = GetHtmlBody(userRegisteredDetails)
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        client.Connect(EnvironmentVariables.SmtpHost, EnvironmentVariables.SmtpPort, false);
        client.AuthenticationMechanisms.Remove("XOAUTH2");
        client.Authenticate(EnvironmentVariables.SmtpEmail, EnvironmentVariables.SmtpPassword);

        client.Send(message);
        client.Disconnect(true);
    }
    
    private static string GetHtmlBody(UserRegisteredEvent userRegisteredDetails)
    {
        const string hostAndPort = "https://localhost:7283";
        const string logoUrl = "https://tech04.blob.core.windows.net/logo/TechNews.png";
        
        return $@"
            <!doctypehtml><meta charset=utf-8><meta content=""ie=edge""http-equiv=x-ua-compatible><title>TechNews - Confirmação de Email</title><meta content=""width=device-width,initial-scale=1""name=viewport><style>@media screen{{@font-face{{font-family:'Source Sans Pro';font-style:normal;font-weight:400;src:local('Source Sans Pro Regular'),local('SourceSansPro-Regular'),url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff')}}@font-face{{font-family:'Source Sans Pro';font-style:normal;font-weight:700;src:local('Source Sans Pro Bold'),local('SourceSansPro-Bold'),url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff')}}a,body,table,td{{-ms-text-size-adjust:100%;-webkit-text-size-adjust:100%}}table,td{{mso-table-rspace:0;mso-table-lspace:0}}img{{-ms-interpolation-mode:bicubic}}a[x-apple-data-detectors]{{font-family:inherit!important;font-size:inherit!important;font-weight:inherit!important;line-height:inherit!important;color:inherit!important;text-decoration:none!important}}div[style*=""margin: 16px 0;""]{{margin:0!important}}body{{width:100%!important;height:100%!important;padding:0!important;margin:0!important}}table{{border-collapse:collapse!important}}a{{color:#1a82e2}}img{{height:auto;line-height:100%;text-decoration:none;border:0;outline:0}}</style><body style=background-color:#e9ecef><div class=preheader style=display:none;max-width:0;max-height:0;overflow:hidden;font-size:1px;line-height:1px;color:#fff;opacity:0>TechNews - Confirme seu e-mail.</div><table border=0 cellpadding=0 cellspacing=0 width=100%><tr><td align=center bgcolor=#e9ecef><table border=0 cellpadding=0 cellspacing=0 width=100% style=max-width:600px><tr><td align=center style=""padding:36px 24px""valign=top><a href=""https:{hostAndPort}"" target=_blank style=display:inline-block><img alt=Logo src=""{logoUrl}"" style=display:block;width:600px;max-width:600px;min-width:600px></a></table><tr><td align=center bgcolor=#e9ecef><table border=0 cellpadding=0 cellspacing=0 width=100% style=max-width:600px><tr><td align=left bgcolor=#ffffff style=""padding:36px 24px 0;font-family:'Source Sans Pro',Helvetica,Arial,sans-serif;border-top:3px solid #d4dadf""><h1 style=margin:0;font-size:32px;font-weight:700;letter-spacing:-1px;line-height:48px>Olá, {userRegisteredDetails.UserName}!</h1></table><tr><td align=center bgcolor=#e9ecef><table border=0 cellpadding=0 cellspacing=0 width=100% style=max-width:600px><tr><td align=left bgcolor=#ffffff style=""padding:24px;font-family:'Source Sans Pro',Helvetica,Arial,sans-serif;font-size:16px;line-height:24px""><p style=margin:0>Clique no link abaixo para confirmar o seu e-mail. Se você não criou uma conta no <a href=""https://{hostAndPort}"">TechNews</a>, pode desconsiderar este e-mail em segurança.<tr><td align=left bgcolor=#ffffff><table border=0 cellpadding=0 cellspacing=0 width=100%><tr><td align=center bgcolor=#ffffff style=padding:12px><table border=0 cellpadding=0 cellspacing=0><tr><td align=center bgcolor=#1a82e2 style=border-radius:6px><a href=""https://{hostAndPort}/email-confirmation?email={userRegisteredDetails.Email}&token={userRegisteredDetails.ValidateEmailToken}""target=_blank style=""display:inline-block;padding:16px 36px;font-family:'Source Sans Pro',Helvetica,Arial,sans-serif;font-size:16px;color:#fff;text-decoration:none;border-radius:6px"">Confirmar e-mail</a></table></table><tr><td align=left bgcolor=#ffffff style=""padding:24px;font-family:'Source Sans Pro',Helvetica,Arial,sans-serif;font-size:16px;line-height:24px""><p style=margin:0>Se o botão acima não funcionar, pode copiar o link abaixo e colar na barra de busca do seu navegador:<p style=margin:0><a href=""https://{hostAndPort}/email-confirmation?email={userRegisteredDetails.Email}&token={userRegisteredDetails.ValidateEmailToken}""target=_blank>https://{hostAndPort}/email-confirmation?email={userRegisteredDetails.Email}&token={userRegisteredDetails.ValidateEmailToken}</a><tr><td align=left bgcolor=#ffffff style=""padding:24px;font-family:'Source Sans Pro',Helvetica,Arial,sans-serif;font-size:16px;line-height:24px;border-bottom:3px solid #d4dadf""><p style=margin:0>Até breve,<br><b>TechNews</b></table><tr><td align=center bgcolor=#e9ecef style=padding:24px><table border=0 cellpadding=0 cellspacing=0 width=100% style=max-width:600px><tr><td align=center bgcolor=#e9ecef style=""padding:12px 24px;font-family:'Source Sans Pro',Helvetica,Arial,sans-serif;font-size:14px;line-height:20px;color:#666""><p style=margin:0>Você recebeu este e-mail por solicitado o cadastro no site TechNews. Se você não solicitou esse cadastro, pode desconsiderar este e-mail em segurança.</table></table>
        ";
    }
}
