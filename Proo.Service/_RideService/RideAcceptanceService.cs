using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service._RideService
{
    public interface IRideAcceptanceService
    {
        TaskCompletionSource<bool> GetOrAddRideAcceptanceSource(string driverId);
        void SetRideAcceptance(string driverId, bool isAccepted);
    }

    public class RideAcceptanceService : IRideAcceptanceService
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _rideAcceptanceSources = new();

        public TaskCompletionSource<bool> GetOrAddRideAcceptanceSource(string driverId)
        {
            return _rideAcceptanceSources.GetOrAdd(driverId, _ => new TaskCompletionSource<bool>());
        }

        public void SetRideAcceptance(string driverId, bool isAccepted)
        {
            if (_rideAcceptanceSources.TryRemove(driverId, out var tcs))
            {
                tcs.SetResult(isAccepted);
            }
        }
    }
}
