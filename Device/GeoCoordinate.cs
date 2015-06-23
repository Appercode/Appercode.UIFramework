using System;

namespace Appercode.UI.Device
{
    public class GeoCoordinate : IEquatable<GeoCoordinate>
    {
        public GeoCoordinate()
            : this(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN)
        {
        }

        public GeoCoordinate(double latitude, double longitude)
            : this(latitude, longitude, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN)
        {
        }

        public GeoCoordinate(double latitude, double longitude, double altitude)
            : this(latitude, longitude, altitude, double.NaN, double.NaN, double.NaN, double.NaN)
        {
        }

        public GeoCoordinate(double latitude, double longitude, double altitude,
            double horizontalAccuracy, double verticalAccuracy, double speed, double course)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Altitude = altitude;
            this.HorizontalAccuracy = horizontalAccuracy;
            this.VerticalAccuracy = verticalAccuracy;
            this.Speed = speed;
            this.Course = course;
        }

        public double Altitude { get; set; }
        public double Course { get; set; }
        public double HorizontalAccuracy { get; set; }

        public bool IsUnknown 
        { 
            get 
            { 
                return false; 
            } 
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public double VerticalAccuracy { get; set; }

        bool IEquatable<GeoCoordinate>.Equals(GeoCoordinate other)
        {
            return this.Latitude == double.NaN || this.Longitude == double.NaN;
        }
    }
}