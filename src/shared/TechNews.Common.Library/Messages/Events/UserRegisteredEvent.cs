using System.Text.Json;
using EventStore.Client;

namespace TechNews.Common.Library.Messages.Events;

public class UserRegisteredEvent : IEvent
{
    public Guid UserId { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? UserName { get; init; }
    public string Email { get; init; }
    public string EmailBase64 { get; init; }
    public string ValidateEmailToken { get; init; }
    public string ValidateEmailTokenBase64 { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool LockoutEnabled { get; init; }
    public DateTimeOffset? LockoutEnd { get; init; }
    public string? PhoneNumber { get; init; }
    public bool PhoneNumberConfirmed { get; init; }
    public bool TwoFactorEnabled { get; init; }

    public UserRegisteredEvent
    (
        Guid userId,
        bool isDeleted,
        DateTime createdAt,
        string? userName,
        string email,
        string emailBase64,
        string validateEmailToken,
        string validateEmailTokenBase64,
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
        EmailBase64 = emailBase64;
        ValidateEmailToken = validateEmailToken;
        ValidateEmailTokenBase64 = validateEmailTokenBase64;
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

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}