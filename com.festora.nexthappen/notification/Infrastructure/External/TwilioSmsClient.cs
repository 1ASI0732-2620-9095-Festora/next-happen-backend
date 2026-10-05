using Twilio.Rest.Api.V2010.Account;
using Twilio.Exceptions;

namespace com.festora.nexthappen.notification.Infrastructure.External;

public class TwilioSmsClient
{
    private readonly ILogger<TwilioSmsClient> _logger;
    private readonly string _fromPhoneNumber;

    public TwilioSmsClient(IConfiguration configuration, ILogger<TwilioSmsClient> logger)
    {
        _logger = logger;
        
        var accountSid = configuration["Twilio:AccountSid"];
        var authToken = configuration["Twilio:AuthToken"];
        _fromPhoneNumber = configuration["Twilio:PhoneNumber"] ?? string.Empty;

        if (!string.IsNullOrEmpty(accountSid) && !string.IsNullOrEmpty(authToken))
        {
            Twilio.TwilioClient.Init(accountSid, authToken);
        }
    }

    public (bool Success, string? ErrorMessage) SendWelcomeSms(string to, string fullName)
    {
        try
        {
            MessageResource.Create(
                body: $"Hi {fullName}, welcome to Festora! Your account is ready to use.",
                from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(to)
            );
            return (true, null);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Twilio SMS delivery failed for {To}", to);
            return (false, ex.Message ?? "Unknown Twilio error");
        }
    }

    public (bool Success, string? ErrorMessage) SendVerificationSms(string to, string code, int expirationMinutes)
    {
        try
        {
            // Forzamos el envío al número verificado por estar en una cuenta de prueba de Twilio.
            var testDestination = "+51902839089"; 
            
            MessageResource.Create(
                body: $"Your Festora verification code is {code}. It expires in {expirationMinutes} minute(s).",
                from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(testDestination)
            );
            return (true, null);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Twilio verification SMS delivery failed for {To}", to);
            return (false, ex.Message ?? "Unknown Twilio error");
        }
    }
}
