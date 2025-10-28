using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Implementations.Security.Authentication.Interfaces;
using Data.Implementations.Security.Authentication;
using Data.Interfaces;
using Entity.Dtos.Login;
using Entity.Models.Security.Authentication;
using Microsoft.Extensions.Logging;
using Utilities.Interfaces;

namespace Business.Implementations.Security.Authentication
{
    public class UserVerificationBusiness : IUserVerificationBusiness
    {
        private readonly IUserVerificationCodeData _codeData;
        private readonly IUserData _userData;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _hasher;
        private readonly ILogger<UserVerificationBusiness> _logger;

        public UserVerificationBusiness(
            IUserVerificationCodeData codeData,
            IUserData userData,
            IEmailService emailService,
            IPasswordHasher hasher,
            ILogger<UserVerificationBusiness> logger)
        {
            _codeData = codeData;
            _userData = userData;
            _emailService = emailService;
            _hasher = hasher;
            _logger = logger;
        }

        public async Task GenerateAndSendCodeAsync(int userId)
        {
            var user = await _userData.GetById(userId)
                ?? throw new Exception("Usuario no encontrado");

            var code = new Random().Next(100000, 999999).ToString();
            var hash = _hasher.HashPassword(code);

            var record = new UserVerificationCode
            {
                UserId = user.Id,
                CodeHash = hash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CodeType = "LOGIN"
            };

            await _codeData.Save(record);

            string message = $"Tu código de verificación es: <strong>{code}</strong><br><br>Válido por 10 minutos.";
            await _emailService.SendEmailAsync(user.Email, message);

            _logger.LogInformation("Código de verificación enviado a {Email}", user.Email);
        }

        public async Task<VerificationResponseDto> VerifyCodeAsync(VerificationRequestDto dto)
        {
            var record = await _codeData.GetActiveCodeAsync(dto.UserId, "LOGIN");
            if (record == null)
                return new VerificationResponseDto { Success = false, Message = "Código expirado o no encontrado" };

            record.Attempts++;

            if (!_hasher.VerifyPassword(record.CodeHash, dto.Code))
            {
                await _codeData.Update(record);
                return new VerificationResponseDto { Success = false, Message = "Código incorrecto" };
            }

            record.IsUsed = true;
            record.ConsumedAt = DateTime.UtcNow;
            await _codeData.Update(record);

            return new VerificationResponseDto { Success = true, Message = "Verificación exitosa" };
        }
    }
}
