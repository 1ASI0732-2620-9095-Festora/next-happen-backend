using com.festora.nexthappen.notification.Infrastructure.External;
using com.festora.nexthappen.notification.Infrastructure.Verification;

namespace com.festora.nexthappen.notification.Application.Services;

public class VerificationCodeService
{
    private readonly VerificationCodeStore _store;
    private readonly TwilioSmsClient _twilioClient;
    private readonly int _expirationMinutes;

    public VerificationCodeService(VerificationCodeStore store, TwilioSmsClient twilioClient, IConfiguration configuration)
    {
        _store = store;
        _twilioClient = twilioClient;
        _expirationMinutes = configuration.GetValue<int>("Notifications:Verification:ExpirationMinutes", 5);
    }

    public (bool Success, string? ErrorMessage) SendVerificationCode(string phone)
    {
        var code = GenerateCode();
        
        var issueResult = _store.Issue(phone, code);
        if (!issueResult.Success)
        {
            return issueResult;
        }

        var smsResult = _twilioClient.SendVerificationSms(phone, code, _expirationMinutes);
        if (!smsResult.Success)
        {
            return smsResult;
        }

        return (true, null);
    }

    public (bool Success, string? ErrorMessage) VerifyCode(string phone, string code)
    {
        return _store.Verify(phone, code);
    }

    private string GenerateCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}
