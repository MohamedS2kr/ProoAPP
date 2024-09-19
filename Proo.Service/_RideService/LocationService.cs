using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service._RideService
{
    public class LocationService
    {
        private const double EarthRadiusKm = 6371;

        public double HaversineDistance(double lat1, double lat2, double lon1, double lon2)
        {
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        public double CalculatedTime(double distance, double averageSpeed = 50) // السرعة بالكيلومترات في الساعة
        {
            return distance / averageSpeed * 60; // الوقت بالدقائق
        }

        public double CalculatePrice(double distance, double baseFare = 5, double costPerKm = 2) /// pendding 
        {
            return baseFare + (distance * costPerKm);
        }

        public (double distance, double estimatedTime, double price) CalculateDestanceAndTimeAndPrice(double startLat, double startLon, double endLat, double endLon)
        {
            double distance = HaversineDistance(startLat, endLat, startLon, endLon);
            double estimatedTime = CalculatedTime(distance);
            double price = CalculatePrice(distance);

            return (distance, estimatedTime, price);
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
