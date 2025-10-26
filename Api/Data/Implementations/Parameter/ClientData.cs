using AutoMapper;
using Data.Interfaces.Parameter;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Interfaces;

namespace Data.Implementations.Parameter
{
    public class ClientData : RepositoryData<Client>, IClientData
    {
        private readonly ILogger<ClientData> _logger;
        private readonly IAuditService _auditService;
        public ClientData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, ILogger<ClientData> logger, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {
            _logger = logger;
            _auditService = auditService;

        }


        public async Task<IEnumerable<ClientDto>> GetAllJoinAsync()
        {
            try
            {
                var parkingId = _parkingContext.ParkingId;

                if (parkingId == null)
                    throw new InvalidOperationException("No se ha establecido el ParkingId en el contexto actual.");

                var clients = await (
                    from client in _context.Clients.AsNoTracking()
                    join person in _context.Persons on client.PersonId equals person.Id
                    join user in _context.Users on person.Id equals user.PersonId
                    join rpu in _context.RolParkingUsers on user.Id equals rpu.UserId
                    where rpu.ParkingId == parkingId && (client.IsDeleted == false || client.IsDeleted == null)
                    select new ClientDto
                    {
                        Id = client.Id,
                        Name = client.Name,
                        PersonId = client.PersonId,
                        Person = $"{person.FirstName} {person.LastName}",
                        Asset = client.Asset,
                        IsDeleted = client.IsDeleted
                    }
                ).ToListAsync();

                return clients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los clientes asociados al parking.");
                throw new DataException("Error al obtener los clientes asociados al parking.", ex);
            }
        }




        public async Task<Client?> GetClientWithVehiclesByPersonIdAsync(int personId)
        {
            try
            {
                return await _context.Set<Client>()
                    .Include(c => c.Person)
                    .Include(c => c.Vehicles)
                    .FirstOrDefaultAsync(c => c.PersonId == personId && c.Asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo Client+Vehicles por PersonId {PersonId}", personId);
                throw;
            }
        }

    }
}
