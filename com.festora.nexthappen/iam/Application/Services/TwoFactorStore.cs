using System.Collections.Concurrent;

namespace com.festora.nexthappen.iam.Application.Services;

public class TwoFactorStore
{
    private class OtpRecord
    {
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public int Attempts { get; set; }
    }

    private readonly ConcurrentDictionary<string, OtpRecord> _store = new(StringComparer.OrdinalIgnoreCase);

    public void SaveCode(string email, string code, TimeSpan validity)
    {
        CleanupExpired();
        _store[email] = new OtpRecord
        {
            Code = code,
            ExpiresAt = DateTime.UtcNow.Add(validity),
            Attempts = 0
        };
    }

    public (bool Valid, string? ErrorMessage) VerifyCode(string email, string code)
    {
        CleanupExpired();

        if (!_store.TryGetValue(email, out var record))
        {
            return (false, "El código ha expirado o no ha sido solicitado.");
        }

        if (DateTime.UtcNow > record.ExpiresAt)
        {
            _store.TryRemove(email, out _);
            return (false, "El código ha expirado.");
        }

        record.Attempts++;
        if (record.Attempts > 4)
        {
            _store.TryRemove(email, out _);
            return (false, "Demasiados intentos fallidos. Solicita un nuevo código.");
        }

        if (!string.Equals(record.Code.Trim(), code.Trim(), StringComparison.Ordinal))
        {
            return (false, "El código ingresado es incorrecto.");
        }

        // Éxito: eliminar código para evitar reutilización
        _store.TryRemove(email, out _);
        return (true, null);
    }

    private void CleanupExpired()
    {
        var now = DateTime.UtcNow;
        foreach (var (key, value) in _store)
        {
            if (now > value.ExpiresAt)
            {
                _store.TryRemove(key, out _);
            }
        }
    }
}
