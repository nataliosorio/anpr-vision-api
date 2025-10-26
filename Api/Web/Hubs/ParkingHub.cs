using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class ParkingHub : Hub
    {
        /// <summary>
        /// Envía una notificación a todos los clientes conectados al grupo de un parking.
        /// </summary>
        public async Task SendNotification(int parkingId, object notification)
        {
            await Clients.Group(parkingId.ToString())
                .SendAsync("ReceiveNotification", notification);
        }

        /// <summary>
        /// Cuando un cliente se conecta, lo unimos al grupo de su parking.
        /// El parkingId se espera en el query string de la conexión.
        /// Ejemplo: ws://localhost:5000/parkingHub?parkingId=1
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var parkingId = httpContext?.Request.Query["parkingId"].ToString();

            if (!string.IsNullOrEmpty(parkingId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, parkingId);
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Cuando un cliente se desconecta, lo removemos de su grupo.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var parkingId = httpContext?.Request.Query["parkingId"].ToString();

            if (!string.IsNullOrEmpty(parkingId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, parkingId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
