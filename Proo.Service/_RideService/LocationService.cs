﻿using System;
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

        public double CalculatePrice(double distance, double baseFare = 5, double costPerKm = 10) /// pendding 
        {
            return baseFare + (distance * costPerKm);
        }

        public (double distance, double estimatedTime, double price) CalculateDestanceAndTimeAndPrice(double startLat, double startLon, double endLat, double endLon , string category)
        {
            double distance = HaversineDistance(startLat, endLat, startLon, endLon);
            double estimatedTime = Math.Round(CalculatedTime(distance), 2);
            double price = Math.Round(CalculatePrice(distance),2);
            if (category == "Ride")
                return (distance, estimatedTime, price);
            else if (category == "Comfort")
                return (distance, estimatedTime, price * 1.5);
            else if (category == "Scoter")
                return (distance, estimatedTime, price / 0.50);
            else if (category == "FastTripe")
                return (distance, estimatedTime, price * 2);
            else
                return (distance, estimatedTime, price);
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
