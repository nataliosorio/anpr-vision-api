using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces.Security.PasswordRecovery;
using Data.Interfaces;
using Data.Interfaces.Security;
using Entity.Models.Security;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Interfaces;

namespace Business.Implementations.Security.PasswordRecovery
{
    public class PasswordRecoveryBusiness : IPasswordRecoveryBusiness
    {
        private readonly IUserData _userData;
        private readonly IPasswordReset _passwordResetData;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<PasswordRecoveryBusiness> _logger;

        public PasswordRecoveryBusiness(
            IUserData userData,
            IPasswordReset passwordResetData,
            IEmailService emailService,
            IPasswordHasher passwordHasher,
            ILogger<PasswordRecoveryBusiness> logger)
        {
            _userData = userData;
            _passwordResetData = passwordResetData;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        // === Paso 1: Solicitar recuperación (envía código al correo) ===
        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _userData.GetUserByEmailsync(email);
            if (user == null)
                throw new BusinessException("El usuario no existe con ese correo.");

            var nowUtc = DateTime.UtcNow;
            var windowStart = nowUtc.AddHours(-1);

            // Límite de solicitudes: 5 por hora
            int count = await _passwordResetData.CountRequestsSinceAsync(user.Id, windowStart);
            if (count >= 5)
                throw new BusinessException("Has alcanzado el límite de 5 códigos por hora. Intenta más tarde.");

            // Generar y guardar el código
            string code = GenerateResetCode();
            var reset = new PasswordReset
            {
                UsuarioId = user.Id,
                Code = code,
                ExpiryDate = nowUtc.AddMinutes(10),
                Used = false,
                CreatedAt = nowUtc
            };

            await _passwordResetData.Add(reset);

            // Enviar correo
            await _emailService.SendEmailAsync(user.Email, $"Tu código de recuperación es: {code}");

            _logger.LogInformation("Código de recuperación enviado a {Email}", email);
        }

        // === Paso 2: Validar código ===
        public async Task<bool> VerifyResetCodeAsync(string email, string code)
        {
            var user = await _userData.GetUserByEmailsync(email);
            if (user == null)
                throw new BusinessException("El usuario no existe.");

            var reset = await _passwordResetData.GetValidCode(user.Id, code);
            return reset != null;
        }

        // === Paso 3: Validar código y resetear contraseña ===
        public async Task VerifyCodeAndResetPasswordAsync(string email, string code, string newPassword)
        {
            var user = await _userData.GetUserByEmailsync(email);
            if (user == null)
                throw new BusinessException("El usuario no existe.");

            var reset = await _passwordResetData.GetValidCode(user.Id, code);
            if (reset == null)
                throw new BusinessException("Código inválido o expirado.");

            var hashedPassword = _passwordHasher.HashPassword(newPassword);
            user.Password = hashedPassword;
            await _userData.Update(user);

            await _passwordResetData.MarkAsUsed(reset);
            _logger.LogInformation("Contraseña restablecida para el usuario {Email}", email);
        }

        // === Helper ===
        private string GenerateResetCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6 dígitos
        }
    }
}
