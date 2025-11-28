using Business.Interfaces;
using Business.Interfaces.Detection;
using Business.Interfaces.Operational;
using Business.Interfaces.Parameter;
using Entity.Dtos.Operational;
using Entity.Dtos.Parameter;
using Entity.Enums;
using Entity.Models;
using Entity.Records;
using Microsoft.Extensions.Logging;
using System;
using Utilities.BackgroundTasks;
using Utilities.Exceptions;
using Utilities.Interfaces;

namespace Business.Implementations.Detection
{
    public class VehicleDetectionManagerBusiness : IVehicleDetectionManagerBusiness
    {
        private readonly ILogger<VehicleDetectionManagerBusiness> _logger;
        private readonly IVehicleBusiness _vehicleBusiness;
        private readonly IRegisteredVehicleBusiness _registeredVehicleBusiness;
        private readonly INotificationBusiness _notificationBusiness;
        private readonly IBlackListBusiness _blackListBusiness;
        private readonly ITypeVehicleBusiness _typeVehicleBusiness;
        private readonly ICamaraBusiness _camaraBusiness;
        private readonly IBackgroundTaskQueue _taskQueue;

        public VehicleDetectionManagerBusiness(
            ILogger<VehicleDetectionManagerBusiness> logger,
            IVehicleBusiness vehicleBusiness,
            IRegisteredVehicleBusiness registeredVehicleBusiness,
            INotificationBusiness notificationBusiness,
            IBlackListBusiness blackListBusiness,
            ITypeVehicleBusiness typeVehicleBusiness,
            IBackgroundTaskQueue taskQueue,
            ICamaraBusiness camaraBusiness)
        {
            _logger = logger;
            _vehicleBusiness = vehicleBusiness;
            _registeredVehicleBusiness = registeredVehicleBusiness;
            _notificationBusiness = notificationBusiness;
            _blackListBusiness = blackListBusiness;
            _typeVehicleBusiness = typeVehicleBusiness;
            _taskQueue = taskQueue;
            _camaraBusiness = camaraBusiness;
        }

        public async Task ProcessDetectionAsync(PlateDetectedEventRecord evt, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Procesando detección de placa {Plate}", evt.Plate);

                // Conversión segura de string → int
                if (!int.TryParse(evt.CameraId, out int cameraId))
                {
                    _logger.LogWarning("CameraId inválido o no numérico: {CameraId}", evt.CameraId);
                    return; // o lanzar excepción si lo prefieres
                }
                CameraDto camera = await _camaraBusiness.GetById(cameraId);
                evt.ParkingId = camera.ParkingId;
                //  Notificación inicial
                await NotifyAsync(evt.ParkingId, "Detección iniciada", $"Se detectó la placa **{evt.Plate}**.", "Info");

                //  Buscar vehículo existente
                var vehicle = await _vehicleBusiness.GetVehicleByPlate(evt.Plate);

                if (vehicle == null)
                {
                    await HandleNewVehicleAsync(evt);
                    return;
                }

                //  Validar lista negra
                bool isBlacklisted = await _blackListBusiness.ExistsAsync(b => b.VehicleId == vehicle.Id);
                if (isBlacklisted)
                {
                    await NotifyAsync(evt.ParkingId, "🚫 Vehículo en lista negra detectado",
                        $"Se ha detectado la placa **{evt.Plate}**, registrada en la lista negra. " +
                        $"No se permitió su registro de entrada. Verifica el historial o toma acción inmediata.",
                        "Warning");
                    return;
                }

                //  Determinar si el vehículo tiene una entrada activa
                bool hasActiveEntry = await _registeredVehicleBusiness.ExistsAsync(r =>
                    r.VehicleId == vehicle.Id &&
                    r.ExitDate == null &&
                    r.Status == ERegisterStatus.In &&
                    r.Asset == true);

                if (hasActiveEntry)
                    await HandleVehicleExitAsync(evt, vehicle);
                else
                    await HandleVehicleEntryAsync(evt, vehicle);
            }
            catch (BusinessException ex)
            {
                // Aquí caen, por ejemplo:
                // - "No hay slots disponibles para este tipo de vehículo."
                _logger.LogWarning(ex, "Error de negocio procesando detección de placa {Plate}", evt.Plate);

                await NotifyAsync(
                    evt.ParkingId,
                    " No se pudo completar el registro",
                    // Puedes usar el mensaje de la excepción o uno más específico
                    ex.Message,
                    "Warning"
                );

                // NO re-lanzamos para que el consumer de Kafka no se caiga por esto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado procesando detección de placa {Plate}", evt.Plate);

                string msg =
                    $"Ocurrió un error inesperado al procesar la detección de la placa **{evt.Plate}**. ";

                await NotifyAsync(
                    evt.ParkingId,
                    "❌ Error procesando detección",
                    msg,
                    "Error"
                );
            }
        }

        // ============================================================
        //  HANDLE NEW VEHICLES
        // ============================================================
        private async Task HandleNewVehicleAsync(PlateDetectedEventRecord evt)
        {
            int typeVehicle = await _typeVehicleBusiness.GetTypeVehicleByPlate(evt.Plate);

            VehicleDto vehicleDto = new()
            {
                Plate = evt.Plate,
                Color = "",
                TypeVehicleId = typeVehicle,
                ClientId = 3
            };

            VehicleDto vehicleResult = await _vehicleBusiness.Save(vehicleDto);
            RegisteredVehiclesDto entryRegister = await _registeredVehicleBusiness.RegisterVehicleWithSlotAsync(vehicleResult.Id, evt.ParkingId ?? 0);

            await NotifyAsync(evt.ParkingId,
                "🚘 Vehículo nuevo registrado y entrada creada",
                $"Se registró el nuevo vehículo con placa **{evt.Plate}** (tipo {vehicleDto.TypeVehicleId}). " +
                $"Se generó su entrada y se asignó el slot **{entryRegister.Slots}**.",
                "Success",
                entryRegister.Id);
        }

        // ============================================================
        //  HANDLE AUTOMATIC ENTRY AND EXIT
        // ============================================================
        private async Task HandleVehicleEntryAsync(PlateDetectedEventRecord evt, VehicleDto vehicle)
        {
            RegisteredVehiclesDto register = await _registeredVehicleBusiness.RegisterVehicleWithSlotAsync(vehicle.Id, evt.ParkingId ?? 0);

            await NotifyAsync(evt.ParkingId,
                "🚘 Entrada registrada automáticamente",
                $"El vehículo con placa **{evt.Plate}** ingresó al parqueadero **{evt.ParkingId}**. " +
                $"Se le asignó el slot **{register.Slots}**.",
                "Success",
                register.Id);
        }

        private async Task HandleVehicleExitAsync(PlateDetectedEventRecord evt, VehicleDto vehicle)
        {
            RegisteredVehiclesDto register = await _registeredVehicleBusiness.RegisterVehicleExitAsync(vehicle.Id);

            await NotifyAsync(evt.ParkingId,
                "🚗 Salida registrada automáticamente",
                $"El vehículo con placa **{evt.Plate}** ha salido del parqueadero **{evt.ParkingId}**. " +
                $"El slot **{register.Slots}** ha sido liberado correctamente.",
                "Success",
                register.Id);
        }

        // ============================================================
        //  CENTRALIZED NOTIFICATION METHOD
        // ==========================================================
        private async Task NotifyAsync(int? parkingId, string title, string message, string type, int? relatedId = null)
        {
            await _notificationBusiness.EnqueueAndNotifyAsync(new NotificationDto
            {
                ParkingId = parkingId ?? 0,
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                RelatedEntityId = relatedId
            });
        }
    }
}
