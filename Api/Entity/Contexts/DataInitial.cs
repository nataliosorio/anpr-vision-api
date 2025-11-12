using Entity.Enums;
using Entity.Models.Operational;
using Entity.Models.Parameter;
using Entity.Models.Security;
using Microsoft.EntityFrameworkCore;
using System;

namespace Entity.Contexts
{
    internal class DataInitial
    {
        public static void Data(ModelBuilder modelBuilder)
        {
            // Roles
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Name = "Administrador", Description = "Rol de administrador", Asset = true },
                new Rol { Id = 2, Name = "Usuario", Description = "Rol de usuario estándar", Asset = true }
            );

            // Permisos
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "Ver", Description = "Permiso para ver", Asset = true },
                new Permission { Id = 2, Name = "Editar", Description = "Permiso para editar", Asset = true },
                new Permission { Id = 3, Name = "Eliminar", Description = "Permiso para eliminar", Asset = true }
            );

            // Personas
            modelBuilder.Entity<Person>().HasData(
                new Person { Id = 1, FirstName = "Admin", LastName = "Principal", Phone = "111111111", Document = "0001", Email = "admin@gmail.com", Age = 30, Asset = true },
                new Person { Id = 2, FirstName = "Usuario", LastName = "Demo", Phone = "222222222", Document = "0002", Email = "usuario@gmail.com", Age = 25, Asset = true },
                new Person { Id = 3, FirstName = "Consumidor", LastName = "Final", Phone = "222222222222", Document = "222222222222", Email = "consumidorFinal@gmail.com", Age = 18, Asset = true }
            );

            // Usuarios (contraseñas hasheadas con BCrypt)
            // Admin123!  → hash generado admin123
            // User123!   → hash generado
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Email = "nataliaosorio973@gmail.com", Password = "$2a$12$C3DSGP6PRwi3a4hsLdnrs.kYnRkJ0PgR3ky/AbI5Dmem7U3e/lSpq", PersonId = 1, Asset = true },
                new User { Id = 2, Username = "usuario", Email = "usuario@mail.com", Password = "$2a$12$bvkOemZZo7d029/kwq5Duudeamk/pxdPn464EZOT6Ndbg6z06h.Gm", PersonId = 2, Asset = true },
                new User { Id = 3, Username = "Consumidor Final", Email = "usuario@mail.com", Password = "$2a$12$bvkOemZZo7d029/kwq5Duudeamk/pxdPn464EZOT6Ndbg6z06h.Gm", PersonId = 3, Asset = true }
            );

            // Módulos
            modelBuilder.Entity<Module>().HasData(
                 new Module { Id = 1, Name = "Dashboard", Description = "Módulo principal que muestra estadísticas, métricas y resumen general del sistema.", Asset = true, IsDeleted = false },
                 new Module
                 {
                     Id = 2,
                     Name = "Monitoreo y Control de Acceso",
                     Description = "Módulo encargado del monitoreo en tiempo real y control del acceso de vehículos.",
                     Asset = true,
                     IsDeleted = false
                 },
                 new Module { Id = 3, Name = "Módulo de Infraestructura", Description = "Módulo para la gestión física del parqueadero: zonas, sectores y espacios disponibles.", Asset = true, IsDeleted = false },
                 new Module
                 {
                     Id = 4,
                     Name = "Módulo Operacional",
                     Description = "Módulo encargado de las operaciones principales del sistema, incluyendo control, registro y supervisión de procesos en el parqueadero.",
                     Asset = true,
                     IsDeleted = false
                 },
                 new Module
                 {
                     Id = 5,
                     Name = "Módulo de Parámetros",
                     Description = "Módulo encargado de la configuración y gestión de los parámetros generales del sistema, como tipos, categorías y valores base.",
                     Asset = true,
                     IsDeleted = false
                 },
                 new Module
                 {
                     Id = 6,
                     Name = "Módulo de Seguridad",
                     Description = "Módulo encargado de la administración de usuarios, roles, permisos y control de acceso al sistema.",
                     Asset = true,
                     IsDeleted = false
                 }




            );

            // Formularios
            modelBuilder.Entity<Form>().HasData(
               new Form { Id = 1, Name = "Dashboard", Description = "Vista principal que muestra estadísticas, métricas y resumen general del sistema.", Url = "/dashboard", Asset = true, IsDeleted = false },
                new Form { Id = 2, Name = "Cámaras", Description = "Interfaz para el monitoreo en tiempo real de las cámaras del parqueadero.", Url = "/cameras-index", Asset = true, IsDeleted = false },
                new Form { Id = 3, Name = "Vehículos", Description = "Formulario para la gestión y consulta de los vehículos registrados en el sistema.", Url = "/vehicles-index", Asset = true, IsDeleted = false },
                new Form { Id = 4, Name = "Distribución del Parqueadero", Description = "Vista que permite visualizar y administrar la distribución física del parqueadero.", Url = "/parking-management", Asset = true, IsDeleted = false },
                new Form { Id = 5, Name = "Tarifas", Description = "Formulario para la creación, edición y gestión de tarifas aplicables.", Url = "/rates-index", Asset = true, IsDeleted = false },
                new Form { Id = 6, Name = "Registros de Entradas", Description = "Interfaz para la visualización y control de los registros de entrada de vehículos.", Url = "/registeredVehicle-index", Asset = true, IsDeleted = false },
                new Form { Id = 7, Name = "Lista Negra", Description = "Módulo de control para la administración de la lista negra de vehículos o clientes.", Url = "/blackList-index", Asset = true, IsDeleted = false },
                new Form { Id = 8, Name = "Clientes", Description = "Formulario para la gestión de clientes asociados al parqueadero.", Url = "/client-index", Asset = true, IsDeleted = false },
                new Form { Id = 9, Name = "Tipo de Vehículos", Description = "Gestión de los tipos de vehículos admitidos en el sistema.", Url = "/TypeVehicle-index", Asset = true, IsDeleted = false },
                new Form { Id = 10, Name = "Tipo de Tarifas", Description = "Gestión de las categorías y tipos de tarifas disponibles en el sistema.", Url = "/RatesType-index", Asset = true, IsDeleted = false },
                new Form { Id = 11, Name = "Zonas", Description = "Formulario para administrar las zonas del parqueadero.", Url = "/Zones-index", Asset = true, IsDeleted = false },
                new Form { Id = 12, Name = "Sectores", Description = "Interfaz para la gestión y configuración de los sectores dentro del parqueadero.", Url = "/sectors-index", Asset = true, IsDeleted = false },
                new Form { Id = 13, Name = "Espacios", Description = "Formulario encargado de la administración de los espacios de parqueo.", Url = "/slots-index", Asset = true, IsDeleted = false },
                new Form { Id = 14, Name = "Roles", Description = "Gestión y configuración de los roles del sistema.", Url = "/role-index", Asset = true, IsDeleted = false },
                new Form { Id = 15, Name = "Permisos", Description = "Formulario para administrar los permisos disponibles del sistema.", Url = "/permission-index", Asset = true, IsDeleted = false },
                new Form { Id = 16, Name = "Módulos", Description = "Gestión de los módulos funcionales del sistema.", Url = "/module-index", Asset = true, IsDeleted = false },
                new Form { Id = 17, Name = "Formularios por Módulos", Description = "Formulario para relacionar formularios con módulos.", Url = "/form-module-index", Asset = true, IsDeleted = false },
                new Form { Id = 18, Name = "Permisos por Roles y Formularios", Description = "Gestión de los permisos asignados a roles por cada formulario.", Url = "/rolFormPermission-index", Asset = true, IsDeleted = false },
                new Form { Id = 19, Name = "Usuarios", Description = "Formulario para la administración de usuarios del sistema.", Url = "/user-index", Asset = true, IsDeleted = false },
                new Form { Id = 20, Name = "Personas", Description = "Gestión de la información personal asociada a los usuarios.", Url = "/persons-index", Asset = true, IsDeleted = false }
             );

            // FormModule
            // Relación entre Formularios y Módulos
            modelBuilder.Entity<FormModule>().HasData(
                // Dashboard
                new FormModule { Id = 1, FormId = 1, ModuleId = 1, Asset = true, IsDeleted = false },

                // Monitoreo y Control de Acceso
                new FormModule { Id = 2, FormId = 2, ModuleId = 2, Asset = true, IsDeleted = false }, // Cámaras
                new FormModule { Id = 3, FormId = 4, ModuleId = 2, Asset = true, IsDeleted = false }, // Distribución del Parqueadero

                // Módulo Operacional
                new FormModule { Id = 4, FormId = 6, ModuleId = 4, Asset = true, IsDeleted = false }, // Registros de Entradas
                new FormModule { Id = 5, FormId = 7, ModuleId = 4, Asset = true, IsDeleted = false }, // Lista Negra
                new FormModule { Id = 6, FormId = 3, ModuleId = 4, Asset = true, IsDeleted = false }, // Vehículos

                // Módulo de Parámetros
                new FormModule { Id = 7, FormId = 8, ModuleId = 5, Asset = true, IsDeleted = false },  // Clientes
                new FormModule { Id = 8, FormId = 5, ModuleId = 5, Asset = true, IsDeleted = false },  // Tarifas
                new FormModule { Id = 9, FormId = 10, ModuleId = 5, Asset = true, IsDeleted = false }, // Tipo de Tarifas
                new FormModule { Id = 10, FormId = 12, ModuleId = 5, Asset = true, IsDeleted = false },// Sectores
                new FormModule { Id = 11, FormId = 13, ModuleId = 5, Asset = true, IsDeleted = false },// Espacios
                new FormModule { Id = 12, FormId = 9, ModuleId = 5, Asset = true, IsDeleted = false }, // Tipos de Vehículos
                new FormModule { Id = 13, FormId = 11, ModuleId = 5, Asset = true, IsDeleted = false },// Zonas

                // Módulo de Seguridad
                new FormModule { Id = 14, FormId = 14, ModuleId = 6, Asset = true, IsDeleted = false }, // Roles
                new FormModule { Id = 15, FormId = 15, ModuleId = 6, Asset = true, IsDeleted = false }, // Permisos
                new FormModule { Id = 16, FormId = 16, ModuleId = 6, Asset = true, IsDeleted = false }, // Módulos
                new FormModule { Id = 17, FormId = 17, ModuleId = 6, Asset = true, IsDeleted = false }, // Formularios por Módulos
                new FormModule { Id = 18, FormId = 18, ModuleId = 6, Asset = true, IsDeleted = false }, // Permisos por Roles y Formularios
                new FormModule { Id = 19, FormId = 19, ModuleId = 6, Asset = true, IsDeleted = false }, // Usuarios
                new FormModule { Id = 20, FormId = 20, ModuleId = 6, Asset = true, IsDeleted = false }  // Personas
            );


            // ParkingCategory
            modelBuilder.Entity<ParkingCategory>().HasData(
                new ParkingCategory { Id = 1, Name = "General", Code = "GEN", Description = "Categoría general", Asset = true },
                new ParkingCategory { Id = 2, Name = "VIP", Code = "VIP", Description = "Categoría exclusiva", Asset = true }
            );
            // Parking
            modelBuilder.Entity<Parking>().HasData(
                new Parking { Id = 1, Name = "Parqueadero Central", Location = "Centro", ParkingCategoryId = 1, Asset = true },
                new Parking { Id = 2, Name = "Parqueadero Norte", Location = "Norte", ParkingCategoryId = 2, Asset = true }
            );

            // RolUser
            modelBuilder.Entity<RolParkingUser>().HasData(
                new RolParkingUser { Id = 1, RolId = 1, UserId = 1, ParkingId = 1, Asset = true },
                new RolParkingUser { Id = 2, RolId = 2, UserId = 2, ParkingId = 2, Asset = true },
                new RolParkingUser { Id = 3, RolId = 2, UserId = 3, ParkingId = 2, Asset = true }
            );

            // RolFormPermission
            // Permisos por Rol y Formulario
            modelBuilder.Entity<RolFormPermission>().HasData(
                new RolFormPermission { Id = 1, RolId = 1, FormId = 1, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 2, RolId = 1, FormId = 2, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 3, RolId = 1, FormId = 3, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 4, RolId = 1, FormId = 4, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 5, RolId = 1, FormId = 5, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 6, RolId = 1, FormId = 6, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 7, RolId = 1, FormId = 7, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 8, RolId = 1, FormId = 8, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 9, RolId = 1, FormId = 9, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 10, RolId = 1, FormId = 10, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 11, RolId = 1, FormId = 11, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 12, RolId = 1, FormId = 12, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 13, RolId = 1, FormId = 13, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 14, RolId = 1, FormId = 14, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 15, RolId = 1, FormId = 15, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 16, RolId = 1, FormId = 16, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 17, RolId = 1, FormId = 17, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 18, RolId = 1, FormId = 18, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 19, RolId = 1, FormId = 19, PermissionId = 1, Asset = true, IsDeleted = false },
                new RolFormPermission { Id = 20, RolId = 1, FormId = 20, PermissionId = 1, Asset = true, IsDeleted = false }
            );


            // TypeVehicle
            modelBuilder.Entity<TypeVehicle>().HasData(
                new TypeVehicle { Id = 1, Name = "Auto", Asset = true },
                new TypeVehicle { Id = 2, Name = "Moto", Asset = true },
                new TypeVehicle { Id = 3, Name = "Camión", Asset = true }
            );


            // Zones
            modelBuilder.Entity<Zones>().HasData(
                new Zones { Id = 1, Name = "Zona A", ParkingId = 1, Asset = true },
                new Zones { Id = 2, Name = "Zona B", ParkingId = 1, Asset = true },
                new Zones { Id = 3, Name = "Zona VIP", ParkingId = 2, Asset = true }
            );

            // Sectors
            modelBuilder.Entity<Sectors>().HasData(
                new Sectors { Id = 1, Name = "Sector 1", Capacity = 10, ZonesId = 1, TypeVehicleId = 1, Asset = true },
                new Sectors { Id = 2, Name = "Sector 2", Capacity = 5, ZonesId = 2, TypeVehicleId = 2, Asset = true },
                new Sectors { Id = 3, Name = "Sector VIP", Capacity = 3, ZonesId = 3, TypeVehicleId = 1, Asset = true }
            );

            // Slots
            modelBuilder.Entity<Slots>().HasData(
                new Slots { Id = 1, Name = "A1", IsAvailable = true, SectorsId = 1, Asset = true },
                new Slots { Id = 2, Name = "A2", IsAvailable = true, SectorsId = 1, Asset = true },
                new Slots { Id = 3, Name = "B1", IsAvailable = true, SectorsId = 2, Asset = true },
                new Slots { Id = 4, Name = "VIP1", IsAvailable = true, SectorsId = 3, Asset = true }
            );

            // RatesType
            modelBuilder.Entity<RatesType>().HasData(
                new RatesType { Id = 1, Name = "Hora", Description = "Tarifa por hora", Asset = true }
            );

            // Rates (fechas fijas en UTC)
            modelBuilder.Entity<Rates>().HasData(
                new Rates { Id = 1, Name = "Tarifa Día", Type = "Hora", Amount = 2.5M, StarHour = new DateTime(2025, 1, 1, 8, 0, 0, DateTimeKind.Utc), EndHour = new DateTime(2025, 1, 1, 20, 0, 0, DateTimeKind.Utc), Year = 2025, ParkingId = 1, RatesTypeId = 1, TypeVehicleId = 1, Asset = true },
                new Rates { Id = 2, Name = "Tarifa Noche", Type = "Hora", Amount = 1.5M, StarHour = new DateTime(2025, 1, 1, 20, 0, 0, DateTimeKind.Utc), EndHour = new DateTime(2025, 1, 2, 6, 0, 0, DateTimeKind.Utc), Year = 2025, ParkingId = 1, RatesTypeId = 1, TypeVehicleId = 2, Asset = true },
                new Rates { Id = 3, Name = "Tarifa VIP", Type = "Hora", Amount = 5M, StarHour = new DateTime(2025, 1, 1, 8, 0, 0, DateTimeKind.Utc), EndHour = new DateTime(2025, 1, 1, 20, 0, 0, DateTimeKind.Utc), Year = 2025, ParkingId = 2, RatesTypeId = 1, TypeVehicleId = 1, Asset = true }
            );

            // Clients
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Name = "Cliente Demo", PersonId = 1, Asset = true },
                new Client { Id = 2, Name = "Cliente Premium", PersonId = 2, Asset = true },
                new Client { Id = 3, Name = "Consumidor Final", PersonId = 3, Asset = true }
            );

            // Vehicles
            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle { Id = 1, Plate = "ABC123", Color = "Rojo", TypeVehicleId = 1, ClientId = 1, Asset = true },
                new Vehicle { Id = 2, Plate = "XYZ987", Color = "Negro", TypeVehicleId = 2, ClientId = 1, Asset = true },
                new Vehicle { Id = 3, Plate = "TRK456", Color = "Blanco", TypeVehicleId = 3, ClientId = 2, Asset = true }
            );

            // BlackList
            modelBuilder.Entity<BlackList>().HasData(
                new BlackList { Id = 1, Reason = "Infracción grave", RestrictionDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), VehicleId = 3, Asset = true }
            );

            // Camera
            modelBuilder.Entity<Camera>().HasData(
                new Camera { Id = 1, Name = "Cam 1", Resolution = "1080p", Url = "http://cam1", ParkingId = 1, Asset = true },
                new Camera { Id = 2, Name = "Cam VIP", Resolution = "4K", Url = "http://cam-vip", ParkingId = 2, Asset = true }
            );
            // RegisteredVehicles
            // RegisteredVehicles
            modelBuilder.Entity<RegisteredVehicles>().HasData(
                new RegisteredVehicles
                {
                    Id = 1,
                    EntryDate = new DateTime(2025, 1, 1, 8, 0, 0, DateTimeKind.Utc),
                    ExitDate = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                    VehicleId = 1,
                    SlotsId = 1,
                    Asset = true,
                    Status = ERegisterStatus.Out   // 🔹 Se fue (tiene ExitDate)
                },
                new RegisteredVehicles
                {
                    Id = 2,
                    EntryDate = new DateTime(2025, 1, 1, 9, 0, 0, DateTimeKind.Utc),
                    VehicleId = 2,
                    SlotsId = 3,
                    Asset = true,
                    Status = ERegisterStatus.In    // 🔹 Sigue dentro (sin ExitDate)
                }
            );


            // MemberShipType
            modelBuilder.Entity<MemberShipType>().HasData(
                new MemberShipType { Id = 1, Name = "Mensual", Description = "Membresía mensual", PriceBase = 50, DurationDaysBase = 30, Asset = true },
                new MemberShipType { Id = 2, Name = "Anual", Description = "Membresía anual", PriceBase = 500, DurationDaysBase = 365, Asset = true }
            );
            // Memberships
            modelBuilder.Entity<Memberships>().HasData(
                new Memberships { Id = 1, MembershipTypeId = 1, VehicleId = 1, StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2025, 1, 31, 0, 0, 0, DateTimeKind.Utc), PriceAtPurchase = 50, DurationDays = 30, Currency = "USD", Asset = true },
                new Memberships { Id = 2, MembershipTypeId = 2, VehicleId = 2, StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PriceAtPurchase = 500, DurationDays = 365, Currency = "USD", Asset = true }
            );
        }
    }
}
