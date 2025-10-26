using AutoMapper;
using Data.Interfaces.Operational;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos.Operational;
using Entity.Models.Operational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Operational
{
    public class BlackListData : RepositoryData<BlackList>, IBlackListData
    {
        public BlackListData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {

        }

        //public async Task<IEnumerable<BlackListDto>> GetAllJoinAsync()
        //{
        //    return await _context.BlackList
        //        .AsNoTracking()
        //        .Select(p => new BlackListDto
        //        {
        //            // --- BaseDto ---
        //            Id = p.Id,                      // int? en BaseDto
        //            Asset = p.Asset,                 // bool? en BaseDto
        //            IsDeleted = p.IsDeleted,         // bool en BaseDto

        //            // --- GenericDto ---

        //            Reason = p.Reason,
        //            RestrictionDate = p.RestrictionDate,




        //            // --- ZonesDto ---
        //            VehicleId = p.VehicleId,
        //            Vehicle = p.Vehicle != null  
        //                ? p.Vehicle.Plate
        //                : null
        //        })
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<BlackListDto>> GetAllJoinAsync()
        {
            var parkingId = _parkingContext.ParkingId; // 👈 del contexto actual

            var query =
                from b in _context.BlackList.AsNoTracking()
                join v in _context.Vehicles.Include(v => v.TypeVehicle).Include(v => v.Client) on b.VehicleId equals v.Id
                join c in _context.Clients on v.ClientId equals c.Id
                join p in _context.Persons on c.PersonId equals p.Id
                join u in _context.Users on p.Id equals u.PersonId
                join rpu in _context.RolParkingUsers on u.Id equals rpu.UserId
                where rpu.ParkingId == parkingId && b.IsDeleted == false
                select new BlackListDto
                {
                    // --- BaseDto ---
                    Id = b.Id,
                    Asset = b.Asset,
                    IsDeleted = b.IsDeleted,

                    // --- BlackListDto ---
                    Reason = b.Reason,
                    RestrictionDate = b.RestrictionDate,
                    VehicleId = b.VehicleId,

                    // 👇 Mostramos la placa (ya está en tu DTO)
                    Vehicle = v != null ? v.Plate : null
                };

            return await query.ToListAsync();
        }




    }
}
