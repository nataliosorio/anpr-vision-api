using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Models.Security;
using Microsoft.EntityFrameworkCore;
using Utilities.Interfaces;

namespace Utilities.Implementations
{
    public class PasswordResett : IPasswordReset
    {
        private readonly ApplicationDbContext _context;


        public PasswordResett(ApplicationDbContext context)
        {
            _context = context;
        }

   
        public async Task Add(PasswordReset reset)
        {
            _context.PasswordResets.Add(reset);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordReset?> GetValidCode(int userId, string code)
        {
            return await _context.PasswordResets
                .Where(r => r.UsuarioId == userId &&
                            r.Code == code &&
                            !r.Used &&
                            r.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }

     





        public async Task MarkAsUsed(PasswordReset reset)
        {
            reset.Used = true;
            _context.PasswordResets.Update(reset);
            await _context.SaveChangesAsync();
        }

      


        public async Task CleanOldResets(int days = 30)
        {
            var old = await _context.PasswordResets
                .Where(r => r.ExpiryDate < DateTime.UtcNow.AddDays(-days))
                .ToListAsync();

            if (old.Any())
            {
                _context.PasswordResets.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
        }


        // conteo de solicitudes en la ventana
        public Task<int> CountRequestsSinceAsync(int userId, DateTime sinceUtc)
        {
            return _context.PasswordResets
                .Where(r => r.UsuarioId == userId && r.CreatedAt >= sinceUtc)
                .CountAsync();
        }

        // la más antigua dentro de la ventana (para calcular cuándo libera cupo)
        public Task<DateTime?> OldestRequestSinceAsync(int userId, DateTime sinceUtc)
        {
            return _context.PasswordResets
                .Where(r => r.UsuarioId == userId && r.CreatedAt >= sinceUtc)
                .OrderBy(r => r.CreatedAt)
                .Select(r => (DateTime?)r.CreatedAt)
                .FirstOrDefaultAsync();
        }







    }
}
