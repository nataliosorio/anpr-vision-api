using AutoMapper;
using Business.Implementations.Parameter;
using Business.Interfaces;
using Business.Interfaces.Operational;
using Business.Interfaces.Parameter;
using Business.Interfaces.Security;
using Data.Implementations;
using Data.Implementations.Operational;
using Data.Interfaces.Operational;
using Entity.Dtos.Dashboard;
using Entity.Dtos.Operational;
using Entity.Dtos.Parameter;
using Entity.Dtos.Security;
using Entity.Enums;
using Entity.Models.Operational;
using Entity.Models.Parameter;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Utilities.Exceptions;
using Utilities.Helpers.Validators;
using Utilities.Implementations.Ticket;
using Utilities.Interfaces;
using Utilities.Interfaces.Ticket;
using Utilities.Pdf;

namespace Business.Implementations.Operational
{
   
    public class RegisteredVehicleBusiness : RepositoryBusiness<RegisteredVehicles, RegisteredVehiclesDto>, IRegisteredVehicleBusiness
    {
        private readonly IRegisteredVehiclesData _data;
        private readonly IVehicleBusiness _vehicleBusiness;
        private readonly ISectorsBusiness _sectorsBusiness;
        private readonly ISlotsBusiness _slotsBusiness;
        private readonly IMapper _mapper;
        private readonly ITypeVehicleBusiness _typeVehicleBusiness;
        private readonly ITicketService _ticketService;
        private readonly IPersonBusiness _personBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IRolBusiness _rolBusiness;
        private readonly IRolParkingUserBusiness _rolParkingUserBusiness;
        private readonly IClientBusiness _clientBusiness;
        private readonly IPasswordHasher _passwordHasher;
        public RegisteredVehicleBusiness(IRegisteredVehiclesData data, IMapper mapper, IVehicleBusiness vehicleBusiness,ITypeVehicleBusiness typeVehicleBusiness, ISectorsBusiness sectorsBusiness, ISlotsBusiness slotsBusiness, ITicketService ticketService, IPersonBusiness personBusiness, IUserBusiness userBusiness, IRolBusiness rolBusiness, IRolParkingUserBusiness rolParkingUserBusiness, IClientBusiness clientBusiness, IPasswordHasher passwordHasher)
            : base(data, mapper)
        {
            _data = data;
            _vehicleBusiness = vehicleBusiness;
            _sectorsBusiness = sectorsBusiness;
            _mapper = mapper;
            _slotsBusiness = slotsBusiness;
            _typeVehicleBusiness = typeVehicleBusiness;
            _ticketService = ticketService;
            _personBusiness = personBusiness;
            _userBusiness = userBusiness;
            _rolBusiness = rolBusiness;
            _rolParkingUserBusiness = rolParkingUserBusiness;
            _clientBusiness = clientBusiness;
            _passwordHasher = passwordHasher;
        }


        public async Task<IEnumerable<RegisteredVehiclesDto>> GetAllJoinAsync()
        {
            try
            {
                IEnumerable<RegisteredVehiclesDto> entities = await _data.GetAllJoinAsync();
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron registros de vehiculos.");
                return entities;
            }
            catch (InvalidOperationException invEx)
            {
                throw new InvalidOperationException("error: ", invEx);
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException("error: ", argEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las registros .", ex);
            }
        }

        // ---------- NUEVOS MÉTODOS ----------
        public async Task<int> GetTotalCurrentlyParkedByParkingAsync(int parkingId)
        {
            if (parkingId <= 0) throw new ArgumentException("parkingId inválido.");
            try
            {
                return await _data.GetTotalCurrentlyParkedByParkingAsync(parkingId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de vehículos estacionados por parking.", ex);
            }
        }

        public async Task<int> GetTotalCurrentlyParkedAsync()
        {
            try
            {
                return await _data.GetTotalCurrentlyParkedAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de vehículos estacionados (global).", ex);
            }
        }

        public Task<VehicleTypeDistributionDto> GetVehicleTypeDistributionGlobalAsync(bool includeZeros = true)
        => _data.GetVehicleTypeDistributionGlobalAsync(includeZeros);

        public Task<List<OccupancyItemDto>> GetSectorOccupancyByZoneAsync(int zoneId)
        => _data.GetSectorOccupancyByZoneAsync(zoneId);

        public async Task<IEnumerable<RegisteredVehiclesDto>> GetByParkingAsync(int parkingId)
        {
            try
            {
                if (parkingId <= 0)
                    throw new ArgumentException("Debe especificar un ID de parqueadero válido.");

                var data = await _data.GetByParkingAsync(parkingId);

                if (!data.Any())
                    throw new InvalidOperationException("No se encontraron registros para este parqueadero.");

                return data;
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException($"Error: {argEx.Message}");
            }
            catch (InvalidOperationException invEx)
            {
                throw new InvalidOperationException($"Error: {invEx.Message}", invEx);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al obtener los vehículos por parqueadero.", ex);
            }
        }

        // Método para registrar vehículo y asignar slot
        public async Task<RegisteredVehiclesDto> RegisterVehicleWithSlotAsync(int vehicleId, int parkingId)
        {
            // 1Obtener el vehículo existente
            VehicleDto vehicle = await _vehicleBusiness.GetById(vehicleId) ?? throw new Exception("Vehículo no encontrado.");

            //  Obtener sectores compatibles con el tipo de vehículo
            Slots assignedSlot = await _slotsBusiness.AssignAvailableSlotAsync(vehicle.TypeVehicleId, parkingId);

            // 6. Marcar el slot como ocupado
            assignedSlot.IsAvailable = false;
            SlotsDto assignedSlotDto = _mapper.Map<SlotsDto>(assignedSlot);
            await _slotsBusiness.Update(assignedSlotDto);


            //  Crear RegisteredVehicle
            RegisteredVehicles registeredVehicle = new RegisteredVehicles
            {
                VehicleId = vehicle.Id,
                SlotsId = assignedSlot.Id,
                EntryDate = DateTime.UtcNow,
                Status = ERegisterStatus.In,
                Asset = true
            };

            await _data.Save(registeredVehicle);

            RegisteredVehiclesDto returnRegisteredVehicle = _mapper.Map<RegisteredVehiclesDto>(registeredVehicle);

            returnRegisteredVehicle.Slots = assignedSlot.Name;

            return returnRegisteredVehicle;
        }
        public async Task<RegisteredVehiclesDto> RegisterVehicleExitAsync(int vehicleId)
        {
            //  Buscar un registro activo del vehículo
            RegisteredVehicles? activeRegister = await _data.GetActiveRegisterByVehicleIdAsync(vehicleId) ?? throw new BusinessException("No se encontró una entrada activa para este vehículo.");

            //  Marcar salida
            activeRegister.ExitDate = DateTime.UtcNow;
            activeRegister.Status = ERegisterStatus.Out;

            //  Liberar el slot
            if (activeRegister.SlotsId.HasValue)
            {
                SlotsDto slot = await _slotsBusiness.GetById(activeRegister.SlotsId.Value);
                if (slot != null)
                {
                    slot.IsAvailable = true;
                    await _slotsBusiness.Update(slot);
                }
            }

            //  Guardar cambios
            await _data.Update(activeRegister);

            //  Mapear y devolver DTO
            RegisteredVehiclesDto dto = _mapper.Map<RegisteredVehiclesDto>(activeRegister);
            dto.Slots = (await _slotsBusiness.GetById(activeRegister.SlotsId ?? 0))?.Name ?? "N/A";
            return dto;
        }
      

        public async Task<ManualEntryResponseDto> ManualRegisterVehicleEntryAsync(ManualVehicleEntryDto dto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // 1) Crear Person
                    var personDto = new PersonDto
                    {
                        FirstName = dto.ClientName,
                        LastName = "",
                        Email = dto.ClientEmail,
                        Document = "",
                        Phone = "",
                        Age = 0
                    };
                    var createdPerson = await _personBusiness.Save(personDto);

                    // 2) Crear User
                    string defaultPassword = "TempPass123!"; // Contraseña temporal
                    var userDto = new UserDto
                    {
                        Username = Guid.NewGuid().ToString("N"), // Username único
                        Email = dto.ClientEmail,
                        Password = _passwordHasher.HashPassword(defaultPassword),
                        PersonId = createdPerson.Id
                    };
                    var createdUser = await _userBusiness.Save(userDto);

                    // 3) Crear Client
                    var clientDto = new ClientDto
                    {
                        PersonId = createdPerson.Id,
                        Name = dto.ClientName
                    };
                    var createdClient = await _clientBusiness.Save(clientDto);

                    // 4) Obtener rol "Usuario" y asignar RolParkingUser
                    var userRole = await _rolBusiness.GetByNameAsync("Usuario");
                    if (userRole == null)
                        throw new BusinessException("El rol 'Usuario' no existe en el sistema.");

                    var rolParkingUserDto = new RolParkingUserDto
                    {
                        UserId = createdUser.Id,
                        RolId = userRole.Id,
                        ParkingId = dto.ParkingId
                    };
                    await _rolParkingUserBusiness.Save(rolParkingUserDto);

                    // 5) Procesar vehículo
                    string normalizedPlate = dto.Plate.Trim().ToUpper();
                    var vehicle = await _vehicleBusiness.GetVehicleByPlate(normalizedPlate);

                    if (vehicle == null)
                    {
                        var newVehicleDto = new VehicleDto
                        {
                            Plate = normalizedPlate,
                            Color = "",
                            TypeVehicleId = dto.TypeVehicleId,
                            ClientId = createdClient.Id
                        };

                        vehicle = await _vehicleBusiness.Save(newVehicleDto);
                    }

                    bool hasActiveEntry = await _data.GetActiveRegisterByVehicleIdAsync(vehicle.Id) != null;
                    if (hasActiveEntry)
                        throw new BusinessException($"El vehículo con placa {normalizedPlate} ya tiene una entrada activa.");

                    // 6) Registrar vehículo con slot
                    RegisteredVehiclesDto registeredVehicle = await RegisterVehicleWithSlotAsync(vehicle.Id, dto.ParkingId);

                    var fullVehicle = await _vehicleBusiness.GetById(vehicle.Id);

                    registeredVehicle.Vehicle = fullVehicle.Plate;
                    registeredVehicle.Sector = fullVehicle.TypeVehicle;

                    // 7) Generar Ticket usando el nuevo servicio
                    byte[] pdfBytes = _ticketService.GenerateTicketPdf(registeredVehicle);

                    // 8) Armar respuesta final
                    var responseDto = _mapper.Map<ManualEntryResponseDto>(registeredVehicle);
                    responseDto.TicketPdfBytes = pdfBytes;

                    // Completar la transacción
                    scope.Complete();

                    return responseDto;
                }
                catch (Exception ex)
                {
                    // El scope se desechará sin Complete, haciendo rollback
                    if (ex is BusinessException) throw;
                    throw new BusinessException($"Error en el registro manual para la placa {dto.Plate}: {ex.Message}", ex);
                }
            }
        }


        public async Task<RegisteredVehiclesDto?> GetRegisteredVehicleFullDtoAsync(int id)
        {
            var entity = await _data.GetFullByIdAsync(id);
            if (entity == null)
                return null;

            // Mapea las propiedades básicas
            var dto = _mapper.Map<RegisteredVehiclesDto>(entity);

            // Añadir manualmente datos que no están en el auto-mapping
            dto.Vehicle = entity.Vehicle?.Plate;
            dto.Sector = entity.Vehicle?.TypeVehicle?.Name;
            dto.Slots = entity.Slots?.Name;

            return dto;
        }

    }
}
