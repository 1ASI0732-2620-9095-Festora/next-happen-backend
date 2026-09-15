using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace com.festora.nexthappen.iam.Infrastructure.Email;

public class ResendEmailService : IEmailService
{
    private readonly HttpClient _http;
    private readonly ResendOptions _options;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(HttpClient http, IOptions<ResendOptions> options, ILogger<ResendEmailService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<(bool Success, string? ErrorMessage)> SendTwoFactorCodeAsync(string toEmail, string code, string? recipientName = null)
    {
        var apiKey = _options.ApiKey?.Trim();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("[Resend] No hay ApiKey configurada para Resend. Código para {Email}: {Code}", toEmail, code);
            return (false, "Servicio de correo no configurado.");
        }

        var greetingName = string.IsNullOrWhiteSpace(recipientName) ? "Usuario" : recipientName.Trim();

        var htmlContent = $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Código de Verificación NextHappen</title>
</head>
<body style=""margin:0;padding:0;background-color:#f9fafb;font-family:'Segoe UI',Roboto,Helvetica,Arial,sans-serif;color:#111827;"">
    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""min-height:100vh;padding:32px 16px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width:480px;background:#ffffff;border-radius:16px;border:1px solid #e5e7eb;box-shadow:0 4px 6px -1px rgba(0,0,0,0.05);overflow:hidden;"">
                    <!-- Header -->
                    <tr>
                        <td style=""padding:32px 32px 16px;text-align:center;border-bottom:1px solid #f3f4f6;"">
                            <span style=""display:inline-block;font-size:24px;font-weight:800;letter-spacing:-0.5px;color:#000000;"">
                                Next<span style=""color:#e11d48;"">Happen</span>
                            </span>
                        </td>
                    </tr>
                    <!-- Body -->
                    <tr>
                        <td style=""padding:32px;"">
                            <h1 style=""font-size:20px;font-weight:700;margin:0 0 12px;color:#111827;"">Tu código de verificación</h1>
                            <p style=""font-size:15px;line-height:24px;color:#4b5563;margin:0 0 24px;"">
                                Hola <strong>{greetingName}</strong>, utiliza el siguiente código de 6 dígitos para completar tu inicio de sesión seguro en NextHappen:
                            </p>
                            <!-- OTP Box -->
                            <div style=""background-color:#fff1f2;border:2px dashed #fda4af;border-radius:12px;padding:20px;text-align:center;margin:0 0 24px;"">
                                <span style=""font-family:Consolas,Monaco,'Courier New',monospace;font-size:36px;font-weight:800;letter-spacing:8px;color:#be123c;display:inline-block;"">
                                    {code}
                                </span>
                            </div>
                            <p style=""font-size:13px;color:#6b7280;line-height:20px;margin:0 0 8px;"">
                                ⏱️ <strong>Validez:</strong> Este código expira en <strong>5 minutos</strong>.
                            </p>
                            <p style=""font-size:13px;color:#9ca3af;line-height:18px;margin:0;"">
                                Si no solicitaste este código, puedes ignorar este mensaje de forma segura. Tu cuenta permanece protegida.
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""padding:20px 32px;background-color:#f9fafb;border-top:1px solid #f3f4f6;text-align:center;"">
                            <p style=""font-size:12px;color:#9ca3af;margin:0;"">
                                © 2026 NextHappen. Plataforma de Ferias y Eventos.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        var payload = new
        {
            from = _options.FromEmail ?? "NextHappen <onboarding@resend.dev>",
            to = new[] { toEmail },
            subject = $"{code} es tu código de verificación de NextHappen",
            html = htmlContent
        };

        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(req);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("[Resend] Correo 2FA enviado exitosamente a {Email}", toEmail);
                return (true, null);
            }

            _logger.LogWarning("[Resend] Error enviando correo a {Email}. StatusCode: {Status}. Detalle: {Body}",
                toEmail, response.StatusCode, responseBody);

            if ((int)response.StatusCode == 403 && responseBody.Contains("testing emails"))
            {
                return (false, "En modo de prueba de Resend solo se puede enviar al correo del titular de la cuenta.");
            }

            return (false, $"Error al enviar el correo ({response.StatusCode}).");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Resend] Excepción enviando correo a {Email}", toEmail);
            return (false, ex.Message);
        }
    }
}
