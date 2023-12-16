using System.Text.Json;
using EventStore.Client;

namespace TechNews.Common.Library.Messages.Events;

public class UserRegisteredEvent : IEvent
{
    public Guid UserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserName { get; set; }
    public string Email { get; set; }
    public string ValidateEmailToken { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }

    public UserRegisteredEvent
    (
        Guid userId,
        bool isDeleted,
        DateTime createdAt,
        string? userName,
        string email,
        string validateEmailToken,
        bool emailConfirmed,
        bool lockoutEnabled,
        DateTimeOffset? lockoutEnd,
        string? phoneNumber,
        bool phoneNumberConfirmed,
        bool twoFactorEnabled
    )
    {
        UserId = userId;
        IsDeleted = isDeleted;
        CreatedAt = createdAt;
        UserName = userName;
        Email = email;
        ValidateEmailToken = validateEmailToken;
        EmailConfirmed = emailConfirmed;
        LockoutEnabled = lockoutEnabled;
        LockoutEnd = lockoutEnd;
        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = phoneNumberConfirmed;
        TwoFactorEnabled = twoFactorEnabled;
    }

    public string GetStreamName()
    {
        return $"User-{UserId}";
    }

    public EventData[] GetEventData()
    {
        var utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(this);
        var eventData = new EventData(Uuid.NewUuid(), nameof(UserRegisteredEvent), utf8Bytes.AsMemory());

        return new[] { eventData };
    }
}