using Entity.Models.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Utilities.Implementations
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly ILogger<PasswordHasher> _logger;
        private const int WorkFactor = 12; // ajustable por config

        public PasswordHasher(ILogger<PasswordHasher> logger)
        {
            _logger = logger;
        }
      

        /// <summary>
        /// Genera el hash seguro de una contraseña usando BCrypt.
        /// </summary>
        public string HashPassword(string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("La contraseña no puede estar vacía.");

                // BCrypt genera automáticamente la sal y la incluye en el resultado
                var hashed = BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
                _logger.LogInformation("Contraseña hasheada correctamente con BCrypt (longitud {Length})", hashed.Length);
                return hashed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar hash de contraseña");
                throw;
            }
        }

        /// <summary>
        /// Verifica si una contraseña ingresada coincide con el hash almacenado.
        /// </summary>
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hashedPassword) || !hashedPassword.StartsWith("$2"))
                {
                    _logger.LogWarning("El formato del hash almacenado es inválido o no corresponde a BCrypt: {Hash}", hashedPassword);
                    return false;
                }

                bool isMatch = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);

                _logger.LogInformation("Resultado verificación contraseña: {Result}", isMatch);
                return isMatch;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al verificar hash (posible cadena inválida o dañada)");
                return false;
            }
        }
    
}
}
