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
                new User { Id = 1, Username = "admin", Email = "admin@mail.com", Password = "$2a$12$C3DSGP6PRwi3a4hsLdnrs.kYnRkJ0PgR3ky/AbI5Dmem7U3e/lSpq", PersonId = 1, Asset = true },
                new User { Id = 2, Username = "usuario", Email = "usuario@mail.com", Password = "$2a$12$bvkOemZZo7d029/kwq5Duudeamk/pxdPn464EZOT6Ndbg6z06h.Gm", PersonId = 2, Asset = true },
                new User { Id = 3, Username = "Consumidor Final", Email = "usuario@mail.com", Password = "$2a$12$bvkOemZZo7d029/kwq5Duudeamk/pxdPn464EZOT6Ndbg6z06h.Gm", PersonId = 3, Asset = true }
            );

            // Módulos
            modelBuilder.Entity<Module>().HasData(
                new Module { Id = 1, Name = "Gestión", Description = "Módulo de gestión", Asset = true }
            );

            // Formularios
            modelBuilder.Entity<Form>().HasData(
                new Form { Id = 1, Name = "Principal", Description = "Formulario principal", Asset = true }
            );

            // FormModule
            modelBuilder.Entity<FormModule>().HasData(
                new FormModule { Id = 1, FormId = 1, ModuleId = 1, Asset = true }
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
            modelBuilder.Entity<RolFormPermission>().HasData(
                new RolFormPermission { Id = 1, RolId = 1, FormId = 1, PermissionId = 1, Asset = true },
                new RolFormPermission { Id = 2, RolId = 1, FormId = 1, PermissionId = 2, Asset = true },
                new RolFormPermission { Id = 3, RolId = 2, FormId = 1, PermissionId = 1, Asset = true }
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
