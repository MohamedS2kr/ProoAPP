using Microsoft.AspNetCore.SignalR;

namespace Proo.APIs.Hubs
{
    public class LocationHub : Hub
    {
        public async Task UpdateLocation(string driverId, double latitude, double longitude)
        {
            // إرسال الموقع الجديد لجميع العملاء المتصلين
            await Clients.All.SendAsync("ReceiveLocationUpdate", driverId, latitude, longitude);
        }
    }
}
