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
using Microsoft.Extensions.Logging;
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
        private readonly IBlackListBusiness _blackListBusiness;
        private readonly IEmailService _emailService;
        private readonly ILogger<RegisteredVehicleBusiness> _logger;
        public RegisteredVehicleBusiness(IRegisteredVehiclesData data, IMapper mapper, IVehicleBusiness vehicleBusiness,ITypeVehicleBusiness typeVehicleBusiness, ISectorsBusiness sectorsBusiness, ISlotsBusiness slotsBusiness, ITicketService ticketService, IPersonBusiness personBusiness, IUserBusiness userBusiness, IRolBusiness rolBusiness, IRolParkingUserBusiness rolParkingUserBusiness, IClientBusiness clientBusiness, IPasswordHasher passwordHasher, IBlackListBusiness blackListBusiness, IEmailService emailService, ILogger<RegisteredVehicleBusiness> logger)
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
            _blackListBusiness = blackListBusiness;
            _emailService = emailService;
            _logger = logger;
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
            try
            {
                string normalizedPlate = dto.Plate.Trim().ToUpper();
                var vehicle = await _vehicleBusiness.GetVehicleByPlate(normalizedPlate);

                if (vehicle == null)
                {
                    // Validar que se proporcionen datos del cliente para vehículo nuevo
                    if (string.IsNullOrWhiteSpace(dto.ClientName) || string.IsNullOrWhiteSpace(dto.ClientEmail))
                        throw new BusinessException("Para vehículos nuevos, se requieren el nombre y correo del cliente.");

                    return await HandleNewVehicleForManualEntryAsync(dto, normalizedPlate);
                }
                else
                {
                    return await HandleExistingVehicleForManualEntryAsync(dto, vehicle, normalizedPlate);
                }
            }
            catch (Exception ex)
            {
                if (ex is BusinessException) throw;
                throw new BusinessException($"Error en el registro manual para la placa {dto.Plate}: {ex.Message}", ex);
            }
        }

        private async Task<ManualEntryResponseDto> HandleNewVehicleForManualEntryAsync(ManualVehicleEntryDto dto, string normalizedPlate)
        {
            // Validar que TypeVehicleId esté presente para vehículos nuevos
            if (!dto.TypeVehicleId.HasValue)
                throw new BusinessException("El tipo de vehículo es requerido para vehículos nuevos.");

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Crear cliente, usuario, etc.
                    var createdClient = await CreateClientForManualEntryAsync(dto);

                    // Crear vehículo
                    var newVehicleDto = new VehicleDto
                    {
                        Plate = normalizedPlate,
                        Color = "",
                        TypeVehicleId = dto.TypeVehicleId.Value,
                        ClientId = createdClient.Id
                    };
                    var vehicle = await _vehicleBusiness.Save(newVehicleDto);

                    // Registrar entrada
                    var response = await RegisterManualEntryAsync(vehicle, dto.ParkingId);
                    scope.Complete();
                    return response;
                }
                catch
                {
                    // Rollback
                    throw;
                }
            }
        }

        private async Task<ManualEntryResponseDto> HandleExistingVehicleForManualEntryAsync(ManualVehicleEntryDto dto, VehicleDto vehicle, string normalizedPlate)
        {
            // Validar lista negra
            bool isBlacklisted = await _blackListBusiness.ExistsAsync(b => b.VehicleId == vehicle.Id);
            if (isBlacklisted)
                throw new BusinessException($"El vehículo con placa {normalizedPlate} está en la lista negra.");

            // Validar entrada activa
            bool hasActiveEntry = await _data.GetActiveRegisterByVehicleIdAsync(vehicle.Id) != null;
            if (hasActiveEntry)
                throw new BusinessException($"El vehículo con placa {normalizedPlate} ya tiene una entrada activa.");

            // Registrar entrada (sin transacción ya que no crea entidades)
            return await RegisterManualEntryAsync(vehicle, dto.ParkingId);
        }

        private async Task<ClientDto> CreateClientForManualEntryAsync(ManualVehicleEntryDto dto)
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
            string defaultPassword = dto.Plate.Trim().ToUpper(); // Contraseña = placa en mayúsculas

            var userDto = new UserDto
            {
                Username = dto.ClientEmail.Trim(),
                Email = dto.ClientEmail.Trim(),
                Password = defaultPassword, // TEXTO PLANO, se hashea en UserBusiness
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

            // Enviar correo
            try
            {
                string message = $"Bienvenido {dto.ClientName} a ANPR Vision.\n\nYa puedes ingresar a la app para tener un seguimiento de tu vehículo.\n\nCredenciales:\nNombre de usuario: {createdUser.Username}\nContraseña: {defaultPassword}\n\nRecomendamos cambiar tu contraseña lo antes posible por seguridad.";
                await _emailService.SendEmailAsync(dto.ClientEmail, message);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx, "Error enviando correo de bienvenida a {Email}", dto.ClientEmail);
                // No fallar la operación por error en email
            }

            return createdClient;
        }

        private async Task<ManualEntryResponseDto> RegisterManualEntryAsync(VehicleDto vehicle, int parkingId)
        {
            // Registrar vehículo con slot
            RegisteredVehiclesDto registeredVehicle = await RegisterVehicleWithSlotAsync(vehicle.Id, parkingId);

            var fullVehicle = await _vehicleBusiness.GetById(vehicle.Id);
            registeredVehicle.Vehicle = fullVehicle.Plate;
            registeredVehicle.Sector = fullVehicle.TypeVehicle;

            // Generar Ticket
            byte[] pdfBytes = _ticketService.GenerateTicketPdf(registeredVehicle);

            // Armar respuesta
            var responseDto = _mapper.Map<ManualEntryResponseDto>(registeredVehicle);
            responseDto.TicketPdfBytes = pdfBytes;

            return responseDto;
        }


        public async Task<VehicleValidationResultDto> ValidateVehiclePlateAsync(VehicleValidationRequestDto request)
        {
            try
            {
                string normalizedPlate = request.Plate.Trim().ToUpper();
                var vehicle = await _vehicleBusiness.GetVehicleByPlate(normalizedPlate);

                var result = new VehicleValidationResultDto
                {
                    Exists = vehicle != null,
                    IsBlacklisted = false,
                    HasActiveEntry = false,
                    TypeVehicleId = vehicle?.TypeVehicleId,
                    ClientName = vehicle?.Client,
                    VehicleColor = vehicle?.Color
                };

                if (vehicle != null)
                {
                    // Verificar lista negra
                    result.IsBlacklisted = await _blackListBusiness.ExistsAsync(b => b.VehicleId == vehicle.Id);

                    // Verificar entrada activa
                    result.HasActiveEntry = await _data.GetActiveRegisterByVehicleIdAsync(vehicle.Id) != null;

                    // Mensaje basado en estado
                    if (result.IsBlacklisted)
                    {
                        result.Message = "Vehículo en lista negra. No se permite entrada.";
                    }
                    else if (result.HasActiveEntry)
                    {
                        result.Message = "Vehículo ya tiene una entrada activa. Puede registrar salida.";
                    }
                    else
                    {
                        result.Message = "Vehículo registrado. Puede crear nueva entrada.";
                    }
                }
                else
                {
                    result.Message = "Vehículo no registrado. Puede crear nueva entrada con datos del cliente.";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando placa {Plate}", request.Plate);
                throw new BusinessException($"Error al validar la placa: {ex.Message}", ex);
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
