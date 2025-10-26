using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Utilities.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string message)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPass"];
            var fromEmail = _configuration["Email:From"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var subject = "Notificación del Sistema";

            var htmlBody = $@"
gi <!doctype html>
<html>
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <title>Notificación del Sistema</title>
  </head>
  <body style=""margin:0; padding:0; background:#f3f4f6; -webkit-font-smoothing:antialiased; -ms-text-size-adjust:100%; -webkit-text-size-adjust:100%;"">
    <!-- Preheader (texto de vista previa) -->
    <div style=""display:none; max-height:0; overflow:hidden; opacity:0; mso-hide:all;"">
      Información importante sobre tu cuenta
    </div>

    <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background:#f3f4f6; padding:24px 0;"">
      <tr>
        <td align=""center"">
          <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""width:600px; max-width:600px; background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 8px 22px rgba(0,0,0,0.08);"">
            <!-- Top bar -->
            <tr>
              <td style=""height:4px; background:#7c5cff;"">
                <!-- puede degradar a #19c3ff si prefieres un degradado estático con imagen -->
              </td>
            </tr>

            <!-- Header -->
            <tr>
              <td style=""padding:28px 28px 8px 28px; font-family:Segoe UI, Arial, sans-serif; color:#111827;"">
                <h2 style=""margin:0 0 6px 0; font-size:22px; line-height:1.25; font-weight:700;"">Hola,</h2>
                <p style=""margin:0; font-size:14px; line-height:1.6; color:#4b5563;"">
                  Te compartimos información importante relacionada con tu cuenta.
                </p>
              </td>
            </tr>

            <!-- Mensaje principal (dinámico) -->
            <tr>
              <td style=""padding:12px 28px 0 28px; font-family:Segoe UI, Arial, sans-serif; color:#1f2937;"">
                <p style=""margin:0; font-size:15px; line-height:1.7; color:#374151;"">{message}</p>
              </td>
            </tr>

            <!-- Callout -->
            <tr>
              <td style=""padding:16px 28px 0 28px;"">
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse:separate;"">
                  <tr>
                    <td style=""border:1px solid #e6e6ff; background:#f7f7ff; border-radius:10px; padding:14px 16px; font-family:Segoe UI, Arial, sans-serif; font-size:14px; line-height:1.6; color:#1f2937;"">
                      <strong>Nota de seguridad:</strong> este mensaje forma parte de nuestras medidas de protección y
                      comunicación transparente. Si no reconoces esta actividad, ignora este correo y mantén tus credenciales en confidencialidad.
                    </td>
                  </tr>
                </table>
              </td>
            </tr>

            <!-- Texto adicional -->
            <tr>
              <td style=""padding:18px 28px 0 28px; font-family:Segoe UI, Arial, sans-serif;"">
                <p style=""margin:0; font-size:15px; line-height:1.7; color:#374151;"">
                  En <strong>nuestro sistema</strong> trabajamos para ofrecer una experiencia confiable y consistente.
                  Agradecemos tu tiempo y tu confianza. Si ya has completado la acción correspondiente, no es necesario realizar pasos adicionales.
                </p>
              </td>
            </tr>

            <!-- Divider -->
            <tr>
              <td style=""padding:22px 28px 0 28px;"">
                <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                  <tr>
                    <td style=""height:1px; background:#e5e7eb;""></td>
                  </tr>
                </table>
              </td>
            </tr>

            <!-- Meta / fecha -->
            <tr>
              <td style=""padding:12px 28px 12px 28px; font-family:Segoe UI, Arial, sans-serif;"">
                <p style=""margin:0; font-size:13px; color:#6b7280;"">
                  Fecha de emisión: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC
                </p>
              </td>
            </tr>

            <!-- Footer -->
            <tr>
              <td style=""padding:0 28px 26px 28px; font-family:Segoe UI, Arial, sans-serif; text-align:center;"">
                <p style=""margin:0; font-size:12px; color:#9ca3af;"">
                  Este correo fue generado automáticamente; por favor, no respondas a este mensaje.
                </p>
              </td>
            </tr>

          </table>
        </td>
      </tr>
    </table>
  </body>
</html>";


            var mailMessage = new MailMessage(fromEmail, to, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
