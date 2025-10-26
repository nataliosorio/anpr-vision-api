using AutoMapper;
using Business.Interfaces.Parameter;
using Data.Interfaces;
using Data.Interfaces.Parameter;
using Entity.Contexts.parking;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Helpers.Validators;

namespace Business.Implementations.Parameter
{
    public class ClientBusiness : RepositoryBusiness<Client, ClientDto>, IClientBusiness
    {
        private readonly IClientData _data;
        private readonly IMapper _mapper;
        private readonly IRepositoryData<Person> _personRepository;
        private readonly IUserData _userRepository;


        private readonly IRepositoryData<RolParkingUser> _rolParkingUserRepository;
        private readonly IParkingContext _parkingContext;
        public ClientBusiness(
        IClientData data,
        IMapper mapper,
        IRepositoryData<Person> personRepository,
        IUserData userRepository,
        IRepositoryData<RolParkingUser> rolParkingUserRepository,
        IParkingContext parkingContext)
        : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _personRepository = personRepository;
            _userRepository = userRepository;
            _rolParkingUserRepository = rolParkingUserRepository;
            _parkingContext = parkingContext;
        }



        public async Task<IEnumerable<ClientDto>> GetAllByParkingAsync()
        {
            try
            {
                var clients = await _data.GetAllJoinAsync();
                if (!clients.Any())
                    throw new InvalidOperationException("No se encontraron clientes asociados a este parking.");

                return clients;
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al obtener los clientes por parking.", ex);
            }
        }


        // Reemplaza el método Save por este:
        public override async Task<ClientDto> Save(ClientDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "PersonaId");

                if (dto.PersonId <= 0)
                    throw new ArgumentException("El campo PersonaId debe ser mayor que 0.");

                var persona = await _personRepository.GetById(dto.PersonId);
                if (persona == null)
                    throw new InvalidOperationException($"No existe una persona con Id {dto.PersonId}.");

                // 1) Validación por PersonId (rápida, SQL)
                bool existePorPersonId = await _data.ExistsAsync(x => x.PersonId == dto.PersonId);
                if (existePorPersonId)
                    throw new InvalidOperationException("Ya existe un cliente asociado a esta persona.");


                // Mapear y guardar
                Client entity = _mapper.Map<Client>(dto);
                entity.Asset = true;

                entity = await _data.Save(entity);

                return _mapper.Map<ClientDto>(entity);
            }
            catch (InvalidOperationException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al registrar el cliente.", ex);
            }
        }
        //public override async Task<ClientDto> Save(ClientDto dto)
        //{
        //    try
        //    {
        //        Validations.ValidateDto(dto);

        //        // ==============================
        //        // 1️⃣ Validar y obtener persona
        //        // ==============================
        //        if (dto.PersonId <= 0)
        //            throw new ArgumentException("Debe asociarse una persona al cliente.");

        //        var persona = await _personRepository.GetById(dto.PersonId);
        //        if (persona == null)
        //            throw new InvalidOperationException($"No existe una persona con Id {dto.PersonId}.");

        //        // ==============================
        //        // 2️⃣ Crear el User asociado
        //        // ==============================
        //        // Validar si ya tiene user
        //        bool userExists = await _userRepository.ExistsAsync(u => u.PersonId == persona.Id);
        //        User user;

        //        if (!userExists)
        //        {
        //            user = new User
        //            {
        //                Username = persona.FirstName,
        //                Email = persona.Email,
        //                Password = BCrypt.Net.BCrypt.HashPassword("123456"), // contraseña temporal
        //                PersonId = persona.Id
        //            };

        //            user = await _userRepository.Save(user);
        //        }
        //        else
        //        {
        //            user = await _userRepository.GetByPersonIdAsync(persona.Id)
        //                ?? throw new InvalidOperationException("Error al recuperar usuario existente.");
        //        }


        //        // ==============================
        //        // 3️⃣ Crear RolParkingUser
        //        // ==============================
        //        int? parkingId = _parkingContext.ParkingId;
        //        if (parkingId == null || parkingId == 0)    
        //            throw new InvalidOperationException("No se encontró el ParkingId en el contexto.");

        //        bool rolUserExists = await _rolParkingUserRepository.ExistsAsync(r =>
        //            r.UserId == user.Id && r.ParkingId == parkingId);

        //        if (!rolUserExists)
        //        {
        //            var rolParkingUser = new RolParkingUser
        //            {
        //                RolId = 2, // 🔸 Id del rol por defecto para clientes (ajústalo según tu catálogo)
        //                UserId = user.Id,
        //                ParkingId = parkingId.Value
        //            };

        //            await _rolParkingUserRepository.Save(rolParkingUser);
        //        }

        //        // ==============================
        //        // 4️⃣ Validar cliente duplicado
        //        // ==============================
        //        bool existePorPersonId = await _data.ExistsAsync(x => x.PersonId == persona.Id);
        //        if (existePorPersonId)
        //            throw new InvalidOperationException("Ya existe un cliente asociado a esta persona.");

        //        // ==============================
        //        // 5️⃣ Crear el cliente
        //        // ==============================
        //        Client entity = _mapper.Map<Client>(dto);
        //        entity.PersonId = persona.Id;
        //        entity.Person = null; // ✅ Asegúrate de que no intente insertar persona
        //        entity.Asset = true;

        //        entity = await _data.Save(entity);

        //        return _mapper.Map<ClientDto>(entity);





        //    }
        //    catch (InvalidOperationException) { throw; }
        //    catch (ArgumentException) { throw; }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessException("Error al registrar el cliente y sus entidades asociadas.", ex);
        //    }
        //}

        // Reemplaza el método Update por este:
        public override async Task Update(ClientDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Id", "PersonaId");

                if (dto.Id <= 0)
                    throw new ArgumentException("El campo Id debe ser mayor que 0.");

                var clienteExistente = await _data.GetById(dto.Id);
                if (clienteExistente == null)
                    throw new InvalidOperationException("El cliente no existe.");

                if (dto.PersonId <= 0)
                    throw new ArgumentException("El atributo PersonaId es obligatorio.");

                var persona = await _personRepository.GetById(dto.PersonId);
                if (persona == null)
                    throw new InvalidOperationException("No existe la persona que se ha seleccionado.");

                // Si cambió la PersonId, comprobar PersonId (excluyendo propio Id)
                if (dto.PersonId != clienteExistente.PersonId)
                {
                    bool existclient = await _data.ExistsAsync(x => x.PersonId == dto.PersonId);
                    if (existclient)
                        throw new InvalidOperationException("Ya existe otro cliente asociado a esta persona.");
                }

                _mapper.Map(dto, clienteExistente);

                await _data.Update(clienteExistente);
            }
            catch (InvalidOperationException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar el cliente.", ex);
            }
        }
    }
}
