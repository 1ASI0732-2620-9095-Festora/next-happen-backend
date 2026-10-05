using System.Collections.Concurrent;

namespace com.festora.nexthappen.notification.Infrastructure.Verification;

public class VerificationCodeStore
{
    private record Entry(string Code, DateTime IssuedAt, DateTime ExpiresAt, int Attempts)
    {
        public Entry WithAttemptRegistered() => this with { Attempts = Attempts + 1 };
    }

    private readonly ConcurrentDictionary<string, Entry> _entriesByPhone = new();

    private readonly TimeSpan _expiration;
    private readonly TimeSpan _resendCooldown;
    private readonly int _maxAttempts;

    public VerificationCodeStore(IConfiguration configuration)
    {
        var expirationMinutes = configuration.GetValue<int>("Notifications:Verification:ExpirationMinutes", 5);
        var resendCooldownSeconds = configuration.GetValue<int>("Notifications:Verification:ResendCooldownSeconds", 60);
        _maxAttempts = configuration.GetValue<int>("Notifications:Verification:MaxAttempts", 5);

        _expiration = TimeSpan.FromMinutes(expirationMinutes);
        _resendCooldown = TimeSpan.FromSeconds(resendCooldownSeconds);
    }

    public (bool Success, string? ErrorMessage) Issue(string phone, string code)
    {
        var now = DateTime.UtcNow;

        if (_entriesByPhone.TryGetValue(phone, out var existing))
        {
            var cooldownEnd = existing.IssuedAt.Add(_resendCooldown);
            if (cooldownEnd > now)
            {
                var secondsLeft = Math.Max(1, (int)(cooldownEnd - now).TotalSeconds);
                return (false, $"Please wait {secondsLeft} more second(s) before requesting a new code");
            }
        }

        var newEntry = new Entry(code, now, now.Add(_expiration), 0);
        _entriesByPhone[phone] = newEntry; 

        return (true, null);
    }

    public (bool Success, string? ErrorMessage) Verify(string phone, string code)
    {
        if (!_entriesByPhone.TryGetValue(phone, out var entry))
        {
            return (false, "No verification code was requested for this phone number, or it was already used");
        }

        if (DateTime.UtcNow > entry.ExpiresAt)
        {
            _entriesByPhone.TryRemove(phone, out _);
            return (false, "Verification code has expired");
        }

        if (entry.Attempts >= _maxAttempts)
        {
            _entriesByPhone.TryRemove(phone, out _);
            return (false, "Too many failed attempts; request a new code");
        }

        if (entry.Code != code)
        {
            _entriesByPhone[phone] = entry.WithAttemptRegistered();
            return (false, "Verification code is incorrect");
        }

        _entriesByPhone.TryRemove(phone, out _);
        return (true, null);
    }
}
