using AutoMapper;
using Business.Interfaces;
using Business.Interfaces.Operational;
using Data.Implementations;
using Entity.Dtos.Operational;
using Entity.Models;
using Entity.Models.Operational;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.Controllers.Implementations.Operational
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : RepositoryController<Vehicle, VehicleDto>
    {
        private readonly IVehicleBusiness _business;
        private readonly IRegisteredVehicleBusiness _registeredVehicleBusiness;
        private readonly IMapper _mapper;

        public VehicleController(IVehicleBusiness business, IMapper mapper, IRegisteredVehicleBusiness registeredVehicleBusiness)
            : base(business)
        {
            _business = business;
            _mapper = mapper;
            _registeredVehicleBusiness = registeredVehicleBusiness;
        }

        //[HttpPost]
        //public override async Task<ActionResult<VehicleDto>> Save(VehicleDto dto)
        //{
        //    try
        //    {
        //        // 1️⃣ Guardar el vehículo normalmente usando la lógica genérica
        //        VehicleDto dtoSaved = await _business.Save(dto);

        //        // 2️⃣ Asignar automáticamente un slot al vehículo recién creado
        //        RegisteredVehiclesDto registeredVehicle = await _registeredVehicleBusiness.RegisterVehicleWithSlotAsync(dtoSaved.Id, dto.ParkingId ?? 0);

        //        var response = new
        //        {
        //            Vehicle = dtoSaved,
        //            RegisteredVehicle = registeredVehicle
        //        };

        //        return CreatedAtRoute(new { id = dtoSaved.Id }, response);

        //    }
        //    catch (Exception ex)
        //    {
        //        var response = new ApiResponse<object>(null, false, ex.Message, null);
        //        return StatusCode(StatusCodes.Status500InternalServerError, response);
        //    }
        //}


        [HttpGet("join")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAllJoin()
        {
            try
            {
                var data = await _business.GetAllJoinAsync();
                if (data == null || !data.Any())
                {
                    var responseNull = new ApiResponse<IEnumerable<VehicleDto>>(null, false, "Registro no encontrado", null);
                    return NotFound(responseNull);
                }
                var response = new ApiResponse<IEnumerable<VehicleDto>>(data, true, "Ok", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<IEnumerable<VehicleDto>>(null, false, ex.Message.ToString(), null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("slot/{slotId}")]
        public async Task<IActionResult> GetActiveVehicleBySlot(int slotId)
        {
            var result = await _business.GetActiveVehicleBySlotAsync(slotId);

            if (result == null)
                return NotFound("No hay un vehículo activo en este slot.");

            return Ok(result);
        }


    }
}
