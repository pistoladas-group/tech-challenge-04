namespace TechNews.Common.Library.Messages.Events;

public class UserRegisteredEvent
{
    public Guid UserId { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? UserName { get; init; }
    public string Email { get; init; }
    public string Token { get; init; }
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
        string token,
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
        Token = token;
        EmailConfirmed = emailConfirmed;
        LockoutEnabled = lockoutEnabled;
        LockoutEnd = lockoutEnd;
        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = phoneNumberConfirmed;
        TwoFactorEnabled = twoFactorEnabled;
    }
}