using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entity.Entity.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class Menu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Forms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Url" },
                values: new object[] { "Vista principal que muestra estadísticas, métricas y resumen general del sistema.", "Dashboard", "/dashboard" });

            migrationBuilder.InsertData(
                table: "Forms",
                columns: new[] { "Id", "Asset", "Description", "IsDeleted", "Name", "Url" },
                values: new object[,]
                {
                    { 2, true, "Interfaz para el monitoreo en tiempo real de las cámaras del parqueadero.", false, "Cámaras", "/cameras-index" },
                    { 3, true, "Formulario para la gestión y consulta de los vehículos registrados en el sistema.", false, "Vehículos", "/vehicles-index" },
                    { 4, true, "Vista que permite visualizar y administrar la distribución física del parqueadero.", false, "Distribución del Parqueadero", "/parking-management" },
                    { 5, true, "Formulario para la creación, edición y gestión de tarifas aplicables.", false, "Tarifas", "/rates-index" },
                    { 6, true, "Interfaz para la visualización y control de los registros de entrada de vehículos.", false, "Registros de Entradas", "/registeredVehicle-index" },
                    { 7, true, "Módulo de control para la administración de la lista negra de vehículos o clientes.", false, "Lista Negra", "/blackList-index" },
                    { 8, true, "Formulario para la gestión de clientes asociados al parqueadero.", false, "Clientes", "/client-index" },
                    { 9, true, "Gestión de los tipos de vehículos admitidos en el sistema.", false, "Tipo de Vehículos", "/TypeVehicle-index" },
                    { 10, true, "Gestión de las categorías y tipos de tarifas disponibles en el sistema.", false, "Tipo de Tarifas", "/RatesType-index" },
                    { 11, true, "Formulario para administrar las zonas del parqueadero.", false, "Zonas", "/Zones-index" },
                    { 12, true, "Interfaz para la gestión y configuración de los sectores dentro del parqueadero.", false, "Sectores", "/sectors-index" },
                    { 13, true, "Formulario encargado de la administración de los espacios de parqueo.", false, "Espacios", "/slots-index" },
                    { 14, true, "Gestión y configuración de los roles del sistema.", false, "Roles", "/role-index" },
                    { 15, true, "Formulario para administrar los permisos disponibles del sistema.", false, "Permisos", "/permission-index" },
                    { 16, true, "Gestión de los módulos funcionales del sistema.", false, "Módulos", "/module-index" },
                    { 17, true, "Formulario para relacionar formularios con módulos.", false, "Formularios por Módulos", "/form-module-index" },
                    { 18, true, "Gestión de los permisos asignados a roles por cada formulario.", false, "Permisos por Roles y Formularios", "/rolFormPermission-index" },
                    { 19, true, "Formulario para la administración de usuarios del sistema.", false, "Usuarios", "/user-index" },
                    { 20, true, "Gestión de la información personal asociada a los usuarios.", false, "Personas", "/persons-index" }
                });

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Módulo principal que muestra estadísticas, métricas y resumen general del sistema.", "Dashboard" });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Asset", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 2, true, "Módulo encargado del monitoreo en tiempo real y control del acceso de vehículos.", false, "Monitoreo y Control de Acceso" },
                    { 3, true, "Módulo para la gestión física del parqueadero: zonas, sectores y espacios disponibles.", false, "Módulo de Infraestructura" },
                    { 4, true, "Módulo encargado de las operaciones principales del sistema, incluyendo control, registro y supervisión de procesos en el parqueadero.", false, "Módulo Operacional" },
                    { 5, true, "Módulo encargado de la configuración y gestión de los parámetros generales del sistema, como tipos, categorías y valores base.", false, "Módulo de Parámetros" },
                    { 6, true, "Módulo encargado de la administración de usuarios, roles, permisos y control de acceso al sistema.", false, "Módulo de Seguridad" }
                });

            migrationBuilder.UpdateData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FormId", "PermissionId" },
                values: new object[] { 2, 1 });

            migrationBuilder.UpdateData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FormId", "RolId" },
                values: new object[] { 3, 1 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "nataliaosorio973@gmail.com");

            migrationBuilder.InsertData(
                table: "FormModule",
                columns: new[] { "Id", "Asset", "FormId", "IsDeleted", "ModuleId" },
                values: new object[,]
                {
                    { 2, true, 2, false, 2 },
                    { 3, true, 4, false, 2 },
                    { 4, true, 6, false, 4 },
                    { 5, true, 7, false, 4 },
                    { 6, true, 3, false, 4 },
                    { 7, true, 8, false, 5 },
                    { 8, true, 5, false, 5 },
                    { 9, true, 10, false, 5 },
                    { 10, true, 12, false, 5 },
                    { 11, true, 13, false, 5 },
                    { 12, true, 9, false, 5 },
                    { 13, true, 11, false, 5 },
                    { 14, true, 14, false, 6 },
                    { 15, true, 15, false, 6 },
                    { 16, true, 16, false, 6 },
                    { 17, true, 17, false, 6 },
                    { 18, true, 18, false, 6 },
                    { 19, true, 19, false, 6 },
                    { 20, true, 20, false, 6 }
                });

            migrationBuilder.InsertData(
                table: "RolFormPermission",
                columns: new[] { "Id", "Asset", "FormId", "IsDeleted", "PermissionId", "RolId" },
                values: new object[,]
                {
                    { 4, true, 4, false, 1, 1 },
                    { 5, true, 5, false, 1, 1 },
                    { 6, true, 6, false, 1, 1 },
                    { 7, true, 7, false, 1, 1 },
                    { 8, true, 8, false, 1, 1 },
                    { 9, true, 9, false, 1, 1 },
                    { 10, true, 10, false, 1, 1 },
                    { 11, true, 11, false, 1, 1 },
                    { 12, true, 12, false, 1, 1 },
                    { 13, true, 13, false, 1, 1 },
                    { 14, true, 14, false, 1, 1 },
                    { 15, true, 15, false, 1, 1 },
                    { 16, true, 16, false, 1, 1 },
                    { 17, true, 17, false, 1, 1 },
                    { 18, true, 18, false, 1, 1 },
                    { 19, true, 19, false, 1, 1 },
                    { 20, true, 20, false, 1, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "FormModule",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Forms");

            migrationBuilder.UpdateData(
                table: "Forms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Formulario principal", "Principal" });

            migrationBuilder.UpdateData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Módulo de gestión", "Gestión" });

            migrationBuilder.UpdateData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FormId", "PermissionId" },
                values: new object[] { 1, 2 });

            migrationBuilder.UpdateData(
                table: "RolFormPermission",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FormId", "RolId" },
                values: new object[] { 1, 2 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "admin@mail.com");
        }
    }
}
