namespace com.festora.nexthappen.iam.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // "User" o "Organizer"
    public string? AvatarUrl { get; set; }
    public DateTime? TermsAcceptedAt { get; set; }
    public string? TermsVersion { get; set; }
}
