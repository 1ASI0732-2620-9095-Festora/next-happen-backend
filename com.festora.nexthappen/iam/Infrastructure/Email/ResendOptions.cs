namespace com.festora.nexthappen.iam.Infrastructure.Email;

public class ResendOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "NextHappen <onboarding@resend.dev>";
}
