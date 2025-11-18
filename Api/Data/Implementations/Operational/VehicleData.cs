using AutoMapper;
using Data.Interfaces.Operational;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos.Operational;
using Entity.Dtos.vehicle;
using Entity.Models.Operational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Operational
{
    public class VehicleData : RepositoryData<Vehicle>, IVehicleData
    {
        private readonly ILogger<VehicleData> _logger;

        public VehicleData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, ILogger<VehicleData> logger, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {
            _logger = logger;
        }
        //public async Task<IEnumerable<VehicleDto>> GetAllJoinAsync()
        //{
        //    return await _context.Vehicles
        //        .AsNoTracking()
        //        .Select(p => new VehicleDto
        //        {
        //            // --- BaseDto ---
        //            Id = p.Id,                      // int? en BaseDto
        //            Asset = p.Asset,                 // bool? en BaseDto
        //            IsDeleted = p.IsDeleted,         // bool en BaseDto

        //            // --- GenericDto ---
        //            Plate = p.Plate,                   // string en GenericDto
        //            Color = p.Color,

        //            // --- ZonesDto ---
        //            TypeVehicleId = p.TypeVehicleId,
        //            TypeVehicle = p.TypeVehicle != null
        //                ? p.TypeVehicle.Name
        //                : null,

        //            ClientId = p.ClientId,
        //            Client = p.Client != null
        //                ? p.Client.Name
        //                : null
        //        })
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<VehicleDto>> GetAllJoinAsync()
        {
            var parkingId = _parkingContext.ParkingId; // 👈 este viene del contexto actual

            var query =
                from v in _context.Vehicles
                    .AsNoTracking()
                    .Include(v => v.TypeVehicle)
                    .Include(v => v.Client)
                        .ThenInclude(c => c.Person)
                join c in _context.Clients on v.ClientId equals c.Id
                join p in _context.Persons on c.PersonId equals p.Id
                join u in _context.Users on p.Id equals u.PersonId
                join rpu in _context.RolParkingUsers on u.Id equals rpu.UserId
                where rpu.ParkingId == parkingId && v.IsDeleted == false  // 👈 filtro clave
                select new VehicleDto
                {
                    Id = v.Id,
                    Asset = v.Asset,
                    IsDeleted = v.IsDeleted,
                    Plate = v.Plate,
                    Color = v.Color,
                    TypeVehicleId = v.TypeVehicleId,
                    TypeVehicle = v.TypeVehicle != null ? v.TypeVehicle.Name : null,
                    ClientId = v.ClientId,
                    Client = v.Client != null
                        ? v.Client.Person.FirstName + " " + v.Client.Person.LastName
                        : null
                };

            return await query.ToListAsync();
        }


        public async Task<RegisteredVehicles?> GetActiveRegisteredVehicleBySlotAsync(int slotId)
        {
            return await _context.RegisteredVehicles
                .Include(rv => rv.Vehicle) // Para traer también la info del vehículo
                .FirstOrDefaultAsync(rv => rv.SlotsId == slotId && rv.ExitDate == null);
        }
        //public async Task<Vehicle?> GetVehicleByPlate(string plate)
        //{
        //    return await _context.Vehicles
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(v => v.Plate.Equals(plate, StringComparison.CurrentCultureIgnoreCase));
        //}

        public async Task<Vehicle?> GetVehicleByPlate(string plate)
        {
            return await _context.Vehicles
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Plate.ToLower() == plate.ToLower());
        }

        //public async Task<IEnumerable<VehicleDto>> GetVehiclesByClientIdAsync(int clientId)
        //{
        //    return await _context.Vehicles
        //        .AsNoTracking()
        //        .Where(v => v.ClientId == clientId && v.IsDeleted == false)
        //        .Include(v => v.TypeVehicle)
        //        .Include(v => v.Client)
        //            .ThenInclude(c => c.Person)
        //        .Select(v => new VehicleDto
        //        {
        //            Id = v.Id,
        //            Plate = v.Plate,
        //            Color = v.Color,

        //            TypeVehicleId = v.TypeVehicleId,
        //            TypeVehicle = v.TypeVehicle != null ? v.TypeVehicle.Name : null,

        //            ClientId = v.ClientId,
        //            Client = v.Client != null
        //                ? $"{v.Client.Person.FirstName} {v.Client.Person.LastName}"
        //                : null
        //        })
        //        .ToListAsync();
        //}


        //public async Task<IEnumerable<VehicleWithStatusDto>> GetVehiclesWithStatusByClientIdAsync(int clientId)
        //{
        //    return await _context.Vehicles
        //        .AsNoTracking()
        //        .Where(v => v.ClientId == clientId && v.IsDeleted == false)
        //        .Include(v => v.TypeVehicle)
        //        .Include(v => v.Client).ThenInclude(c => c.Person)
        //        .Select(v => new VehicleWithStatusDto
        //        {
        //            Id = v.Id,
        //            Plate = v.Plate,
        //            Color = v.Color,

        //            TypeVehicleId = v.TypeVehicleId,
        //            TypeVehicle = v.TypeVehicle != null ? v.TypeVehicle.Name : null,

        //            ClientId = v.ClientId,
        //            Client = v.Client != null
        //                ? v.Client.Person.FirstName + " " + v.Client.Person.LastName
        //                : null,

        //            IsInside = false 
        //        })
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<VehicleWithStatusDto>> GetVehiclesWithStatusByClientIdAsync(int clientId)
        {
            return await _context.Vehicles
                .AsNoTracking()
                .Where(v => v.ClientId == clientId && v.IsDeleted == false)
                .Include(v => v.TypeVehicle)
                .Include(v => v.Client).ThenInclude(c => c.Person)
                .Select(v => new VehicleWithStatusDto
                {
                    Id = v.Id,
                    Plate = v.Plate,
                    Color = v.Color,

                    TypeVehicleId = v.TypeVehicleId,
                    TypeVehicle = v.TypeVehicle != null ? v.TypeVehicle.Name : null,

                    ClientId = v.ClientId,
                    Client = v.Client != null
                        ? v.Client.Person.FirstName + " " + v.Client.Person.LastName
                        : null,

                    // estos 4 campos los llenamos en Business
                    IsInside = false,
                    EntryDate = null,
                    SlotId = null,
                    SlotName = null,
                    TimeInside = null
                })
                .ToListAsync();
        }





    }
}
