using Business.Implementations;
using Business.Implementations.Dashboard;
using Business.Implementations.Detection;
using Business.Implementations.Menu;
using Business.Implementations.Operational;
using Business.Implementations.Parameter;
using Business.Implementations.Security;
using Business.Implementations.Security.Authentication;
using Business.Implementations.Security.PasswordRecovery;
using Business.Interfaces;
using Business.Interfaces.Dashboard;
using Business.Interfaces.Detection;
using Business.Interfaces.Menu;
using Business.Interfaces.Operational;
using Business.Interfaces.Parameter;
using Business.Interfaces.Security;
using Business.Interfaces.Security.Authentication;
using Business.Interfaces.Security.PasswordRecovery;
using Data.Implementations;
using Data.Implementations.Dashboard;
using Data.Implementations.Menu;
using Data.Implementations.Operational;
using Data.Implementations.Parameter;
using Data.Implementations.Security;
using Data.Interfaces;
using Data.Interfaces.Dashboard;
using Data.Interfaces.Menu;
using Data.Interfaces.Operational;
using Data.Interfaces.Parameter;
using Data.Interfaces.Security;
using Entity.Context;
using Entity.Contexts.parking;
using Entity.Dtos.Security;
using Entity.Models.Security;
using Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Utilities.Audit.Services;
using Utilities.Audit.Strategy;
using Utilities.BackgroundTasks;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Helpers.Validators;
using Utilities.Implementations;
using Utilities.Interfaces;
using Web.Services;

namespace Web.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            // 🔹 Registrar el BackgroundService
            services.AddHostedService<KafkaConsumerService>();
            //segundo plano
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();
            // sin necesidad de crear Business o Data concreta
            services.AddScoped<IRepositoryBusiness<Rol, RolDto>, RepositoryBusiness<Rol, RolDto>>();
            services.AddScoped<IRepositoryData<Rol>, RepositoryData<Rol>>();
            //Obtener Usuario
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            // Inyección de dependencias para auditoría con Strategy
            
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IEmailService, EmailService>();
            // Genéricos base
            services.AddScoped(typeof(IRepositoryBusiness<,>), typeof(RepositoryBusiness<,>));
            services.AddScoped(typeof(IRepositoryData<>), typeof(RepositoryData<>));

            // Concretos
            services.AddScoped<IFormBusiness, FormBusiness>();
            services.AddScoped<IFormData, FormData>();

            services.AddScoped<IFormModuleBusiness, FormModuleBusiness>();
            services.AddScoped<IPersonParkignData, FormModuleData>();

            services.AddScoped<IModuleBusiness, ModuleBusiness>();
            services.AddScoped<IModuleData, ModuleData>();

            services.AddScoped<IPermissionBusiness, PermissionBusiness>();
            services.AddScoped<IPermissionData, PermissionData>();

            services.AddScoped<IPersonBusiness, PersonBusiness>();
            services.AddScoped<IPersonData, PersonData>();

            services.AddScoped<IRolBusiness, RolBusiness>();
            services.AddScoped<IRolData, RolData>();

            services.AddScoped<IRolFormPermissionBusiness, RolFormPermissionBusiness>();
            services.AddScoped<IRolFormPermissionData, RolFormPermissionData>();

            services.AddScoped<IRolParkingUserBusiness, RolParkingUserBusiness>();
            services.AddScoped<IRolParkingUserData, RolParkingUserData>();

            services.AddScoped<IUserBusiness, UserBusiness>();
            services.AddScoped<IUserData, UserData>();

            services.AddScoped<IBlackListBusiness, BlackListBusiness>();
            services.AddScoped<IBlackListData, BlackListData>();

            services.AddScoped<ICamaraBusiness, CamaraBusiness>();
            services.AddScoped<ICamaraData, CameraData>();

            services.AddScoped<IClientBusiness, ClientBusiness>();
            services.AddScoped<IClientData, ClientData>();

            services.AddScoped<IMembershipsBusiness, MembershipsBusiness>();
            services.AddScoped<IMembershipsData, MembershipsData>();

            services.AddScoped<IMemberShipTypeBusiness, MemberShipTypeBusiness>();
            services.AddScoped<IMemberShipTypeData, MemberShipTypeData>();

            services.AddScoped<IParkingBusiness, ParkingBusiness>();
            services.AddScoped<IParkingData, ParkingData>();

            services.AddScoped<IParkingCategoryBusiness, ParkingCategoryBusiness>();
            services.AddScoped<IParkingCategoryData, ParkingCategoryData>();

            services.AddScoped<IRatesBusiness, RatesBusiness>();
            services.AddScoped<IRatesData, RatesData>();

            services.AddScoped<IRatesTypeBusiness, RatesTypeBusiness>();
            services.AddScoped<IRatesTypeData, RatesTypeData>();

            services.AddScoped<IRegisteredVehicleBusiness, RegisteredVehicleBusiness>();
            services.AddScoped<IRegisteredVehiclesData, RegisteredVehicleData>();

            services.AddScoped<ISectorsBusiness, SectorsBusiness>();
            services.AddScoped<ISectorsData, SectorsData>();

            services.AddScoped<ISlotsBusiness, SlotsBusiness>();
            services.AddScoped<ISlotsData, SlotsData>();

            services.AddScoped<ITypeVehicleBusiness, TypeVehicleBusiness>();
            services.AddScoped<ITypeVehicleData, TypeVehicleData>();

            services.AddScoped<IVehicleBusiness, VehicleBusiness>();
            services.AddScoped<IVehicleData, VehicleData>();

            services.AddScoped<IZonesBusiness, ZonesBusiness>();
            services.AddScoped<IZonesData, ZonesData>();

            // Program.cs o Startup.cs (ConfigureServices)
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IDashboardBusiness, DashboardBusiness>();

            services.AddScoped<INotificationBusiness, NotificationBusiness>();
            services.AddScoped<INotificationData, NotificationData>();

            services.AddScoped<IVehicleDetectionManagerBusiness, VehicleDetectionManagerBusiness>();
            services.AddScoped<INotificationDispatcher, SignalRNotificationDispatcher>();
            services.AddScoped<IUserAuthenticationBusiness, UserAuthenticationBusiness>();

            services.AddScoped<IPasswordRecoveryBusiness, PasswordRecoveryBusiness>();





            services.AddScoped<IObtainTypeVehicle, ObtainTypeVehicle>();








            services.AddScoped<IParkingContext, ParkingContext>();
            services.AddScoped<IMenuData, MenuData>();
            services.AddScoped<IMenuBusiness, MenuBusiness>();




            services.AddTransient<Validations>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IPasswordReset, PasswordResett>();
            return services;
        }
    }
}