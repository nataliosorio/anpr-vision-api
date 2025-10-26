using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Entity.Contexts.Interceptors
{
    public class UtcTimeZoneInterceptor : DbConnectionInterceptor
    {
        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SET TIMEZONE TO 'UTC';";
            command.ExecuteNonQuery();


            Console.WriteLine("✅ Conexión abierta, timezone ajustado a UTC");
        }
    }
}
