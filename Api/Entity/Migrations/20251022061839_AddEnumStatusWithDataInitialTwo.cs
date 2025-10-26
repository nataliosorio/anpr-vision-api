using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddEnumStatusWithDataInitialTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberShipTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PriceBase = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DurationDaysBase = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberShipTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParkingCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Document = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RatesTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatesTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeVehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormModule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormId = table.Column<int>(type: "integer", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormModule_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormModule_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parkings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Location = table.Column<string>(type: "text", nullable: false),
                    ParkingCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parkings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parkings_ParkingCategories_ParkingCategoryId",
                        column: x => x.ParkingCategoryId,
                        principalTable: "ParkingCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolFormPermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RolId = table.Column<int>(type: "integer", nullable: false),
                    FormId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolFormPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolFormPermission_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolFormPermission_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolFormPermission_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cameras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Resolution = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    ParkingId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cameras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cameras_Parkings_ParkingId",
                        column: x => x.ParkingId,
                        principalTable: "Parkings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParkingId = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    RelatedEntityId = table.Column<int>(type: "integer", nullable: true),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Parkings_ParkingId",
                        column: x => x.ParkingId,
                        principalTable: "Parkings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StarHour = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndHour = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    ParkingId = table.Column<int>(type: "integer", nullable: false),
                    RatesTypeId = table.Column<int>(type: "integer", nullable: false),
                    TypeVehicleId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rates_Parkings_ParkingId",
                        column: x => x.ParkingId,
                        principalTable: "Parkings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rates_RatesTypes_RatesTypeId",
                        column: x => x.RatesTypeId,
                        principalTable: "RatesTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rates_TypeVehicles_TypeVehicleId",
                        column: x => x.TypeVehicleId,
                        principalTable: "TypeVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParkingId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zones_Parkings_ParkingId",
                        column: x => x.ParkingId,
                        principalTable: "Parkings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Plate = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: true),
                    TypeVehicleId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vehicles_TypeVehicles_TypeVehicleId",
                        column: x => x.TypeVehicleId,
                        principalTable: "TypeVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResets_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolParkingUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RolId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ParkingId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolParkingUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolParkingUsers_Parkings_ParkingId",
                        column: x => x.ParkingId,
                        principalTable: "Parkings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolParkingUsers_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolParkingUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    ZonesId = table.Column<int>(type: "integer", nullable: false),
                    TypeVehicleId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sectors_TypeVehicles_TypeVehicleId",
                        column: x => x.TypeVehicleId,
                        principalTable: "TypeVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sectors_Zones_ZonesId",
                        column: x => x.ZonesId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlackList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    RestrictionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlackList_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MembershipTypeId = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PriceAtPurchase = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DurationDays = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memberships_MemberShipTypes_MembershipTypeId",
                        column: x => x.MembershipTypeId,
                        principalTable: "MemberShipTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Memberships_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    SectorsId = table.Column<int>(type: "integer", nullable: false),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slots_Sectors_SectorsId",
                        column: x => x.SectorsId,
                        principalTable: "Sectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegisteredVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    SlotsId = table.Column<int>(type: "integer", nullable: true),
                    Asset = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisteredVehicles_Slots_SlotsId",
                        column: x => x.SlotsId,
                        principalTable: "Slots",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegisteredVehicles_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Forms",
                columns: new[] { "Id", "Asset", "Description", "IsDeleted", "Name" },
                values: new object[] { 1, true, "Formulario principal", false, "Principal" });

            migrationBuilder.InsertData(
                table: "MemberShipTypes",
                columns: new[] { "Id", "Asset", "Description", "DurationDaysBase", "IsDeleted", "Name", "PriceBase" },
                values: new object[,]
                {
                    { 1, true, "Membresía mensual", 30, false, "Mensual", 50m },
                    { 2, true, "Membresía anual", 365, false, "Anual", 500m }
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Asset", "Description", "IsDeleted", "Name" },
                values: new object[] { 1, true, "Módulo de gestión", false, "Gestión" });

            migrationBuilder.InsertData(
                table: "ParkingCategories",
                columns: new[] { "Id", "Asset", "Code", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, "GEN", "Categoría general", false, "General" },
                    { 2, true, "VIP", "Categoría exclusiva", false, "VIP" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Asset", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, "Permiso para ver", false, "Ver" },
                    { 2, true, "Permiso para editar", false, "Editar" },
                    { 3, true, "Permiso para eliminar", false, "Eliminar" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "Age", "Asset", "Document", "Email", "FirstName", "IsDeleted", "LastName", "Phone" },
                values: new object[,]
                {
                    { 1, 30, true, "0001", "admin@gmail.com", "Admin", false, "Principal", "111111111" },
                    { 2, 25, true, "0002", "usuario@gmail.com", "Usuario", false, "Demo", "222222222" },
                    { 3, 18, true, "222222222222", "consumidorFinal@gmail.com", "Consumidor", false, "Final", "222222222222" }
                });

            migrationBuilder.InsertData(
                table: "RatesTypes",
                columns: new[] { "Id", "Asset", "Description", "IsDeleted", "Name" },
                values: new object[] { 1, true, "Tarifa por hora", false, "Hora" });

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "Id", "Asset", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, "Rol de administrador", false, "Administrador" },
                    { 2, true, "Rol de usuario estándar", false, "Usuario" }
                });

            migrationBuilder.InsertData(
                table: "TypeVehicles",
                columns: new[] { "Id", "Asset", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, false, "Auto" },
                    { 2, true, false, "Moto" },
                    { 3, true, false, "Camión" }
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Asset", "IsDeleted", "Name", "PersonId" },
                values: new object[,]
                {
                    { 1, true, false, "Cliente Demo", 1 },
                    { 2, true, false, "Cliente Premium", 2 },
                    { 3, true, false, "Consumidor Final", 3 }
                });

            migrationBuilder.InsertData(
                table: "FormModule",
                columns: new[] { "Id", "Asset", "FormId", "IsDeleted", "ModuleId" },
                values: new object[] { 1, true, 1, false, 1 });

            migrationBuilder.InsertData(
                table: "Parkings",
                columns: new[] { "Id", "Asset", "IsDeleted", "Location", "Name", "ParkingCategoryId" },
                values: new object[,]
                {
                    { 1, true, false, "Centro", "Parqueadero Central", 1 },
                    { 2, true, false, "Norte", "Parqueadero Norte", 2 }
                });

            migrationBuilder.InsertData(
                table: "RolFormPermission",
                columns: new[] { "Id", "Asset", "FormId", "IsDeleted", "PermissionId", "RolId" },
                values: new object[,]
                {
                    { 1, true, 1, false, 1, 1 },
                    { 2, true, 1, false, 2, 1 },
                    { 3, true, 1, false, 1, 2 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Asset", "Email", "IsDeleted", "Password", "PersonId", "Username" },
                values: new object[,]
                {
                    { 1, true, "admin@mail.com", false, "$2a$12$C3DSGP6PRwi3a4hsLdnrs.kYnRkJ0PgR3ky/AbI5Dmem7U3e/lSpq", 1, "admin" },
                    { 2, true, "usuario@mail.com", false, "$2a$12$bvkOemZZo7d029/kwq5Duudeamk/pxdPn464EZOT6Ndbg6z06h.Gm", 2, "usuario" },
                    { 3, true, "usuario@mail.com", false, "$2a$12$bvkOemZZo7d029/kwq5Duudeamk/pxdPn464EZOT6Ndbg6z06h.Gm", 3, "Consumidor Final" }
                });

            migrationBuilder.InsertData(
                table: "Cameras",
                columns: new[] { "Id", "Asset", "IsDeleted", "Name", "ParkingId", "Resolution", "Url" },
                values: new object[,]
                {
                    { 1, true, false, "Cam 1", 1, "1080p", "http://cam1" },
                    { 2, true, false, "Cam VIP", 2, "4K", "http://cam-vip" }
                });

            migrationBuilder.InsertData(
                table: "Rates",
                columns: new[] { "Id", "Amount", "Asset", "EndHour", "IsDeleted", "Name", "ParkingId", "RatesTypeId", "StarHour", "Type", "TypeVehicleId", "Year" },
                values: new object[,]
                {
                    { 1, 2.5m, true, new DateTime(2025, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc), false, "Tarifa Día", 1, 1, new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), "Hora", 1, 2025 },
                    { 2, 1.5m, true, new DateTime(2025, 1, 2, 6, 0, 0, 0, DateTimeKind.Utc), false, "Tarifa Noche", 1, 1, new DateTime(2025, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc), "Hora", 2, 2025 },
                    { 3, 5m, true, new DateTime(2025, 1, 1, 20, 0, 0, 0, DateTimeKind.Utc), false, "Tarifa VIP", 2, 1, new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), "Hora", 1, 2025 }
                });

            migrationBuilder.InsertData(
                table: "RolParkingUsers",
                columns: new[] { "Id", "Asset", "IsDeleted", "ParkingId", "RolId", "UserId" },
                values: new object[,]
                {
                    { 1, true, false, 1, 1, 1 },
                    { 2, true, false, 2, 2, 2 },
                    { 3, true, false, 2, 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "Asset", "ClientId", "Color", "IsDeleted", "Plate", "TypeVehicleId" },
                values: new object[,]
                {
                    { 1, true, 1, "Rojo", false, "ABC123", 1 },
                    { 2, true, 1, "Negro", false, "XYZ987", 2 },
                    { 3, true, 2, "Blanco", false, "TRK456", 3 }
                });

            migrationBuilder.InsertData(
                table: "Zones",
                columns: new[] { "Id", "Asset", "IsDeleted", "Name", "ParkingId" },
                values: new object[,]
                {
                    { 1, true, false, "Zona A", 1 },
                    { 2, true, false, "Zona B", 1 },
                    { 3, true, false, "Zona VIP", 2 }
                });

            migrationBuilder.InsertData(
                table: "BlackList",
                columns: new[] { "Id", "Asset", "IsDeleted", "Reason", "RestrictionDate", "VehicleId" },
                values: new object[] { 1, true, false, "Infracción grave", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3 });

            migrationBuilder.InsertData(
                table: "Memberships",
                columns: new[] { "Id", "Asset", "Currency", "DurationDays", "EndDate", "IsDeleted", "MembershipTypeId", "PriceAtPurchase", "StartDate", "VehicleId" },
                values: new object[,]
                {
                    { 1, true, "USD", 30, new DateTime(2025, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, 50m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, true, "USD", 365, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, 500m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 }
                });

            migrationBuilder.InsertData(
                table: "Sectors",
                columns: new[] { "Id", "Asset", "Capacity", "IsDeleted", "Name", "TypeVehicleId", "ZonesId" },
                values: new object[,]
                {
                    { 1, true, 10, false, "Sector 1", 1, 1 },
                    { 2, true, 5, false, "Sector 2", 2, 2 },
                    { 3, true, 3, false, "Sector VIP", 1, 3 }
                });

            migrationBuilder.InsertData(
                table: "Slots",
                columns: new[] { "Id", "Asset", "IsAvailable", "IsDeleted", "Name", "SectorsId" },
                values: new object[,]
                {
                    { 1, true, true, false, "A1", 1 },
                    { 2, true, true, false, "A2", 1 },
                    { 3, true, true, false, "B1", 2 },
                    { 4, true, true, false, "VIP1", 3 }
                });

            migrationBuilder.InsertData(
                table: "RegisteredVehicles",
                columns: new[] { "Id", "Asset", "EntryDate", "ExitDate", "IsDeleted", "SlotsId", "Status", "VehicleId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), false, 1, "Out", 1 },
                    { 2, true, new DateTime(2025, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), null, false, 3, "In", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlackList_VehicleId",
                table: "BlackList",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_ParkingId",
                table: "Cameras",
                column: "ParkingId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_PersonId",
                table: "Clients",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormModule_FormId",
                table: "FormModule",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormModule_ModuleId",
                table: "FormModule",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_MembershipTypeId",
                table: "Memberships",
                column: "MembershipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_VehicleId",
                table: "Memberships",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ParkingId",
                table: "Notifications",
                column: "ParkingId");

            migrationBuilder.CreateIndex(
                name: "IX_Parkings_ParkingCategoryId",
                table: "Parkings",
                column: "ParkingCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_UsuarioId",
                table: "PasswordResets",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_ParkingId",
                table: "Rates",
                column: "ParkingId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_RatesTypeId",
                table: "Rates",
                column: "RatesTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_TypeVehicleId",
                table: "Rates",
                column: "TypeVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredVehicles_SlotsId",
                table: "RegisteredVehicles",
                column: "SlotsId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredVehicles_VehicleId",
                table: "RegisteredVehicles",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolFormPermission_FormId",
                table: "RolFormPermission",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_RolFormPermission_PermissionId",
                table: "RolFormPermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolFormPermission_RolId",
                table: "RolFormPermission",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_RolParkingUsers_ParkingId",
                table: "RolParkingUsers",
                column: "ParkingId");

            migrationBuilder.CreateIndex(
                name: "IX_RolParkingUsers_RolId",
                table: "RolParkingUsers",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_RolParkingUsers_UserId",
                table: "RolParkingUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_TypeVehicleId",
                table: "Sectors",
                column: "TypeVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_ZonesId",
                table: "Sectors",
                column: "ZonesId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_SectorsId",
                table: "Slots",
                column: "SectorsId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonId",
                table: "Users",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ClientId",
                table: "Vehicles",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TypeVehicleId",
                table: "Vehicles",
                column: "TypeVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_ParkingId",
                table: "Zones",
                column: "ParkingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlackList");

            migrationBuilder.DropTable(
                name: "Cameras");

            migrationBuilder.DropTable(
                name: "FormModule");

            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PasswordResets");

            migrationBuilder.DropTable(
                name: "Rates");

            migrationBuilder.DropTable(
                name: "RegisteredVehicles");

            migrationBuilder.DropTable(
                name: "RolFormPermission");

            migrationBuilder.DropTable(
                name: "RolParkingUsers");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "MemberShipTypes");

            migrationBuilder.DropTable(
                name: "RatesTypes");

            migrationBuilder.DropTable(
                name: "Slots");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "TypeVehicles");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Parkings");

            migrationBuilder.DropTable(
                name: "ParkingCategories");
        }
    }
}
