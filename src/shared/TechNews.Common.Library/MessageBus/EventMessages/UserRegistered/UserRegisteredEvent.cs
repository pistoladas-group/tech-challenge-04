using TechNews.Common.Library.MessageBus.EventMessages.Base;

namespace TechNews.Common.Library.MessageBus.EventMessages.UserRegistered;

public class UserRegisteredEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
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
        string token,
        bool emailConfirmed,
        bool lockoutEnabled,
        DateTimeOffset? lockoutEnd,
        string? phoneNumber,
        bool phoneNumberConfirmed,
        bool twoFactorEnabled
    ) : base(aggregateId: userId, aggregateName: "User", eventName: nameof(UserRegisteredEvent))
    {
        this.UserId = userId;
        this.IsDeleted = isDeleted;
        this.CreatedAt = createdAt;
        this.UserName = userName;
        this.Email = email;
        this.Token = token;
        this.EmailConfirmed = emailConfirmed;
        this.LockoutEnabled = lockoutEnabled;
        this.LockoutEnd = lockoutEnd;
        this.PhoneNumber = phoneNumber;
        this.PhoneNumberConfirmed = phoneNumberConfirmed;
        this.TwoFactorEnabled = twoFactorEnabled;
    }
}
