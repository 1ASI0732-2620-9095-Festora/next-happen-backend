namespace com.festora.nexthappen.iam.Infrastructure.Email;

public interface IEmailService
{
    Task<(bool Success, string? ErrorMessage)> SendTwoFactorCodeAsync(string toEmail, string code, string? recipientName = null);
}
