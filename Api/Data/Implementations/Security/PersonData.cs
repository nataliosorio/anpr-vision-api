using AutoMapper;
using Data.Interfaces.Security;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Models.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Security
{
    public class PersonData : RepositoryData<Person>, IPersonData
    {
        public PersonData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {

        }

        public async Task<IEnumerable<Person>> GetAllByParkingAsync()
        {
            try
            {
                var parkingId = _parkingContext.ParkingId; // 👈 del contexto actual

                var persons = await (
                    from p in _context.Persons.AsNoTracking()
                    join u in _context.Users on p.Id equals u.PersonId
                    join rpu in _context.RolParkingUsers on u.Id equals rpu.UserId
                    where rpu.ParkingId == parkingId && (p.IsDeleted == false || p.IsDeleted == null)
                    select new Person
                    {
                        Id = p.Id,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Document = p.Document,
                        Phone = p.Phone,
                        Email = p.Email,
                        Age = p.Age,
                        Asset = p.Asset,
                        IsDeleted = p.IsDeleted
                    }
                ).Distinct().ToListAsync();

                return persons;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener personas por parking: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Person>> GetUnlinkedAsync()
        {
            var persons = await _context.Persons
                .AsNoTracking()
                .Where(p => (p.IsDeleted == false || p.IsDeleted == null))
                .Where(p => !_context.Clients.Any(c => c.PersonId == p.Id)) 
                .ToListAsync();

            return persons;
        }

        public async Task<IEnumerable<Person>> GetPersonUnlinked()
        {
            var persons = await _context.Persons
               .AsNoTracking()
               .Where(p => (p.IsDeleted == false || p.IsDeleted == null))
               .Where(p => !_context.Users.Any(u => u.PersonId == p.Id))
               .ToListAsync();

            return persons;

        }


    }
}
