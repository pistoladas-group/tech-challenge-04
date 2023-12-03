using System.Text.Json;

namespace TechNews.Common.Library.Models;

public class ConfirmEmailBrokerMessage
{
    public string email { get; init; }
    public string token { get; init; }
    
    //for MassTransit
    public ConfirmEmailBrokerMessage()
    {
        this.email = string.Empty;
        this.token = string.Empty;
    }
    
    public ConfirmEmailBrokerMessage(string email, string token)
    {
        this.email = email;
        this.token = token;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
